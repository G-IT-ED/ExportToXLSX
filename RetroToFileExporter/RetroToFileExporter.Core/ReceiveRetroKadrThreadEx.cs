using RetroToFileExporter.Core.Interfaces;
using RSDU.INP.RSDU.Common;
using RSDU.INP.RSDU.Signal;

namespace RetroToFileExporter.Core
{
    public class ReceiveRetroKadrThreadEx : ReceiveRetroKadrThread
    {
        private IQueuePublisherAction _queuePublisherAction;
        private readonly IQueuePublisherSchedule _queuePublisherSchedule;

        public ReceiveRetroKadrThreadEx(IRsduDBSettings localRsdudbConf, IReceiveRetroThreadVariables _receiveRetroTh_vars, IQueuePublisherAction queuePublisherAction, IQueueConsumerAction queueActionConsumerAction, IQueuePublisherSchedule queuePublisherSchedule, IQueueConsumerSchedule queueConsumerSchedule, ConfigStorage configStorage, RsduSignalSystem svcSignal)
            : base(localRsdudbConf, _receiveRetroTh_vars, queueActionConsumerAction, queueConsumerSchedule, queuePublisherAction, configStorage, svcSignal)
        {
            _queuePublisherAction = queuePublisherAction;
            _queuePublisherSchedule = queuePublisherSchedule;
        }
    }
}