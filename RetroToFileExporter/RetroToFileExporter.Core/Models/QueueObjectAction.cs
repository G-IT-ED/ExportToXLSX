using System;
using RetroToFileExporter.Core.Interfaces;

namespace RetroToFileExporter.Core.Models
{
    public class QueueObjectAction
    {
        public QueueObjectAction(Guid guid, long dt1970, IAction action, int countError)
        {
            Guid = guid;
            Dt1970 = dt1970;
            Action = action;
            CountError = countError;
        }

        public QueueObjectAction()
        {
            
        }

        public Guid Guid { get; set; }
        public long Dt1970 { get; set; }
        public IAction Action { get; set; }
        public string Xml { get; set; }
        public int CountError { get; set; }
        public DateTime DateError { get; set; }
    }
}