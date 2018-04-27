using System;
using System.Collections.Generic;
using System.Threading;
using RetroToFileExporter.Core.Interfaces;
using RSDU.Messaging;
using Quartz;
using Quartz.Impl;
using RSDU;
using ITrigger = Quartz.ITrigger;

namespace RetroToFileExporter.Core.Models
{
    class QuartzSchedule : IQuartzSchedule
    {
        private static IQueuePublisherSchedule _queue;
        private IScheduler _shed;

        private const String NameTh = "QuartzScheduleThread";

        /// <summary>
        /// Объект потока выполнения.
        /// Главная функция потока - Run
        /// </summary>
        private Thread _thread;

        Dictionary<IJobDetail,ITrigger> _jobsDictionary = new Dictionary<IJobDetail, ITrigger>();
        
        private static void QueueAddScheduleActions(string guid)
        {
            try
            {
                _queue.EnqueueObject(new QueueObjectSchedule(guid, RsduTime.ToTime1970(DateTime.Now)));
                Log.Info(NameTh + "> Добавлена задача Quartz Guid: " + guid);
            }
            catch (Exception ex)
            {
                Log.Error("Не удалось добавить задачу Quartz Guid:" + guid, ex);
            }
        }
        
        public class JobToExport : IInterruptableJob
        {
            private bool _interrupted;

            public void Execute(IJobExecutionContext context)
            {
                QueueAddScheduleActions(context.Trigger.JobKey.Name);
            }
            
            public void Interrupt()
            {
                _interrupted = true;
            }
        }

        public QuartzSchedule(ConfigStorage configStorage, IQueuePublisherSchedule queue)
        {
            _queue = queue;
            ConfigSchedule(configStorage);
        }

        private void ConfigSchedule(ConfigStorage configStorage)
        {
            var schedules = configStorage.AllSchedules;
            if (schedules == null) return;

            foreach (var schedule in schedules)
            {
                try
                {
                    if (!schedule.IsEnabledShedule) continue;

                    //Что выполняем
                    var job = JobBuilder.Create<JobToExport>()
                        .WithIdentity(schedule.GuidSchedule.ToString(), schedule.GuidSchedule.ToString())
                        .Build();
                    //Когда выполняем
                    var trigger = TriggerBuilder.Create()
                        .WithIdentity("Trigger", schedule.GuidSchedule.ToString())
                        .WithCronSchedule(schedule.ScheduleQuartz, x => x.WithMisfireHandlingInstructionDoNothing())
                        .ForJob(schedule.GuidSchedule.ToString(), schedule.GuidSchedule.ToString())
                        .Build();
                    _jobsDictionary.Add(job, trigger);
                }
                catch (Exception e)
                {
                    Log.Error("Не верно записано выражение CRON:" + schedule.GuidSchedule, e);
                }
            }
        }

        public void StartThread()
        {
            Log.Info(NameTh + "> Запуск процесса Quartz");

            _thread = new Thread(Run);
            _thread.Start();
            //TODO: подписываемся на изменение файла config.db и обновляем триггеры, чтобы подтянуть новые во время работы Quartz
        }

        private void Run()
        {
            // -----------------------------------------
            // !!! Выполняется в отдельном потоке

            try
            {
                Log.Info(NameTh + "> ====== Процесс Quartz запущен! =====");

                var shedFactory = new StdSchedulerFactory();
                _shed = shedFactory.GetScheduler();

                _shed.Start();

                foreach (var job in _jobsDictionary)
                {
                    _shed.ScheduleJob(job.Key, job.Value);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Не удалось запустить процесс Quartz", ex);
            }
            
        }

        public void StopThread()
        {
            try
            {
                Log.Info(NameTh + "> ====== Остановка процесса Quartz ===");

                _shed.Clear();
                foreach (var trigger in _jobsDictionary)
                {
                    _shed.Interrupt(trigger.Key.Key);
                }
                _shed.Shutdown();
                _thread.Abort();
            }
            catch (Exception ex)
            {
                Log.Error("Не удалось остановить процесс Quartz", ex);
            }
            
        }
    }
}