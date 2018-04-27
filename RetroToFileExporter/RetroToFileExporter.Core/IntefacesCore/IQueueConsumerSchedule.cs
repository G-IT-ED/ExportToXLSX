using System;
using RetroToFileExporter.Core.Models;

namespace RetroToFileExporter.Core.Interfaces
{
    public interface IQueueConsumerSchedule
    {
        /// <summary>
        /// Событие возникающее при добавлении новых объектов в очередь
        /// </summary>
        event EventHandler OnQueueChanged;

        /// <summary>
        /// Метод получения первого на очереди
        /// </summary>
        /// <returns>Объект очереди, первый к обработке</returns>
        QueueObjectSchedule GetFirst();
        
        /// <summary>
        /// Пуста ли очередь?
        /// </summary>
        /// <returns>true - очередь пуста, false - очередь не пуста</returns>
        bool IsEmptyQueue();

        /// <summary>
        /// Получить action по guid
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        IAction GetActionByGuid(Guid guid);

        /// <summary>
        /// Хранилище расписаний
        /// </summary>
        ScheduleStorage ScheduleStorage { get; set; }

        /// <summary>
        /// Получить расписание по guid
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        ISchedule GetScheduleByGuid(Guid guid);
    }
}