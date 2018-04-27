using System;
using System.Collections.Generic;
using RetroToFileExporter.Core.Models;
using Action = RetroToFileExporter.Core.Models.Action;

namespace RetroToFileExporter.Core.Interfaces
{
    /// <summary>
    /// Распиание
    /// </summary>
    public interface ISchedule
    {
        /// <summary>
        /// GuidQuery расписания
        /// </summary>
        Guid GuidSchedule { get; set; }

        /// <summary>
        /// Наименование расписания 
        /// </summary>
        string NameSchedule { get; set; }

        /// <summary>
        /// Расписание quartz для дальнейшего использования библиотекой
        /// </summary>
        string ScheduleQuartz { get; set; }

        /// <summary>
        /// Включено
        /// </summary>
        bool IsEnabledShedule { get; set; }

        /// <summary>
        /// Привязанные триггеры
        /// </summary>
        List<Action> Actions { get; set; }
    }
}