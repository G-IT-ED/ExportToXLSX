using System.Collections.Generic;
using RetroToFileExporter.Core.Models;
using RSDU;

namespace RetroToFileExporter.Core.Interfaces
{
    public interface IQueueAdminSchedule
    {
        /// <summary>
        /// Метод очистки очереди
        /// </summary>
        void ClearQueue();
        
        /// <summary>
        /// Метод удаления записи из очереди
        /// </summary>
        /// <param name="record"></param>
        void DeleteRecord(QueueObjectSchedule record);
    }
}