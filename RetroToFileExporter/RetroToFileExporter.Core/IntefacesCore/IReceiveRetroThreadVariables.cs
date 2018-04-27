using RSDU;

namespace RetroToFileExporter.Core.Interfaces
{
    public interface IReceiveRetroThreadVariables
    {
        RsduTime LastTimeReceive { get; set; }
    }
}