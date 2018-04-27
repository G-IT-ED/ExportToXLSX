using System;
using System.Collections.Generic;
using DevExpress.XtraPrinting;
using RSDU.INP.Annotations;

namespace RetroToFileExporter.Core.Interfaces
{
    public interface IDataExporter
    {
        bool Export(Dictionary<DateTime, DateTime> startFinish, DateTime dt);
    }
}