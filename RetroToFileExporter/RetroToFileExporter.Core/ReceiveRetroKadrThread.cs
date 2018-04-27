using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Timers;
using RetroToFileExporter.Core.Interfaces;
using RetroToFileExporter.Core.Models;
using RetroToFileExporter.Core.Objects;
using RSDU;
using RSDU.Database.Mappers.Helpers;
using RSDU.DataRegistry;
using RSDU.DataRegistry.Identity;
using RSDU.Domain;
using RSDU.Domain.Interfaces;
using RSDU.Domain.ProxyObjects;
using RSDU.INP.RSDU.Common;
using RSDU.INP.RSDU.Signal;
using RSDU.Messaging;

namespace RetroToFileExporter.Core
{
    public class ReceiveRetroKadrThread
    {
        private const String NameTh = "ReceiveRetroThread";

        /// <summary>
        /// ������ ������������ ������� ����� ������
        /// </summary>
        private System.Timers.Timer _timerThread = new System.Timers.Timer();

        /// <summary>
        /// ������ ������������ �������
        /// </summary>
        private System.Timers.Timer _timerToRecieve = new System.Timers.Timer();

        /// <summary>
        /// �������� ������� ��� ������� � ������� (������, ���������) � ������ �������
        /// </summary>
        ConcurrentDictionary<Guid, int> _listActionsRunning = new ConcurrentDictionary<Guid, int>();
        
        /// <summary>
        /// ����-��� (���) ���������� ������� ����������� � ����
        /// ����������� 60 ���.
        /// </summary>
        private int _sleepDbReConnect = 60;

        /// <summary>
        /// ��������� ����������� � �� ����
        /// </summary>
        private IRsduDBSettings _rsduDbConf;

        /// <summary>
        /// ���������������� ���������, ���������� ��� ������ ������
        /// </summary>
        private IReceiveRetroThreadVariables _settings;

        private IQueueConsumerAction _queueAction;
        private IQueueConsumerSchedule _queueSchedule;
        private IQueuePublisherAction _queuePublisherAction;
        private readonly ConfigStorage _configStorage;
        private readonly RsduSignalSystem _svcSignal;

        /// <summary>
        /// �������, ������������ ������� ��������� ��������.
        /// ��������� ����� ������� ���������� ��� ������ ������� Stop.
        /// </summary>
        private ManualResetEvent _evStop = new ManualResetEvent(false);

        /// <summary>
        /// �������, ������������ ������� ��������� ��������.
        /// ��������� ����� ������� ���������� ��� ������ ������� Stop.
        /// </summary>
        private ManualResetEvent _evStopSchedule = new ManualResetEvent(false);

        /// <summary>
        /// �������, ������������ ������������� ��������� ������ ������� �����
        /// </summary>
        private AutoResetEvent _evGetSchedule = new AutoResetEvent(true);

        /// <summary>
        /// �������, ������������ ����������� ��������� ��������.
        /// ��������� ����� ������� ���������� ��� ���������� ������� ������ RunSchedule.
        /// �������� ����� ������� ������������ � ������� Stop.
        /// </summary>
        private ManualResetEvent _evStopedSchedule = new ManualResetEvent(false);
        
        /// <summary>
        /// �������, ������������ ����������� ��������� ��������.
        /// ��������� ����� ������� ���������� ��� ���������� ������� ������ RunAction.
        /// �������� ����� ������� ������������ � ������� Stop.
        /// </summary>
        private ManualResetEvent _evStoped = new ManualResetEvent(false);

        /// <summary>
        /// ������ ������ ���������� �������� Excel.
        /// ������� ������� ������ - RunAction
        /// </summary>
        private Thread _threadAction;

        /// <summary>
        /// ������ ������ ���������� ���������� ������� �����.
        /// ������� ������� ������ - RunAction
        /// </summary>
        private Thread _threadSchedule;
        
        /// <summary>
        /// ������� ������ ���������� � ���� ����
        /// </summary>
        private AutoResetEvent _evDbConnLost = new AutoResetEvent(true);

        /// <summary>
        /// �������, ������������ ������������� ��������� ��������� ������ �������������.
        /// </summary>
        private AutoResetEvent _evGetRetro = new AutoResetEvent(true);
        
        /// <summary>
        /// ������������� ������� ����� N �����������
        /// </summary>
        private static readonly double _timeToReceive = 3000;

