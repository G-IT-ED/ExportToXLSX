using System;
using System.Collections.Generic;
using System.Data.SQLite;
using RetroToFileExporter.Core.Interfaces;
using RetroToFileExporter.Core.Objects;
using RSDU.Messaging;

namespace RetroToFileExporter.Core.Models
{
    public class ActionsStorage : IActionSettings
    {
        private readonly object _lockConfDb;

        private readonly string _confDbDsn;

        public ActionsStorage(string configDbFile)
        {
            _confDbDsn = string.Format("Data Source={0};", configDbFile);
            _lockConfDb = new object();

            CheckAndFixDB();
        }

        public ActionsStorage(string configDbFile, object lockDbFile)
        {
            _confDbDsn = string.Format("Data Source={0};", configDbFile);
            _lockConfDb = lockDbFile;

            CheckAndFixDB();
        }

        // Проверяет правильность структуры таблиц БД и при необходимости исправляет её.
        // Метод добавлен из-за необходимости добавления нового параметра для действий RunTimeOffset, 
        // чтобы у заказчиков всё заработало автоматически
        void CheckAndFixDB()
        {
            lock (_lockConfDb)
            {
                using (SQLiteConnection connection = new SQLiteConnection(_confDbDsn))
                {
                    try
                    {
                        connection.Open();

                        bool runTimeOffsetColumnExist = false;

                        string sql = "PRAGMA table_info('triggers')";

                        using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string columnName = reader.GetString(1);

                                if (columnName == "RunTimeOffset")
                                {
                                    runTimeOffsetColumnExist = true;
                                    break;
                                }
                            }
                        }

                        if (!runTimeOffsetColumnExist)
                        {
                            string q = "ALTER TABLE triggers ADD COLUMN RunTimeOffset INTEGER DEFAULT 0;";

                            using (SQLiteCommand command = new SQLiteCommand(q, connection))
                            {
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Не удалось произвести обновление структуры файла конфигурации: не удалось добавить столбец RunTimeOffset в таблицу triggers.", ex);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        public Action GetTriggerByGuid(Guid guid)
        {
            Action val = null;
            lock (_lockConfDb)
            {
                using (SQLiteConnection connection = new SQLiteConnection(_confDbDsn))
                {
                    try
                    {
                        connection.Open();
                        string sql = "select * from triggers where GuidTrigger = @guid";
                        using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                        {
                            SQLiteParameter tr = new SQLiteParameter("@guid") {Value = guid.ToString()};
                            command.Parameters.Add(tr);

                            using (SQLiteDataReader reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                    val = CreateSettingBySqlReader(reader);
                            }
                        }
                        if (val != null) val.QueryConditions = GetQueryConditions(val);
                    }
                    catch (Exception e)
                    {
                        Log.Error("Не удалось получить задачу:" + guid, e);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }

            return val;
        }

        public Action CreateSettingBySqlReader(SQLiteDataReader reader)
        {
            string guidTrigger = Convert.ToString(reader["GuidTrigger"]);
            string name = Convert.ToString(reader["NameTrigger"]);
            string filesFolder = Convert.ToString(reader["FilesFolder"]);
            string fileName = Convert.ToString(reader["FileName"]);
            string tagTime = Convert.ToString(reader["TagTime"]);
            string versionExcel = Convert.ToString(reader["VersionExcel"]);
            bool info = Convert.ToBoolean(reader["Info"]);
            bool objectField = Convert.ToBoolean(reader["ObjectField"]);
            bool typeParam = Convert.ToBoolean(reader["TypeParam"]);
            bool idParam = Convert.ToBoolean(reader["IdParam"]);
            bool idTableParam = Convert.ToBoolean(reader["IdTableParam"]);
            bool nameArchTable = Convert.ToBoolean(reader["NameArchTable"]);
            bool stateColumnHeader = Convert.ToBoolean(reader["StateColumnHeader"]);
            bool isHighLevelReliability = Convert.ToBoolean(reader["IsHighLevelReliability"]);
            int kadrId = Convert.ToInt32(reader["KadrId"]);
            int runTimeOffset = Convert.ToInt32(reader["RunTimeOffset"]);

            var trigger = new Action(new Guid(guidTrigger), name, filesFolder, fileName, tagTime, versionExcel, info, objectField, typeParam,
                                      idParam, idTableParam, nameArchTable, stateColumnHeader, isHighLevelReliability, kadrId, new List<QueryCondition>(), runTimeOffset);

            List<QueryCondition> queryConditions = GetQueryConditions(trigger);
            trigger.QueryConditions = queryConditions;
            return trigger;
        }

        private List<QueryCondition> GetQueryConditions(Action action)
        {
            List<QueryCondition>lst = new List<QueryCondition>();
            lock (_lockConfDb)
            {
                using (SQLiteConnection connection = new SQLiteConnection(_confDbDsn))
                {
                    try
                    {
                        connection.Open();
                        string sql = "select * from query_conditions where GuidTrigger = @guid";
                        using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                        {
                            SQLiteParameter tr = new SQLiteParameter("@guid") { Value = action.GuidAction.ToString() };
                            command.Parameters.Add(tr);

                            using (SQLiteDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    QueryCondition g = CreateQueryBySqlReader(reader);
                                    lst.Add(g);
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



        private QueryCondition CreateQueryBySqlReader(SQLiteDataReader reader)
        {
            string Name = Convert.ToString(reader["Name"]);
            string DirectionTime = Convert.ToString(reader["DirectionTime"]);
            string Interval = Convert.ToString(reader["Interval"]);
            int IntervalCount = Convert.ToInt32(reader["IntervalCount"]);
            int Offset = Convert.ToInt32(reader["Offset"]);
            string GuidTrigger = Convert.ToString(reader["GuidTrigger"]);
            string GuidQuery = Convert.ToString(reader["Guid"]);
            return new QueryCondition(Name, DirectionTime, Interval, IntervalCount, Offset, GuidTrigger, GuidQuery);
        }

        public void UpdateAction(Action action)
        {
            lock (_lockConfDb)
            {
                using (SQLiteConnection connection = new SQLiteConnection(_confDbDsn))
                {
                    try
                    {
                        connection.Open();
                        string sql = "UPDATE triggers " +
                                     "SET NameTrigger=@NameTrigger, " +
                                     "KadrId=@KadrId, " +
                                     "Info=@Info, " +
                                     "ObjectField=@ObjectField, " +
                                     "TypeParam=@TypeParam, " +
                                     "IdParam=@IdParam, " +
                                     "IdTableParam=@IdTableParam, " +
                                     "NameArchTable=@NameArchTable, " +
                                     "StateColumnHeader=@StateColumnHeader, " +
                                     "FilesFolder=@FilesFolder, " +
                                     "FileName=@FileName, " +
                                     "TagTime=@TagTime, " +
                                     "IsHighLevelReliability=@IsHighLevelReliability, " +
                                     "VersionExcel=@VersionExcel, " +
                                     "RunTimeOffset=@RunTimeOffset " +
                                     "WHERE GuidTrigger = @GuidTrigger";
                        using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@NameTrigger", action.NameTrigger);
                            command.Parameters.AddWithValue("@KadrId", action.KadrId);
                            command.Parameters.AddWithValue("@Info", action.Info);
                            command.Parameters.AddWithValue("@ObjectField", action.ObjectField);
                            command.Parameters.AddWithValue("@TypeParam", action.TypeParam);
                            command.Parameters.AddWithValue("@IdParam", action.IdParam);
                            command.Parameters.AddWithValue("@IdTableParam", action.IdTableParam);
                            command.Parameters.AddWithValue("@NameArchTable", action.NameArchTable);
                            command.Parameters.AddWithValue("@StateColumnHeader", action.StateColumnHeader);
                            command.Parameters.AddWithValue("@FilesFolder", action.FilesFolder);
                            command.Parameters.AddWithValue("@FileName", action.FileName);
                            command.Parameters.AddWithValue("@TagTime", action.TagTime);
                            command.Parameters.AddWithValue("@IsHighLevelReliability", action.IsHighLevelReliability);
                            command.Parameters.AddWithValue("@VersionExcel", action.VersionExcel);
                            command.Parameters.AddWithValue("@GuidTrigger", action.GuidAction.ToString());
                            command.Parameters.AddWithValue("@RunTimeOffset", action.RunTimeOffset);

                            command.ExecuteNonQuery();
                        }
                        if (action.QueryConditions == null) return;
                        UpdateQueryCondition(action);
                    }
                    catch (Exception e)
                    {
                        Log.Error("Не удалось обновить Action: " + action.GuidAction, e);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        private void UpdateQueryCondition(Action action)
        {
            if (action.QueryConditions == null) return;
            var oldTrigger = GetTriggerByGuid(action.GuidAction);
            foreach (var queryCondition in action.QueryConditions)
            {
                if (!oldTrigger.QueryConditions.Contains(queryCondition))
                    AddQueryCondition(queryCondition);
            }
            foreach (var queryCondition in oldTrigger.QueryConditions)
            {
                if (!action.QueryConditions.Contains(queryCondition))
                    DeleteQueryCondition(queryCondition);
            }
            foreach (var queryCondition in action.QueryConditions)
            {
                UpdateQuery(queryCondition);
            }

        }

        private void UpdateQuery(QueryCondition queryCondition)
        {
            lock (_lockConfDb)
            {
                using (SQLiteConnection connection = new SQLiteConnection(_confDbDsn))
                {
                    try
                    {
                        connection.Open();
                        string sql = "UPDATE query_conditions " +
                                     "SET " +
                                     "GuidTrigger=@GuidTrigger, " +
                                     "Name=@Name, " +
                                     "DirectionTime=@DirectionTime, " +
                                     "Interval=@Interval, " +
                                     "IntervalCount=@IntervalCount, " +
                                     "Offset=@Offset " +
                                     "WHERE Guid = @Guid";
                        using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@GuidTrigger", queryCondition.GuidTrigger.ToString());
                            command.Parameters.AddWithValue("@Name", queryCondition.Name);
                            command.Parameters.AddWithValue("@DirectionTime", queryCondition.DirectionTime);
                            command.Parameters.AddWithValue("@Interval", queryCondition.Interval);
                            command.Parameters.AddWithValue("@IntervalCount", queryCondition.IntervalCount);
                            command.Parameters.AddWithValue("@Offset", queryCondition.Offset);
                            command.Parameters.AddWithValue("@Guid", queryCondition.GuidQuery.ToString());
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

        public List<Action> GetAllActions()
        {
            List<Action> lst = new List<Action>();
            lock (_lockConfDb)
            {
                using (SQLiteConnection connection = new SQLiteConnection(_confDbDsn))
                {
                    try
                    {
                        connection.Open();
                        string sql = "select * from triggers";
                        using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                        {
                            using (SQLiteDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    Action g = CreateSettingBySqlReader(reader);
                                    lst.Add(g);
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

        public void AddTrigger(Action action)
        {
            lock (_lockConfDb)
            {
                using (SQLiteConnection connection = new SQLiteConnection(_confDbDsn))
                {
                    try
                    {
                        connection.Open();
                        string sql =
                            "Insert into  triggers (KadrId,NameTrigger,Info,ObjectField,TypeParam,IdParam," +
                            "IdTableParam,NameArchTable,StateColumnHeader,FilesFolder,FileName,TagTime," +
                            "IsHighLevelReliability,VersionExcel,GuidTrigger,RunTimeOffset" +
                            ") " +
                            "values (" +
                            "@KadrId," +
                            "@NameTrigger," +
                            "@Info," +
                            "@ObjectField," +
                            "@TypeParam," +
                            "@IdParam," +
                            "@IdTableParam," +
                            "@NameArchTable," +
                            "@StateColumnHeader," +
                            "@FilesFolder," +
                            "@FileName," +
                            "@TagTime," +
                            "@IsHighLevelReliability," +
                            "@VersionExcel," +
                            "@GuidTrigger," +
                            "@RunTimeOffset)";
                        using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@NameTrigger", action.NameTrigger);
                            command.Parameters.AddWithValue("@KadrId", action.KadrId);
                            command.Parameters.AddWithValue("@Info", action.Info);
                            command.Parameters.AddWithValue("@ObjectField", action.ObjectField);
                            command.Parameters.AddWithValue("@TypeParam", action.TypeParam);
                            command.Parameters.AddWithValue("@IdParam", action.IdParam);
                            command.Parameters.AddWithValue("@IdTableParam", action.IdTableParam);
                            command.Parameters.AddWithValue("@NameArchTable", action.NameArchTable);
                            command.Parameters.AddWithValue("@StateColumnHeader", action.StateColumnHeader);
                            command.Parameters.AddWithValue("@FilesFolder", action.FilesFolder);
                            command.Parameters.AddWithValue("@FileName", action.FileName);
                            command.Parameters.AddWithValue("@TagTime", action.TagTime);
                            command.Parameters.AddWithValue("@IsHighLevelReliability", action.IsHighLevelReliability);
                            command.Parameters.AddWithValue("@VersionExcel", action.VersionExcel);
                            command.Parameters.AddWithValue("@GuidTrigger", action.GuidAction.ToString());
                            command.Parameters.AddWithValue("@RunTimeOffset", action.RunTimeOffset);
                            command.ExecuteNonQuery();
                        }
                        foreach (var queryCondition in action.QueryConditions)
                        {
                            AddQueryCondition(queryCondition);
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error("Не удалось добавить триггер:" + action.GuidAction, e);
                        throw (e);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        private void AddQueryCondition(QueryCondition queryCondition)
        {
            lock (_lockConfDb)
            {
                using (SQLiteConnection connection = new SQLiteConnection(_confDbDsn))
                {
                    try
                    {
                        connection.Open();
                        string sql = "Insert into  query_conditions (Name,DirectionTime,Interval,IntervalCount,Offset, Guid, GuidTrigger) " +
                                     "values (" +
                                     "@Name, " +
                                     "@DirectionTime, " +
                                     "@Interval, " +
                                     "@IntervalCount, " +
                                     "@Offset, " +
                                     "@Guid, " +
                                     "@GuidTrigger)";
                        using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@Name", queryCondition.Name);
                            command.Parameters.AddWithValue("@DirectionTime", queryCondition.DirectionTime);
                            command.Parameters.AddWithValue("@Interval", queryCondition.Interval);
                            command.Parameters.AddWithValue("@IntervalCount", queryCondition.IntervalCount);
                            command.Parameters.AddWithValue("@Offset", queryCondition.Offset);
                            command.Parameters.AddWithValue("@Guid", queryCondition.GuidQuery.ToString());
                            command.Parameters.AddWithValue("@GuidTrigger", queryCondition.GuidTrigger.ToString());
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

        public void DeleteTrigger(Action action)
        {
            lock (_lockConfDb)
            {
                using (SQLiteConnection connection = new SQLiteConnection(_confDbDsn))
                {
                    try
                    {
                        connection.Open();
                        string sql = "DELETE FROM triggers " +
                                     "WHERE GuidTrigger = @GuidTrigger";
                        using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@GuidTrigger", action.GuidAction.ToString());
                            command.ExecuteNonQuery();
                        }
                        if (action.QueryConditions == null) return;
                        foreach (var queryCondition in action.QueryConditions)
                        {
                            DeleteQueryCondition(queryCondition);
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error("Не удалось удалить задачу:" + action.GuidAction, e);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        private void DeleteQueryCondition(QueryCondition queryCondition)
        {
            lock (_lockConfDb)
            {
                using (SQLiteConnection connection = new SQLiteConnection(_confDbDsn))
                {
                    try
                    {
                        connection.Open();
                        string sql = "DELETE FROM query_conditions " +
                                     "WHERE Guid = @Guid";
                        using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@Guid", queryCondition.GuidQuery.ToString());
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
    }
}