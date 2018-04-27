using System;

namespace RetroToFileExporter.Core.Objects
{
    /// <summary>
    /// Аргументы события изменения архива у параметра
    /// </summary>
    public class ArchiveChangedEventArgs : EventArgs
    {
        /// <summary>
        /// старое значение архива
        /// </summary>
        private int _oldIdGinfo;

        /// <summary>
        /// старое значение архива
        /// </summary>
        public int OldIdGinfo
        {
            get { return _oldIdGinfo; }
            set { _oldIdGinfo = value; }
        }
    }
}
