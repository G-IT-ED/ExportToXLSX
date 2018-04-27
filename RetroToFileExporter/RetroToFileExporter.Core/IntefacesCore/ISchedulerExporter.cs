namespace RetroToFileExporter.Core.Interfaces
{
    public interface ISchedulerExporter
    {
        bool IsStarted();
        void Clear();
        void ShutDown();
        void Start();
    }
}