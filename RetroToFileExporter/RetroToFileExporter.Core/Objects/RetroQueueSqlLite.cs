using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Xml.Serialization;
using RetroToFileExporter.Core.Interfaces;
using RetroToFileExporter.Core.Models;
using RSDU.Messaging;

namespace RetroToFileExporter.Core.Objects
{
    public class RetroQueueSqlLite : IQueueAdminAction, IQueueConsumerAction, IQueuePublisherAction
    {
        private const String NameTh = "RetroQueueSqlLite";

        /// <summary>
        /// Объект блокировки доступа к БД SQLite
        /// Используется для блокировки доступа с БД из нескольких потоков
        /// </summary>
        private  Object _lockDb = new Object();

        private readonly string _dbDsn;

        public RetroQueueSqlLite(string dbFile)
        {
            _dbDsn = String.Format("Data Source={0};", dbFile);
        }

        public RetroQueueSqlLite(string configDbFile, Object lockDbFile)
        {
            _dbDsn = String.Format("Data Source={0};", configDbFile);
            _lockDb = lockDbFile;
        }

        public void ClearQueue()
        {
            ListQueueObjects.ListQueueActions.Clear();
        }

        public void DeleteRecord(QueueObjectAction record)
        {
            #region Удаляем задачу из памяти
            if (!record.Action.IsHighLevelReliability)
            {
                int i = 0;
                ListQueueObjects.ListQueueActions.TryRemove(record, out i);
                return;
            }
            #endregion
            #region Удаляем задачу из базы
            lock (_lockDb)
            {
                using (SQLiteConnection connection = new SQLiteConnection(_dbDsn))
                {
                    try
                    {
                        connection.Open();
                        string sql = "delete from retro_queue where GUID = @GUID";

                        using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                        {
                            SQLiteParameter guid = new SQLiteParameter("@GUID") {Value = record.Guid.ToString()};
                            command.Parameters.Add(guid);

                            command.ExecuteNonQuery();
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error(NameTh + "> ОШИБКА!!! Возникла ошибка при удалении объекта из очереди! ID=" + record.Guid, e);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            #endregion
        }

        

        public event EventHandler OnQueueChanged;
        public QueueObjectAction GetFirst()
        {
            List<QueueObjectAction> result = new List<QueueObjectAction>();
            Random r = new Random();

            if (ListQueueObjects.ListQueueActions != null && !ListQueueObjects.ListQueueActions.IsEmpty)
            {
                result.AddRange(ListQueueObjects.ListQueueActions.Keys);
            }

            lock (_lockDb)
            {
                using (SQLiteConnection connection = new SQLiteConnection(_dbDsn))
                {
                    try
                    {
                        connection.Open();
                        string sql = "select * from retro_queue";
                        using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                        {
                            using (SQLiteDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    QueueObjectAction g = FillQueueAction(reader);
                                    result.Add(g);
                                }
                            }
                        }

                    }
                    catch (Exception e)
                    {
                        Log.Error(NameTh + "> ОШИБКА!!! Возникла ошибка при получении объекта из очереди!", e);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }

            if (result.Count == 0) return null;
            List<QueueObjectAction> listQueue = new List<QueueObjectAction>();
            List<QueueObjectAction> listErrorQueue = new List<QueueObjectAction>();
            foreach (var action in result)
            {
                if (action.DateError == new DateTime())
                    listQueue.Add(action);
                else
                    listErrorQueue.Add(action);
            }
            return listQueue.Count > 0 ? listQueue[r.Next(0, listQueue.Count - 1)] : listErrorQueue[r.Next(0, listErrorQueue.Count - 1)];
        }

        private QueueObjectAction FillQueueAction(SQLiteDataReader reader)
        {
            String guid = Convert.ToString(reader["Guid"]);
            int dt1970 = Convert.ToInt32(reader["DT1970"]);
            var xml = Convert.ToString(reader["OBJECT_XML"]);
            var countError = Convert.ToInt32(reader["COUNT_ERROR"]);
            return new QueueObjectAction(new Guid(guid), dt1970, GetActionByXml(xml), countError);
        }

        private IAction GetActionByXml(string xml)
        {
            object result;
            XmlSerializer serializer = new XmlSerializer(typeof(Models.Action));
            using (TextReader readers = new StringReader(xml))
            {
                result = serializer.Deserialize(readers);
            }
            IAction action = result as Models.Action;
            return action;
        }

        public bool IsEmptyQueue()
        {
            bool result = false;
            if (!ListQueueObjects.ListQueueActions.IsEmpty) return false;

            lock (_lockDb)
            {
                using (SQLiteConnection connection = new SQLiteConnection(_dbDsn))
                {
                    try
                    {
                        connection.Open();
                        string sql = "select count(*) as QUEUE_COUNT from retro_queue";
                        using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                        {
                            using (SQLiteDataReader reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    int count = Convert.ToInt32(reader["QUEUE_COUNT"]);
                                    result = count == 0;
                                }
                            }
                        }

                    }
                    catch (Exception e)
                    {
                        Log.Error(NameTh + "> ОШИБКА!!! Возникла ошибка при опеределении количества объектов в очереди!", e);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            return result;
        }

        public bool EnqueueList(List<QueueObjectAction> objectLst)
        {
            foreach (var queueObjectAction in objectLst)
            {
                EnqueueObject(queueObjectAction);
            }
            return true;
        }

        public bool EnqueueObject(QueueObjectAction obj)
        {
            #region Складываем задачу в память
            if (!obj.Action.IsHighLevelReliability)
            {
                ListQueueObjects.ListQueueActions.AddOrUpdate(obj, 0, Update);
                ChangeQueue();
                return true;
            }
            #endregion Складываем задачу в базу
            #region Складываем задачу в базу
            lock (_lockDb)
            {
                using (SQLiteConnection connection = new SQLiteConnection(_dbDsn))
                {
                    try
                    {
                        connection.Open();
                        string insSql = "INSERT INTO retro_queue (GUID, DT1970, OBJECT_XML, COUNT_ERROR) " +
                                                    "VALUES (@GUID, @DT1970, @OBJECT_XML, @COUNT_ERROR)";
                        using (SQLiteCommand command = new SQLiteCommand(insSql, connection))
                        {
                            SQLiteParameter guid = new SQLiteParameter("@GUID");
                            SQLiteParameter dt1970 = new SQLiteParameter("@DT1970");
                            SQLiteParameter objectXml = new SQLiteParameter("@OBJECT_XML");
                            SQLiteParameter countError = new SQLiteParameter("@COUNT_ERROR");

                            guid.Value = obj.Guid.ToString();
                            dt1970.Value = obj.Dt1970;
                            objectXml.Value = obj.Action.ToXml();
                            countError.Value = obj.CountError;

                            command.Parameters.Add(guid);
                            command.Parameters.Add(dt1970);
                            command.Parameters.Add(objectXml);
                            command.Parameters.Add(countError);

                            command.ExecuteNonQuery();
                        }
                        ChangeQueue();
                    }
                    catch (Exception e)
                    {
                        Log.Error(NameTh + "> ОШИБКА!!! Возникла ошибка при записи объекта в очередь!", e);
                        return false;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            return true;
            #endregion
        }

        private int Update(QueueObjectAction arg1, int arg2)
        {
            Random r = new Random();
            return r.Next(0, 100);
        }

        public int Count()
        {
            var result = ListQueueObjects.ListQueueActions.Count;

            lock (_lockDb)
            {
                using (SQLiteConnection connection = new SQLiteConnection(_dbDsn))
                {
                    try
                    {
                        connection.Open();
                        string sql = "select count(*) as QUEUE_COUNT from retro_queue";
                        using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                        {
                            using (SQLiteDataReader reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    int count = Convert.ToInt32(reader["QUEUE_COUNT"]);
                                    result += count;
                                }
                            }
                        }

                    }
                    catch (Exception e)
                    {
                        Log.Error(NameTh + "> ОШИБКА!!! Возникла ошибка при опеределении количества объектов в очереди!", e);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            return result;
        }
        

        private void ChangeQueue()
        {
            // Очередь была обновлена - вызываем событие обновеления очереди
            // для подписчиков
            if (OnQueueChanged != null)
                OnQueueChanged(this, null);
        }
    }
}