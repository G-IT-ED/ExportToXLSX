using System;
using RetroToFileExporter.Core.Defines;
using RetroToFileExporter.Core.Interfaces;
using RetroToFileExporter.Core.Models;
using RetroToFileExporter.Core.Objects;
using RSDU;
using RSDU.INP.WorkVariables;
using RSDU.INP.WorkVariables.Interfaces;
using RSDU.INP.WorkVariables.Model;

namespace RetroToFileExporter.Core
{
    public class WorkStorage
    {
        class ReceiveRetroThreadVars : IReceiveRetroThreadVariables
        {
            private IWorkVariables _vars = null;
            public ReceiveRetroThreadVars(IWorkVariables vars)
            {
                _vars = vars;
            }

            public RsduTime LastTimeReceive
            {
                get
                {
                    WorkVariable val = _vars.GetVariable(WorkVariableDefines.VAR_LASTDATE_RETRO);
                    long time1970 = Convert.ToInt64(val.Value);
                    if (time1970 <= 0)
                        time1970 = (new RsduTime(DateTime.Today)).Time1970;

                    return new RsduTime(time1970);
                }
                set
                {
                    WorkVariable val = _vars.GetVariable(WorkVariableDefines.VAR_LASTDATE_RETRO);
                    val.Value = value.Time1970.ToString();
                    _vars.UpdateVariable(val);
                }
            }
        }

        /// <summary>
        /// Путь к файлу рабочей БД SQLite
        /// </summary>
        private const string WorkDbFile = @"db\work.db";

        private IWorkVariables _vars = null;
        private QueueScheduleStorage _queue = null;
        private RetroQueueSqlLite _queueAction = null;

        /// <summary>
        /// Объект блокировки доступа к БД SQLite
        /// Используется для блокировки доступа с БД из нескольких потоков
        /// </summary>
        private Object _lockWorkDb = new Object();
        private ReceiveRetroThreadVars _receiveRetroThreadVars = null;

        public IWorkVariables WorkVars { get { return _vars; } }
        public IReceiveRetroThreadVariables ReceiveRetroThreadVariables { get { return _receiveRetroThreadVars; } }
        public IQueueAdminSchedule QueueAdminSchedule { get { return _queue; } }
        public IQueueConsumerSchedule QueueConsumerSchedule { get { return _queue; } }
        public IQueuePublisherSchedule QueuePublisherSchedule { get { return _queue; } }


        public IQueueAdminAction QueueAdminAction { get { return _queueAction; } }
        public IQueueConsumerAction QueueConsumerAction { get { return _queueAction; } }
        public IQueuePublisherAction QueuePublisherAction { get { return _queueAction; } }

        public WorkStorage(String appPath)
        {
            _vars = new VariableStorage(appPath + "\\" + WorkDbFile, _lockWorkDb);

            _receiveRetroThreadVars = new ReceiveRetroThreadVars(_vars);
            _queue = new QueueScheduleStorage();
            _queueAction = new RetroQueueSqlLite(appPath + "\\" + WorkDbFile, _lockWorkDb);
        }
    }
}