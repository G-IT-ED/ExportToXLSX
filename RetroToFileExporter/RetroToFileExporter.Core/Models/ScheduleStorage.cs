using System;
using System.Collections.Generic;
using System.Data.SQLite;
using RetroToFileExporter.Core.Interfaces;
using RSDU.Messaging;

namespace RetroToFileExporter.Core.Models
{
    public class ScheduleStorage : IScheduleSettings
    {
        private readonly Object _lockConfDb = new Object();

        private readonly string _confDbDsn;
        private string _configDBFile;

        protected Object DbLockObject { get { return _lockConfDb; } }

        public ScheduleStorage(string configDbFile)
        {           
            _confDbDsn = String.Format("Data Source={0};", configDbFile);
            _configDBFile = configDbFile;
        }

        public ScheduleStorage(string configDbFile, Object lockDbFile)
        {
            _configDBFile = configDbFile;
            _confDbDsn = String.Format("Data Source={0};", configDbFile);
            _lockConfDb = lockDbFile;
        }
        public Schedule GetScheduleByGuid(Guid guid)
        {
            Schedule val = null;
            lock (DbLockObject)
            {
                using (SQLiteConnection connection = new SQLiteConnection(_confDbDsn))
                {
                    try
                    {
                        connection.Open();
                        string sql = "select * from schedules where GuidSchedule = @guid";
                        using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                        {
                            SQLiteParameter tr = new SQLiteParameter("@guid") { Value = guid.ToString() };
                            command.Parameters.Add(tr);

                            using (SQLiteDataReader reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                    val = CreateSettingBySqlReader(reader);
                            }
                        }
                        val.Actions = GetTriggersBySchedule(val);
                    }
                    catch (Exception e)
                    {
                        Log.Error("Не удалось получить расписание:" + guid, e);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }

            return val;
        }

        public List<Schedule> GetAllSchedule()
        {
            List<Schedule> lst = new List<Schedule>();
            lock (DbLockObject)
            {
                using (SQLiteConnection connection = new SQLiteConnection(_confDbDsn))
                {
                    try
                    {
                        connection.Open();
                        string sql = "select * from schedules";
                        using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                        {
                            using (SQLiteDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    Schedule g = CreateSettingBySqlReader(reader);
                                    lst.Add(g);
                                }
                            }
                        }
                        foreach (var schedule in lst)
                        {
                            schedule.Actions = GetTriggersBySchedule(schedule);
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error("Не удалось получить все расписания:", e);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }

            return lst;
        }

        private List<Action> GetTriggersBySchedule(Schedule schedule)
        {
            List<Action> lst = new List<Action>();
            lock (DbLockObject)
            {
                using (SQLiteConnection connection = new SQLiteConnection(_confDbDsn))
                {
                    try
                    {
                        connection.Open();
                        string sql = "select * from schedule_triggers where scheduleGuid = @scheduleGuid";
                        using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                        {
                            SQLiteParameter tr = new SQLiteParameter("@scheduleGuid") { Value = schedule.GuidSchedule.ToString() };
                            command.Parameters.Add(tr);

                            using (SQLiteDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    Action t = CreateActionBySqlReader(reader);
                                    lst.Add(t);
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        throw (e);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }

            return lst;
        }

        private Action CreateActionBySqlReader(SQLiteDataReader reader)
        {
            string guidTrigger = Convert.ToString(reader["triggerGuid"]);
            ActionsStorage actionsStorage = new ActionsStorage(_configDBFile);
            var result = actionsStorage.GetTriggerByGuid(new Guid(guidTrigger));
            return result;
        }

        public void AddSchedule(Schedule schedule)
        {
            lock (DbLockObject)
            {
                using (SQLiteConnection connection = new SQLiteConnection(_confDbDsn))
                {
                    try
                    {
                        connection.Open();
                        string sql = "Insert into  schedules (NameSchedule,ScheduleQuartz,IsEnabledShedule,GuidSchedule) " +
                                     "values (" +
                                     "@NameSchedule, " +
                                     "@ScheduleQuartz, " +
                                     "@IsEnabledShedule, " +
                                     "@GuidSchedule)";
                        using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@GuidSchedule", schedule.GuidSchedule.ToString());
                            command.Parameters.AddWithValue("@NameSchedule", schedule.NameSchedule);
                            command.Parameters.AddWithValue("@ScheduleQuartz", schedule.ScheduleQuartz);
                            command.Parameters.AddWithValue("@IsEnabledShedule", schedule.IsEnabledShedule);
                            command.ExecuteNonQuery();
                        }
                        if (schedule.Actions != null)
                        {
                            foreach (var trigger in schedule.Actions)
                            {
                                AddTrigger(schedule.GuidSchedule, trigger.GuidAction);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error("Не удалось добавить расписание:" + schedule.GuidSchedule, e);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }    
        }

        public void DeleteSchedule(Schedule schedule)
        {
            lock (DbLockObject)
            {
                using (SQLiteConnection connection = new SQLiteConnection(_confDbDsn))
                {
                    try
                    {
                        connection.Open();
                        string sql = "DELETE FROM schedules " +
                                     "WHERE GuidSchedule = @Guid";
                        using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@Guid", schedule.GuidSchedule.ToString());
                            command.ExecuteNonQuery();
                        }
                        if (schedule.Actions != null)
                        {
                            foreach (var trigger in schedule.Actions)
                            {
                                DeleteTrigger(schedule.GuidSchedule, trigger.GuidAction);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error("Не удалось удалить расписание:" + schedule.GuidSchedule, e);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }


        public Schedule CreateSettingBySqlReader(SQLiteDataReader reader)
        {
            string guidSchedule = Convert.ToString(reader["GuidSchedule"]);
            string nameSchedule = Convert.ToString(reader["NameSchedule"]);
            string scheduleQuartz = Convert.ToString(reader["ScheduleQuartz"]);
            bool isEnabledShedule = Convert.ToBoolean(reader["IsEnabledShedule"]);
            return new Schedule(new Guid(guidSchedule), nameSchedule, scheduleQuartz, isEnabledShedule);
        }

        public void UpdateSchedule(Schedule schedule)
        {
            lock (DbLockObject)
            {
                using (SQLiteConnection connection = new SQLiteConnection(_confDbDsn))
                {
                    try
                    {
                        connection.Open();
                        string sql = "UPDATE schedules " +
                                     "SET " +
                                     "NameSchedule=@NameSchedule, " +
                                     "ScheduleQuartz=@ScheduleQuartz, " +
                                     "IsEnabledShedule=@IsEnabledShedule " +
                                     "WHERE GuidSchedule = @Guid";
                        using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@Guid", schedule.GuidSchedule.ToString());
                            command.Parameters.AddWithValue("@NameSchedule", schedule.NameSchedule);
                            command.Parameters.AddWithValue("@ScheduleQuartz", schedule.ScheduleQuartz);
                            command.Parameters.AddWithValue("@IsEnabledShedule", schedule.IsEnabledShedule);
                            command.ExecuteNonQuery();
                        }
                        UpdateScheduleTriggers(schedule);
                    }
                    catch (Exception e)
                    {
                        Log.Error("Не удалось обновить расписание:" + schedule.GuidSchedule, e);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        private void UpdateScheduleTriggers(Schedule schedule)
        {
            if (schedule.Actions == null) return;
            
            var oldSchedule = GetScheduleByGuid(schedule.GuidSchedule);
            foreach (var trigger in schedule.Actions)
            {
                if (!oldSchedule.Actions.Contains(trigger))
                    AddTrigger(schedule.GuidSchedule, trigger.GuidAction);
            }
            foreach (var trigger in oldSchedule.Actions)
            {
                if (!schedule.Actions.Contains(trigger))
                    DeleteTrigger(schedule.GuidSchedule, trigger.GuidAction);
            }
        }

        private void DeleteTrigger(Guid guidSchedule, Guid guidTrigger)
        {
            lock (DbLockObject)
            {
                using (SQLiteConnection connection = new SQLiteConnection(_confDbDsn))
                {
                    try
                    {
                        connection.Open();
                        string sql = "DELETE FROM schedule_triggers " +
                                     "WHERE triggerGuid = @triggerGuid " +
                                     "AND scheduleGuid = @scheduleGuid";
                        using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@triggerGuid", guidTrigger.ToString());
                            command.Parameters.AddWithValue("@scheduleGuid", guidSchedule.ToString());
                            command.ExecuteNonQuery();
                        }
                    }
                    catch (Exception e)
                    {
                        throw (e);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        private void AddTrigger(Guid guidSchedule, Guid guidTrigger)
        {
            lock (DbLockObject)
            {
                using (SQLiteConnection connection = new SQLiteConnection(_confDbDsn))
                {
                    try
                    {
                        connection.Open();
                        string sql = "Insert into  schedule_triggers (scheduleGuid,triggerGuid) " +
                                     "values (" +
                                     "@scheduleGuid, " +
                                     "@triggerGuid)";
                        using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@scheduleGuid", guidSchedule.ToString());
                            command.Parameters.AddWithValue("@triggerGuid", guidTrigger.ToString());
                            command.ExecuteNonQuery();
                        }
                    }
                    catch (Exception e)
                    {
                        throw (e);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        public Action GetActionByGuid(Guid guid)
        {
            Action val = null;
            lock (DbLockObject)
            {
                using (SQLiteConnection connection = new SQLiteConnection(_confDbDsn))
                {
                    try
                    {
                        connection.Open();
                        string sql = "select * from schedule_triggers where GuidTrigger = @GuidTrigger";
                        using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                        {
                            SQLiteParameter tr = new SQLiteParameter("@GuidTrigger") { Value = guid.ToString() };
                            command.Parameters.Add(tr);

                            using (SQLiteDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    val = CreateActionBySqlReader(reader);
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        throw (e);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }

            return val;
        }
    }
}