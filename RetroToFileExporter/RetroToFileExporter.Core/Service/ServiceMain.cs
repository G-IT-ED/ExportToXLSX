using RSDU.DataRegistry;
using RSDU.INP.Windows.Service;
using System;
using RSDU.Messaging;
using System.Reflection;
using System.IO;
using RetroToFileExporter.Core.Interfaces;
using RetroToFileExporter.Core.Models;
using RSDU.INP.RSDU.Signal;
using RSDU.INP.RSDU.Common;

namespace RetroToFileExporter.Core.Service
{
    public class ServiceMain : WinServiceMainBase
    {
        private ReceiveRetroKadrThreadEx _rcvRetroThread = null;
        private IRsduDBSettings _localRsdudbConn = null;
        private IReceiveRetroThreadVariables _receiveRetroThVars = null;
        private IQueuePublisherSchedule _queuePublisherSchedule = null;
        private RsduSignalSystem _svcSignal = null;
        private IQueueConsumerSchedule _queueConsumerSchedule = null;
        private IQuartzSchedule _quartzSchedule = null;
        private IQueueConsumerAction _queueConsumerAction = null;
        private IQueuePublisherAction _queuePublisherAction = null;

        public ServiceMain()
        {
            ServiceName = "Сервис автоматической выгрузки кадров ретроспективы";
            ServiceInitialize();
        }

        ConfigStorage _configStorage;

        private void ServiceInitialize()
        {
            String appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            _configStorage = new ConfigStorage(appPath);
            WorkStorage workStorage = new WorkStorage(appPath);

            _queuePublisherSchedule = workStorage.QueuePublisherSchedule;
            _queueConsumerSchedule = workStorage.QueueConsumerSchedule;

            _queueConsumerSchedule.ScheduleStorage = new ScheduleStorage(appPath + "\\" + @"db\config.db");

            _queuePublisherAction = workStorage.QueuePublisherAction;
            _queueConsumerAction = workStorage.QueueConsumerAction;

            _localRsdudbConn = _configStorage;
            _receiveRetroThVars = workStorage.ReceiveRetroThreadVariables;
            _svcSignal = new RsduSignalSystem();    
        }

        protected override bool OnStart()
        {
            _quartzSchedule = new QuartzSchedule(_configStorage, _queuePublisherSchedule);
            try
            {
                DataRegistry registry = Registry.GetNewRegistry(_localRsdudbConn.DSN, _localRsdudbConn.Login, _localRsdudbConn.Password);
                registry.SetAppName(_localRsdudbConn.AppName);
                _svcSignal.Initialize(registry);
            }
            catch (Exception ex)
            {
                Log.Error(String.Format("Возникли ошибки при инициализации сигнальной системы! Отправка сигналов производиться не будет! \n\rОшибка: {0}\n{1}", ex.Message, ex.StackTrace));
            }
            try
            {
                _rcvRetroThread = new ReceiveRetroKadrThreadEx(_localRsdudbConn, _receiveRetroThVars, _queuePublisherAction, _queueConsumerAction, _queuePublisherSchedule, _queueConsumerSchedule, _configStorage, _svcSignal);
                _rcvRetroThread.Start();
                _quartzSchedule.StartThread();

                _svcSignal.LoadAndInitSignal();

                return true;
            }
            catch (Exception ex)
            {
                Log.Error(String.Format("Возникли ошибки при старте сервиса! \n\rОшибка: {0}\n{1}", ex.Message, ex.StackTrace));
            }
            return false;
        }

        protected override void OnStop()
        {
            _rcvRetroThread.Stop();
            _quartzSchedule.StopThread();
            _svcSignal.StopSignal();
        }
    }
}