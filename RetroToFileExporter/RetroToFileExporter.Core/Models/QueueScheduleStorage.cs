using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.XtraPrinting.Native;
using RetroToFileExporter.Core.Interfaces;

namespace RetroToFileExporter.Core.Models
{
    public class QueueScheduleStorage : IQueueAdminSchedule, IQueueConsumerSchedule, IQueuePublisherSchedule
    {
        #region IQueueAdminSchedule

        public void ClearQueue()
        {
            ListQueueObjects.ListQueueSchedules.Clear();
        }
        
        public void DeleteRecord(QueueObjectSchedule record)
        {
            int i = 0;
            ListQueueObjects.ListQueueSchedules.TryRemove(record,out i);
        }

        #endregion

        #region IQueueConsumerSchedule
        public QueueObjectSchedule GetFirst()
        {
            if (ListQueueObjects.ListQueueSchedules == null || ListQueueObjects.ListQueueSchedules.Count == 0)
                return null;

            var result = ListQueueObjects.ListQueueSchedules.First();
            DeleteRecord(result.Key);
            return result.Key;
        }

        public event EventHandler OnQueueChanged;

        public bool IsEmptyQueue()
        {
            return ListQueueObjects.ListQueueSchedules.IsEmpty();
        }

        public IAction GetActionByGuid(Guid guid)
        {
            return ScheduleStorage.GetActionByGuid(guid);
        }

        public ScheduleStorage ScheduleStorage { get; set; }
        public ISchedule GetScheduleByGuid(Guid guid)
        {
            return ScheduleStorage.GetScheduleByGuid(guid);
        }

        #endregion

        #region IQueuePublisherSchedule
        public bool EnqueueList(List<QueueObjectSchedule> objectLst)
        {
            try
            {
                foreach (var queueObject in objectLst)
                {
                    ListQueueObjects.ListQueueSchedules.AddOrUpdate(queueObject, 0, Update);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private int Update(QueueObjectSchedule arg1, int arg2)
        {
            return 1;
        }

        public bool EnqueueObject(QueueObjectSchedule obj)
        {
            try
            {
                ListQueueObjects.ListQueueSchedules.AddOrUpdate(obj,0,Update);
                ChangeQueue();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion
        private void ChangeQueue()
        {
            // Очередь была обновлена - вызываем событие обновеления очереди
            // для подписчиков
            if (OnQueueChanged != null)
                OnQueueChanged(this, null);
        }
    }
}