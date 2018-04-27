using System;
using System.Collections.Concurrent;

namespace RetroToFileExporter.Core.Models
{
    [Serializable]
    public static class ListQueueObjects
    {
        /// <summary>
        /// Очередь для задач расписания
        /// </summary>
        public static ConcurrentDictionary<QueueObjectSchedule, int> ListQueueSchedules = new ConcurrentDictionary<QueueObjectSchedule, int>();

        /// <summary>
        /// Очередь для выгрузки в файл
        /// </summary>
        public static ConcurrentDictionary<QueueObjectAction, int> ListQueueActions = new ConcurrentDictionary<QueueObjectAction, int>();
    }
}