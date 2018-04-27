using System;
using System.Collections.Generic;
using RetroToFileExporter.Core.Models;

namespace RetroToFileExporter.Core.Interfaces
{
    public interface IScheduleSettings
    {
        /// <summary>
        /// Метод получения расписания по GUID
        /// </summary>
        /// <param name="guid">Идентифицирующий GUID</param>
        /// <returns></returns>
        Schedule GetScheduleByGuid(Guid guid);

        /// <summary>
        /// Метод обновления расписания
        /// </summary>
        /// <param name="schedule">Расписание</param>
        void UpdateSchedule(Schedule schedule);

        /// <summary>
        /// Метод получения всех зарегистрированный расписаний
        /// </summary>
        /// <returns></returns>
        List<Schedule> GetAllSchedule();

        /// <summary>
        /// Метод добавления расписания
        /// </summary>
        /// <param name="schedule"> Расписание</param>
        void AddSchedule(Schedule schedule);

        /// <summary>
        /// Метод удаления расписания
        /// </summary>
        /// <param name="schedule">Расписание</param>
        void DeleteSchedule(Schedule schedule);
    }
}