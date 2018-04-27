using System;
using RetroToFileExporter.Core.Models;

namespace RetroToFileExporter.Core.Interfaces
{
    public interface IQueueConsumerAction
    {
        /// <summary>
        /// Событие возникающее при добавлении новых объектов в очередь
        /// </summary>
        event EventHandler OnQueueChanged;

        /// <summary>
        /// Метод получения первого на очереди
        /// </summary>
        /// <returns>Объект очереди, первый к обработке</returns>
        QueueObjectAction GetFirst();

        /// <summary>
        /// Пуста ли очередь?
        /// </summary>
        /// <returns>true - очередь пуста, false - очередь не пуста</returns>
        bool IsEmptyQueue();

        /// <summary>
        /// Удалить элемент очереди
        /// </summary>
        void DeleteRecord(QueueObjectAction record);

        /// <summary>
        /// Добавить элемент очереди
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        bool EnqueueObject(QueueObjectAction obj);

        /// <summary>
        /// Количество объектов в очереди
        /// </summary>
        /// <returns></returns>
        int Count();
    }
}