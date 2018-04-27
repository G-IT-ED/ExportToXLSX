﻿using System.Collections.Generic;
using RetroToFileExporter.Core.Models;

namespace RetroToFileExporter.Core.Interfaces
{
    public interface IQueuePublisherAction
    {
        /// <summary>
        /// Добавить список объектов в конец очереди
        /// </summary>
        /// <param name="objectLst">Список объектов очереди</param>
        /// <returns>true - добавление прошло успешно, false - добавить объект в очередь не удалось</returns>
        bool EnqueueList(List<QueueObjectAction> objectLst);

        /// <summary>
        /// Добавить объект в конец очереди
        /// </summary>
        /// <param name="objectLst">Объект очереди</param>
        /// <returns>true - добавление прошло успешно, false - добавить объект в очередь не удалось</returns>
        bool EnqueueObject(QueueObjectAction obj);     
    }
}