using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RetroToFileExporter.Core.Service;
using RSDU.INP.Windows.Service;

namespace RetroToFileExporter.Launcher
{
    class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            IWinServiceMain svc = new ServiceMain();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new WinServiceDebugForm(svc));
        }
    }
}