        /// <summary>
        /// ����������� ����� ����� ������, ��� ����������������� ������� ��� �������� ���������
        /// </summary>
        private static readonly double _minTimeToReceive = 60000;

        /// <summary>
        /// ���������� ����� ����� ���� ����� ������ ���������� error ������� �������
        /// </summary>
        private static readonly int _minuteToReceiveError = 2;

        /// <summary>
        /// �������� ������������ ���������� �������
        /// </summary>
        private MultyThreadQueue _threadQueue;

        public void Start()
        {
            try
            {
                Log.Info(NameTh + "> ������ �������� ��������� ������ ������������� �� ����...");

                if (_configStorage.TimeOutExport < _minTimeToReceive) Log.Error("������� ��������� ����� ���������� ������!");

                _timerThread.Interval = _configStorage.TimeOutExport;
                _timerThread.Elapsed += OnTimeOutExport;

                _timerToRecieve.Interval = _timeToReceive;
                _timerToRecieve.Elapsed += OnTimerReceive;
                _timerToRecieve.Start();

                _queueSchedule.OnQueueChanged += OnQueueScheduleChanged;
                _queueAction.OnQueueChanged += OnQueueActionChanged;

                _evStop.Reset();
                _evStoped.Reset();

                _threadAction = new Thread(RunAction);
                _threadAction.Start();

                _evStopSchedule.Reset();
                _evStopedSchedule.Reset();

                _threadSchedule = new Thread(RunSchedule);
                _threadSchedule.Start();

                _evGetRetro.Set();
            }
            catch (Exception ex)
            {
                _svcSignal.InitErrorSignal();
                Log.Error("�� ������� ��������� ����� ��������� ������ ������������� �� ����", ex);
            }
        }

        /// <summary>
        /// ������������ �������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTimerReceive(object sender, ElapsedEventArgs e)
        {
            _evGetRetro.Set();
            _evGetSchedule.Set();
        }

        /// <summary>
        /// ��������� ������� ����� ������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTimeOutExport(object sender, ElapsedEventArgs e)
        {
            _threadAction.Join();

            _threadAction = new Thread(RunAction);
            _threadAction.Start();

            _evStopSchedule.Reset();
            _evStopedSchedule.Reset();
        }

        /// <summary>
        /// �������� �������� ������ �����
        /// </summary>
        /// <param name="dateStart"></param>
        /// <param name="dateFinish"></param>
        /// <param name="kadr"></param>
        /// <param name="dataRegistry"></param>
        /// <returns></returns>
        private List<ParamData> GetDataArc(DateTime dateStart, DateTime dateFinish, RetroKadr kadr, DataRegistry dataRegistry)
        {
            var result = new List<ParamData>();
            lock (dataRegistry.Locker)
            {
                using (dataRegistry.ConnHolder)
                {
                    foreach (IArchive arc in kadr.Parameters)
                    {
                        result.Add(new ParamData(
                                       MeasureHelper.GetByTablePeriod(dataRegistry, arc.Retfname, new RsduTime(dateStart), new RsduTime(dateFinish)),
                                       arc));
                    }
                }
            }
            
            return result;
        }

        /// <summary>
        /// �������� ������� �� Quartz - ���������� ��������� �������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnQueueScheduleChanged(object sender, EventArgs e)
        {
            _evGetSchedule.Set();
        }

        /// <summary>
        /// �������� �������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnQueueActionChanged(object sender, EventArgs e)
        {
            _evGetRetro.Set();
        }
        
        public void Stop()
        {
            Log.Info(NameTh + "> ====== ��������� �������� ��������� ������ ������������� �� ����... ===");

            _queueSchedule.OnQueueChanged -= OnQueueScheduleChanged;
            _queueAction.OnQueueChanged -= OnQueueActionChanged;

            _evStop.Set();
            _evStoped.WaitOne();
            _evStoped.Reset();

            _evStopSchedule.Set();
            _evStopedSchedule.WaitOne();
            _evStopedSchedule.Reset();

            var threads = _threadQueue.GetAll();
            foreach (var thread in threads)
            {
                try
                {
                    thread.Abort();
                }
                catch { }
            }
        }

