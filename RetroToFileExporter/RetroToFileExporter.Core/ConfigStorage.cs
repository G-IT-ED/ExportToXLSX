using System;
using System.Collections.Generic;
using RetroToFileExporter.Core.Defines;
using RetroToFileExporter.Core.Models;
using RSDU.INP.Settings;
using RSDU.INP.Settings.Interfaces;
using RSDU.INP.RSDU.Common;
using Action = RetroToFileExporter.Core.Models.Action;

namespace RetroToFileExporter.Core
{
    public class ConfigStorage : IRsduDBSettings
    {
        private Object _lockConfDb = new Object();
        private const string ConfigDbFile = @"db\config.db";
        private SettingStorage _settings = null;
        private ActionsStorage _settingsAction;
        private ScheduleStorage _settingsSchedule;

        public ISettings Settings { get { return _settings; } }

        public ConfigStorage(string appPath)
        {
            _settings = new SettingStorage(appPath + "\\" + ConfigDbFile, _lockConfDb);
            _settingsAction = new ActionsStorage(appPath + "\\" + ConfigDbFile, _lockConfDb);
            _settingsSchedule = new ScheduleStorage(appPath + "\\" + ConfigDbFile, _lockConfDb);
        }

        #region Interface IRsduDBSettings
        public string DSN { get { return _settings.GetSettingByDefineAlias(SettingDefines.RSDU_DB_DSN).Value; } }
        public string Login { get { return _settings.GetSettingByDefineAlias(SettingDefines.RSDU_DB_LOGIN).Value; } }
        public string Password { get { return _settings.GetSettingByDefineAlias(SettingDefines.RSDU_DB_PASSWORD).Value; } }
        public string AppName { get { return _settings.GetSettingByDefineAlias(SettingDefines.RSDU_DB_APP).Value; } }
        #endregion 

        public List<Action> AllActions {get { return _settingsAction.GetAllActions(); }}
        public List<Schedule> AllSchedules{get { return _settingsSchedule.GetAllSchedule(); }}

        public int TimeOutExport{get{return Convert.ToInt32(_settings.GetSettingByDefineAlias(SettingDefines.MAX_TIME_EXPORT).Value);}}
        public int CountConnection { get { return Convert.ToInt32(_settings.GetSettingByDefineAlias(SettingDefines.MAX_COUNT_CONNECTION).Value); } }
        public int CountError { get { return Convert.ToInt32(_settings.GetSettingByDefineAlias(SettingDefines.MAX_COUNT_ERROR).Value); } }
    }

}