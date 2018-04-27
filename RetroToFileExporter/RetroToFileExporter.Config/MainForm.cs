using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using RetroToFileExporter.Config.Controls;
using RetroToFileExporter.Core.Config;
using RetroToFileExporter.Core.Interfaces;
using RSDU.INP.Settings.GUI;
using RSDU.INP.Settings.Interfaces;
using RSDU.INP.WorkVariables.GUI;
using RSDU.INP.WorkVariables.Interfaces;

namespace RetroToFileExporter.Config
{
    public partial class MainForm : Form
    {
        private ISettings _settings;
        private IActionSettings _actionSettings;
        private IScheduleSettings _scheduleSettings;
        private SchedulerControl schedulerControl;

        public MainForm()
        {
            String appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            ConfigSQLiteDB configDb = new ConfigSQLiteDB(appPath);
            _settings = configDb.Settings;
            _actionSettings = configDb.ActionSettings;
            _scheduleSettings = configDb.ScheduleSettings;
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            CreateScheduleControl();
            CreateTriggerControl();
            CreateSettingControl();
        }

        private void CreateTriggerControl()
        {
            TriggerControl triggerControl = new TriggerControl(_actionSettings,_scheduleSettings);
            triggerControl.Parent = _trigger;
            triggerControl.Dock = DockStyle.Fill;
        }

        private void CreateScheduleControl()
        {
            schedulerControl = new SchedulerControl(_scheduleSettings, _actionSettings);
            schedulerControl.Parent = _schedule;
            schedulerControl.Dock = DockStyle.Fill;
        }

        private void CreateSettingControl()
        {
            SettingControl settingCtrl = new SettingControl(_settings);
            settingCtrl.Parent = _pageSettings;
            settingCtrl.Dock = DockStyle.Fill;
        }
        
        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPage == _schedule && schedulerControl != null)
                schedulerControl.SetData();
        }
    }
}