        public ReceiveRetroKadrThread(IRsduDBSettings localRsdudbConf, IReceiveRetroThreadVariables receiveRetroThVars, IQueueConsumerAction queueAction, IQueueConsumerSchedule queueSchedule, IQueuePublisherAction queuePublisherAction, ConfigStorage configStorage, RsduSignalSystem svcSignal)
        {
            _rsduDbConf = localRsdudbConf;
            _settings = receiveRetroThVars;
            _queueAction = queueAction;
            _queueSchedule = queueSchedule;
            _queuePublisherAction = queuePublisherAction;
            _configStorage = configStorage;
            _svcSignal = svcSignal;
            _threadQueue = new MultyThreadQueue(configStorage.CountConnection);
        }

        /// <summary>
        /// ������� ������ �������� ������� ������� ����� � ��������� ������
        /// </summary>
        private void RunSchedule()
        {
            Log.Info(NameTh + "> ====== ������� ������� ������� ����� �������! =====");

            // ������������ ������ ��������� �������
            WaitHandle[] events = new WaitHandle[] { _evStopSchedule, _evGetSchedule };

            bool isRun = true;
            while (isRun)
            {
                int signaledEvent = WaitHandle.WaitAny(events);
                switch (signaledEvent)
                {
                    case 0:
                        // (_evStopSchedule) �������� ������� �� ��������� ������ ---------------------------------------------------------------------                        
                        //Log.HasInfoSheet(NameTh + "> �������� ������� �� ��������� �������� ������� ������� �����...");
                        isRun = false;
                        break;
                        
                    case 1:
                        // (_evGetSchedule) �������� �������, � ���, ��� ��������� ������ ������� ����� ------------------------------
                        ReceiveScheduleActions();
                        break;
                }
            }

            _evStopSchedule.Reset();
            _evStopedSchedule.Set();

            Log.Info(NameTh + "> ====== ������� ������� ������� ����� ����������! =====");

        }

        /// <summary>
        /// ��������� ������� ����������
        /// </summary>
        private void ReceiveScheduleActions()
        {
            try
            {
                if (_queueSchedule.IsEmptyQueue()) return;
                var allActionOnSchedule = _queueSchedule.GetFirst();
                var schedule = _queueSchedule.GetScheduleByGuid(new Guid(allActionOnSchedule.Guid));
                foreach (var action in schedule.Actions)
                {
                    _queuePublisherAction.EnqueueObject(new QueueObjectAction(action.GuidAction, new RsduTime(DateTime.Now).Time1970, action,0));
                }
            }
            catch (Exception ex)
            {
                Log.Error("�� ������� �������� ���������� � �������", ex);
            }
        }
        
        /// <summary>
        /// ������� ������ �������� ��������� ������ ������������� 
        /// </summary>
        private void RunAction()
        {
            // ������� ������ �������� �������� ������ �������������
            // -----------------------------------------
            // !!! ����������� � ��������� ������

            Log.Info(NameTh + "> ====== ������� �������� ������ ������������� �������! =====");

            // ������������ ������ ��������� �������
            WaitHandle[] events = new WaitHandle[] {_evStop, _evDbConnLost, _evGetRetro};

            bool isRun = true;
            while (isRun)
            {
                int signaledEvent = WaitHandle.WaitAny(events);
                switch (signaledEvent)
                {
                    case 0:
                        // (_evStop) �������� ������� �� ��������� ������ ---------------------------------------------------------------------   
                        isRun = false;
                        break;

                    case 1:
                        // (_evDbConnLost) ��������� ������� ����������� ���������� -----------------------------------------------------------
                        DbConnectionRestorePropcess();
                        break;

                    case 2:
                        // (_evGetRetro) �������� �������, � ���, ��� ��������� ��������� ������ ��������� ������ �������������� ------------------------------
                        _threadQueue.Enqueue(new Thread(ReceiveRetroProcess));

                        Thread thread = _threadQueue.GetFirst();
                        if (thread != null)
                            thread.Start();
                        break;
                }
            }

            _evStop.Reset();
            _evStoped.Set();

            Log.Info(NameTh + "> ====== ������� �������� ������ ������������� ����������! =====");
        }


