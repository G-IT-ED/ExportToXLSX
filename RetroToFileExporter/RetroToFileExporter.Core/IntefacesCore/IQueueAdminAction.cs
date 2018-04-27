using RetroToFileExporter.Core.Models;

namespace RetroToFileExporter.Core.Interfaces
{
    public interface IQueueAdminAction
    {
        /// <summary>
        /// Метод очистки очереди
        /// </summary>
        void ClearQueue();

        /// <summary>
        /// Метод удаления записи из очереди
        /// </summary>
        /// <param name="record"></param>
        void DeleteRecord(QueueObjectAction record); 
    }
}