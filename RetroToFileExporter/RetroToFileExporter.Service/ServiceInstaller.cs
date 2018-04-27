using RSDU.INP.Windows.Service;
using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.Reflection;

namespace RetroToFileExporter.Service
{
    [RunInstaller(true)]
    public partial class ServiceInstaller : Installer
    {        
        public ServiceInstaller()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Функция, вызывающаяся при инсталляции сервиса
        /// </summary>
        /// <param name="savedState"></param>
        public override void Install(IDictionary savedState)
        {
            LoadServiceDefinition();
            base.Install(savedState);
        }

        /// <summary>
        /// Функция, вызывающаяся при деинсталляции сервиса
        /// </summary>
        /// <param name="mySavedState"></param>
        public override void Uninstall(IDictionary mySavedState)
        {
            LoadServiceDefinition();
            base.Uninstall(mySavedState);
        }

        private void LoadServiceDefinition()
        {
            String app_path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            ServiceDefinition.SetServiceInstaller(app_path, this);
        }
    }
}