        private void DbConnectionRestorePropcess()
        {
#if DEBUG
            Log.Info("������� ��������� � ����");
#endif
            // ������������ ������ ��������� �������
            _evDbConnLost.Set();
            WaitHandle[] events = new WaitHandle[] { _evStop, _evDbConnLost };
            bool isRun = true;

            while (isRun)
            {
                int signaledEvent = WaitHandle.WaitAny(events, _sleepDbReConnect * 1000);
                switch (signaledEvent)
                {
                    case 0: // (_evStop) �������� ������� �� ��������� ������ ---------------------------------------------------------------------                        

                        isRun = false;
                        break;

                    case 1:
                    case WaitHandle.WaitTimeout:
                        try
                        {
                            var dataRegistry = Registry.GetNewRegistry(_rsduDbConf.DSN, _rsduDbConf.Login, _rsduDbConf.Password);
                            dataRegistry.SetAppName(_rsduDbConf.AppName);
                        }
                        catch (Exception e)
                        {
                            Log.Error(NameTh + "> ������!!! �������� ������ ��� ����������� � �� ����", e);
                            break;
                        }

                        isRun = false;
                        break;              
                }
            }
        }

        #region ��������� ���� 
        public DateTime GetDateStart(QueryCondition queryCondition, DateTime dt)
        {
            int direction = GetDirection(queryCondition.DirectionTime);
            DateTime start = StartPeriod(queryCondition.Interval, dt);
            
            if (queryCondition.DirectionTime == "������") return GetFutureDate(queryCondition, start);
            
            return GetBackDate(queryCondition, direction, start);
        }

        public DateTime GetDateFinish(string interval, int count, DateTime start)
        {
            switch (interval)
            {
                case "�����":
                    return start.AddDays(count);
                case "�����":
                    return start.AddMonths(count);
                case "���":
                    return start.AddYears(count);
            }
            throw new InvalidDataException();
        }

        private DateTime GetBackDate(QueryCondition queryCondition, int direction, DateTime start)
        {
            var result = new DateTime();
            switch (queryCondition.Interval)
            {
                case "�����":
                    result = start.AddDays(direction * queryCondition.IntervalCount);
                    result = result.AddDays(-direction);
                    result = result.AddDays(queryCondition.Offset);
                    break;
                case "�����":
                    result = start.AddMonths(direction * queryCondition.IntervalCount);
                    result = result.AddMonths(-direction);
                    result = result.AddMonths(queryCondition.Offset);
                    break;
                case "���":
                    result = start.AddYears(direction * queryCondition.IntervalCount);
                    result = result.AddYears(-direction);
                    result = result.AddYears(queryCondition.Offset);
                    break;
            }
            return result;
        }

        private DateTime GetFutureDate(QueryCondition queryCondition, DateTime start)
        {
            var result = new DateTime();
            switch (queryCondition.Interval)
            {
                case "�����":
                    result = start.AddDays(queryCondition.Offset);
                    break;
                case "�����":
                    result = start.AddMonths(queryCondition.Offset);
                    break;
                case "���":
                    result = start.AddYears(queryCondition.Offset);
                    break;
            }
            return result;
        }

        private int GetDirection(string directionTime)
        {
            switch (directionTime)
            {
                case "������":
                    return 1;
                case "�����":
                    return -1;
            }
            throw new InvalidDataException();
        }

