namespace RetroToFileExporter.Core.Interfaces
{
    public interface IQuartzSchedule
    {
        void StartThread();
        void StopThread();
    }
}