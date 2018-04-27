using System;
using RetroToFileExporter.Core.Interfaces;
using RetroToFileExporter.Core.Models;
using RSDU;
using RSDU.INP.RSDU.Common;
using RSDU.INP.Settings;
using RSDU.INP.Settings.Interfaces;

namespace RetroToFileExporter.Core.Config
{
    public class ConfigSQLiteDB
    {
        class RsduDBSettings : IRsduDBSettings
        {
            private ISettings _settings = null;
            private String _dsn_def;
            private String _login_def;
            private String _pswd_def;
            private String _app_def;

            public RsduDBSettings(ISettings settings, String DSNDef, String LoginDef, String PswdDef, String AppNameDef)
            {
                _settings = settings;
                _dsn_def = DSNDef;
                _login_def = LoginDef;
                _pswd_def = PswdDef;
                _app_def = AppNameDef;
            }

            public string DSN { get { return _settings.GetSettingByDefineAlias(_dsn_def).Value; } }
            public string Login { get { return _settings.GetSettingByDefineAlias(_login_def).Value; } }
            public string Password { get { return _settings.GetSettingByDefineAlias(_pswd_def).Value; } }
            public string AppName { get { return _settings.GetSettingByDefineAlias(_app_def).Value; } }
        }
        
        private const string _config_db_file = @"db\config.db";
        private IRsduDBSettings _local_rsdu_db;
        private IRsduDBSettings _remote_rsdu_db;

        public IRsduDBSettings LocalRsduDBSettings { get { return _local_rsdu_db; } }
        public IRsduDBSettings RemoteRsduDBSettings { get { return _remote_rsdu_db; } }

        /// <summary>
        /// Объект блокировки доступа к БД SQLite
        /// Используется для блокировки доступа с БД из нескольких потоков
        /// </summary>
        private Object _lockConfDB = new Object();

        private ISettings _settings = null;
        public ISettings Settings { get { return _settings; } }
        private IActionSettings _actionSettings = null;
        private IScheduleSettings _scheduleSettings = null;
        public IActionSettings ActionSettings { get { return _actionSettings; } }
        public IScheduleSettings ScheduleSettings { get { return _scheduleSettings; } }


        public ConfigSQLiteDB(string app_path)
        {
            _settings = new SettingStorage(app_path + "\\" + _config_db_file, _lockConfDB);
            _actionSettings = new ActionsStorage(app_path + "\\" + _config_db_file, _lockConfDB);
            _scheduleSettings = new ScheduleStorage(app_path + "\\" + _config_db_file, _lockConfDB);

            _local_rsdu_db = new RsduDBSettings(_settings,
                    ConfigParamDefine.LOCAL_RSDU_DB_DSN,
                    ConfigParamDefine.LOCAL_RSDU_DB_LOGIN,
                    ConfigParamDefine.LOCAL_RSDU_DB_PASSWORD,
                    ConfigParamDefine.LOCAL_RSDU_DB_APP);
        } 
    }
}