        private DateTime StartPeriod(string interval, DateTime dt)
        {
            switch (interval)
            {
                case "�����":
                    return dt.Date;
                case "�����":
                    return dt.Date.AddDays(-(dt.Day - 1));
                case "���":
                    return dt.Date.AddDays(-(dt.Date.DayOfYear - 1));
            }
            throw new InvalidDataException();
        }
#endregion 

        
        /// <summary>
        /// � ������� ��������� ������ - ���������.
        /// ����� ������ ����������� � ��������� ������ (new Thread(ReceiveRetroProcess)) - �� �������� ���!
        /// </summary>
        private void ReceiveRetroProcess()
        {
            QueueObjectAction objectAction = null;

            try
            {
                if (_queueAction == null || _queueAction.IsEmptyQueue()) return;
#if DEBUG
                Log.Info("�� ������� ��������� ������� ");
#endif

                objectAction = _queueAction.GetFirst();

                // ������������ ������ ����� ��������
                if (objectAction == null || _listActionsRunning.ContainsKey(objectAction.Guid))
                    return;

                if (!IsReceivedObject(objectAction))
                {
                    Log.Info("���������� �������� �������� " + objectAction.Action.GuidAction);
                    return;
                }

                _listActionsRunning[objectAction.Guid] = 0;

                DateTime dtNow = DateTime.Now;

                // ������� ����� ���������� �������� ���� ������ ��������������� ���������
                if (objectAction.Action.RunTimeOffset > 0)
                {
                    Log.Info("���������� �������� �������� " + objectAction.Action.GuidAction);
                    Thread.Sleep(TimeSpan.FromSeconds(objectAction.Action.RunTimeOffset));
                    Log.Info("���������� �������� ������������ " + objectAction.Action.GuidAction);
                }

                var dataRegistry = Registry.GetNewRegistry(_rsduDbConf.DSN, _rsduDbConf.Login, _rsduDbConf.Password);
                dataRegistry.SetAppName(_rsduDbConf.AppName);

                lock (dataRegistry.Locker)
                {
                    Log.Info("���������� ����� � ������� �� ��������� " + _queueAction.Count());
                    _timerThread.Start();
                    var archives = new List<ParamData>();
                    
                    // ��������� ����� �� ��
                    RetroKadr kadr = GetKadr(objectAction.Action.KadrId, dataRegistry);
                    if (kadr == null)
                    {
                        _timerThread.Stop();
                        RemoveActionRunning(objectAction);
                        return;
                    }
                    
                    var conditionsDates = new List<QueryConditionsDates>();
                    
                    // ������ ������� � ������ �������
                    Dictionary<DateTime, DateTime> startFinish = new Dictionary<DateTime, DateTime>();
                    foreach (var queryCondition in objectAction.Action.QueryConditions)
                    {
                        DateTime start = GetDateStart(queryCondition, dtNow);
                        DateTime finish = GetDateFinish(queryCondition.Interval, queryCondition.IntervalCount, start);

                        conditionsDates.Add(new QueryConditionsDates(queryCondition, start, finish));

                        List<ParamData> archive = GetDataArc(start, finish, kadr, dataRegistry);

                        if (archive != null) archives.AddRange(archive);

                        startFinish.Add(start, finish);
                    }
                    
                    IDataExporter dataExporter = new DataExporter(objectAction.Action, kadr, archives, conditionsDates);                    
                    Log.Info("����������� ������ " + objectAction.Guid);                    
                    bool exported = dataExporter.Export(startFinish, dtNow);
                    Log.Info("��������� ������ " + objectAction.Guid + " - " + exported);

                    if (exported)
                    {
                        _queueAction.DeleteRecord(objectAction);
                    }
                    else
                    {
                        _queueAction.DeleteRecord(objectAction);
                        objectAction.CountError = objectAction.CountError + 1;
                        _queueAction.EnqueueObject(objectAction);
                    }
                    _evGetRetro.Set();
                    _timerThread.Stop();
                    RemoveActionRunning(objectAction);
#if DEBUG
                    Log.Info("���������� ����� � ������� ����� ��������� " + _queueAction.Count());
#endif
                }
                dataRegistry = null;
            }
            catch (ThreadAbortException)
            {
                // ��� ��������� ������� ��� ������ ��������� � ������� Thread.Abort. 
                // ������������� ��������� ���������� "����� ��������� � �������� ��������" ��-�� ����, 
                // ��� ��������� ������ ��������� � ���������������� ��������� �.�. �� Thread.Sleep ������
                Thread.ResetAbort();
            }
            catch (Exception ex)
            {
                Log.Error("�� ������� ��������� ������ �� �������", ex);

                if (objectAction != null)
                    RemoveActionRunning(objectAction);
            }
        }

        /// <summary>
        /// ������� ����������� ��������
        /// </summary>
        /// <param name="objectAction"></param>
        private void RemoveActionRunning(QueueObjectAction objectAction)
        {
            int i;
            _listActionsRunning.TryRemove(objectAction.Guid, out i);
        }

        /// <summary>
        /// �������� ���������� ��������
        /// </summary>
        /// <param name="objectAction"></param>
        /// <returns></returns>
        public bool IsReceivedObject(QueueObjectAction objectAction)
        {
            TimeSpan timeLastError = DateTime.Now - objectAction.DateError;

            if (timeLastError.TotalMinutes < _minuteToReceiveError) return false;

            if (objectAction.CountError > _configStorage.CountError)
            {
                _svcSignal.SendSignal(SysSignal.Define.ServiceFault);
                Log.Error("��������� ���������� ������� ���������� ������" + objectAction.Guid);
                _queueAction.DeleteRecord(objectAction);
                return false;
            }
            return true;
        }

