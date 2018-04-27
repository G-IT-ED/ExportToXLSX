using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using RetroToFileExporter.Core.Service;
using RSDU.INP.Windows.Service;
using RSDU.Messaging;

namespace RetroToFileExporter.Service
{
    public partial class Service : ServiceBase
    {
        private IWinServiceMain _svc;
        private TraceWriter _tracewriter;

        public Service()
        {
            _tracewriter = TraceWriter.Create(new TraceWriterSettings());
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                _svc = new ServiceMain();
                _svc.Start();
            }
            catch (Exception exс)
            {
                Journal.Write(exс.Source, "При старте сервиса произошла ошибка\r\n" + exс, EventLogEntryType.Error);
                Log.Write(exс);
            }
        }

        protected override void OnStop()
        {
            try
            {
                RequestAdditionalTime(5000);
                _svc.Stop();
            }
            catch (Exception exс)
            {
                Journal.Write(exс.Source, "При останове сервиса произошла ошибка\r\n" + exс, EventLogEntryType.Error);
                Log.Write(exс);
            }
        }
    }
}
