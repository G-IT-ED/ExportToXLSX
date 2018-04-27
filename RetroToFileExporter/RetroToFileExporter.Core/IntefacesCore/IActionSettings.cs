using System;
using System.Collections.Generic;
using RetroToFileExporter.Core.Models;
using Action = RetroToFileExporter.Core.Models.Action;

namespace RetroToFileExporter.Core.Interfaces
{
    public interface IActionSettings
    {
        /// <summary>
        /// Метод получения триггера по GUID
        /// </summary>
        /// <param name="guid">Идентифицирующий GUID</param>
        Action GetTriggerByGuid(Guid guid);

        /// <summary>
        /// Метод обновления триггера
        /// </summary>
        /// <param name="action">Триггер</param>
        void UpdateAction(Action action);

        /// <summary>
        /// Метод получение всех триггеров
        /// </summary>
        List<Action> GetAllActions();

        /// <summary>
        /// Метод добавления триггера
        /// </summary>
        /// <param name="action"></param>
        void AddTrigger(Action action);

        /// <summary>
        /// Удалить триггер
        /// </summary>
        /// <param name="action"></param>
        void DeleteTrigger(Action action);
    }
}