        /// <summary>
        /// ��������� ����� �� id
        /// </summary>
        /// <param name="kadrId"></param>
        /// <returns></returns>
        private RetroKadr GetKadr(int kadrId, DataRegistry _dataRegistry)
        {
            lock (_dataRegistry.Locker)
            {
                using (_dataRegistry.ConnHolder)
                {
                    _dataRegistry.Map.RemoveFromMap<RetroKadr>(new ObjectIdentity(kadrId));
                    var kadr = _dataRegistry.Map.Find<RetroKadr>(new ObjectIdentity(kadrId));
                    _dataRegistry.Map.FullLoad(kadr);

                    foreach (var parameter in kadr.Parameters)
                    {
                        _dataRegistry.Map.MiddleLoad(parameter.Ginfo);
                        if (parameter.Ginfo != null) _dataRegistry.Map.MiddleLoad(parameter.Ginfo.GTopt);

                        if (parameter.Measure == null) continue;

                        if (parameter.Measure is AutoMeasure)
                            _dataRegistry.Map.MiddleLoad((AutoMeasure) parameter.Measure);
                        if (parameter.Measure is ElRegDgMeasure)
                            _dataRegistry.Map.MiddleLoad((ElRegDgMeasure) parameter.Measure);
                        if (parameter.Measure is ElRegMeasure)
                            _dataRegistry.Map.MiddleLoad((ElRegMeasure) parameter.Measure);
                        if (parameter.Measure is PhRegMeasure)
                            _dataRegistry.Map.MiddleLoad((PhRegMeasure) parameter.Measure);
                        if (parameter.Measure is PswtMeasure)
                            _dataRegistry.Map.MiddleLoad((PswtMeasure) parameter.Measure);
                        if (parameter.Measure is CalcMeasure)
                            _dataRegistry.Map.MiddleLoad((CalcMeasure) parameter.Measure);
                        if (parameter.Measure is DaMeasure)
                            _dataRegistry.Map.MiddleLoad((DaMeasure) parameter.Measure);
                        if (parameter.Measure is DgMeasure)
                            _dataRegistry.Map.MiddleLoad((DgMeasure) parameter.Measure);
                        if (parameter.Measure is EaMeasure)
                            _dataRegistry.Map.MiddleLoad((EaMeasure) parameter.Measure);
                        if (parameter.Measure is StateMeasure)
                            _dataRegistry.Map.MiddleLoad((StateMeasure) parameter.Measure);

                        if (parameter.Measure == null) continue;

                        if (parameter.Measure.Node is AstOrganization)
                            _dataRegistry.Map.MiddleLoad((AstOrganization) parameter.Measure.Node);
                        if (parameter.Measure.Node is DgPlanGroup)
                            _dataRegistry.Map.MiddleLoad((DgPlanGroup) parameter.Measure.Node);
                        if (parameter.Measure.Node is EaGroup)
                            _dataRegistry.Map.MiddleLoad((EaGroup) parameter.Measure.Node);
                        if (parameter.Measure.Node is Equipment)
                            _dataRegistry.Map.MiddleLoad((Equipment) parameter.Measure.Node);
                        if (parameter.Measure.Node is HgGroup)
                            _dataRegistry.Map.MiddleLoad((HgGroup) parameter.Measure.Node);
                        if (parameter.Measure.Node is SborEquipment)
                            _dataRegistry.Map.MiddleLoad((SborEquipment) parameter.Measure.Node);
                    }
                    return kadr;
                }
            }
        }
    }

    /// <summary>
    /// ����� ��� �������� ������� ������������� �������
    /// </summary>
    public class QueryConditionsDates
    {
        public QueryConditionsDates(QueryCondition queryCondition, DateTime start, DateTime finish)
        {
            QueryCondition = queryCondition;
            Start = start;
            Finish = finish;
        }

        public override string ToString()
        {
            return String.Format("Period: [{0}, {1}]",
                ( Start != null? Start.ToString(): "not set"),
                (Finish != null ? Finish.ToString() : "not set"));
        }

        public QueryCondition QueryCondition { get; set; }
        public DateTime Start { get; set; }
        public DateTime Finish { get; set; }
    }
}
