using System;
using System.Collections.Generic;
using RetroToFileExporter.Core.Interfaces;

namespace RetroToFileExporter.Core.Models
{
    public class Schedule : ISchedule
    {
        IActionSettings actionSettings;

        public Schedule(Guid guid, string nameSchedule, string scheduleQuartz, bool isEnabledShedule, List<Action> actions)
        {
            GuidSchedule = guid;
            NameSchedule = nameSchedule;
            ScheduleQuartz = scheduleQuartz;
            IsEnabledShedule = isEnabledShedule;
            Actions = actions;
        }

        public Schedule(Guid guid, string nameSchedule, string scheduleQuartz, bool isEnabledShedule)
        {
            GuidSchedule = guid;
            NameSchedule = nameSchedule;
            ScheduleQuartz = scheduleQuartz;
            IsEnabledShedule = isEnabledShedule;
        }

        public Guid GuidSchedule { get; set; }
        public string NameSchedule { get; set; }
        public string ScheduleQuartz { get; set; }
        public bool IsEnabledShedule { get; set; }
        public List<Action> Actions { get; set; }
    }
}