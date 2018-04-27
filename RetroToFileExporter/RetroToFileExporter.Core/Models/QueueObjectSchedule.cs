using RSDU;

namespace RetroToFileExporter.Core.Models
{
    public class QueueObjectSchedule
    {
        public string Guid { get; set; }
        public RsduTime Date { get; set; }

        public QueueObjectSchedule(string guid, RsduTime date)
        {
            Guid = guid;
            Date = date;
        }
    }
}