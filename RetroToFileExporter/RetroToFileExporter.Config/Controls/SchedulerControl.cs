using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Quartz;
using Quartz.Impl;
using RetroToFileExporter.Core.Interfaces;
using RetroToFileExporter.Core.Models;
using RSDU.Messaging;
using Action = RetroToFileExporter.Core.Models.Action;

namespace RetroToFileExporter.Config.Controls
{
    public partial class SchedulerControl : UserControl
    {
        private IScheduleSettings _setting;
        private BindingList<Schedule> _bindingList = new BindingList<Schedule>();
        private Schedule _currentSchedule;
        private IActionSettings _actionSettings;
        BindingList<Action> _triggersControlsTriggers = new BindingList<Action>();

        public SchedulerControl(IScheduleSettings settings, IActionSettings actionSettings)
        {
            InitializeComponent();
            _setting = settings;
            _actionSettings = actionSettings;
            SetData();
        }

        public void SetData()
        {
            _bindingList.Clear();
            var allSchedule = _setting.GetAllSchedule();
            foreach (var schedule in allSchedule)
            {
                if (schedule.ScheduleQuartz.Length == 0)
                    schedule.ScheduleQuartz = "0 0 * * * ?";
                _bindingList.Add(schedule);
            }
            _scheduleGridControl.DataSource = _bindingList;
        }

        private void _gridView_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            _currentSchedule = null;

            var rowsHandle = _gridViewSchedule.GetSelectedRows();
            var bindingSource = new BindingSource() { DataSource = _gridViewSchedule.DataSource };
            foreach (var rowHandle in rowsHandle)
            {
                var index = _gridViewSchedule.GetDataSourceRowIndex(rowHandle);
                if (index >= 0 && bindingSource.Count >= index)
                    _currentSchedule = (Schedule)bindingSource[index];
            }
            if (_currentSchedule != null)
                SetSchedule();
        }

        private void SetSchedule()
        {
            if (_currentSchedule == null) return;
            SetEnabledControls(true);
            NameSchedule.Text = _currentSchedule.NameSchedule;
            ScheduleQuartz.Text = _currentSchedule.ScheduleQuartz;
            IsEnabledShedule.Checked = _currentSchedule.IsEnabledShedule;

            TriggerName.DataSource = null;
            BindingList<Action> bindingListTriggerAll = new BindingList<Action>();
            var triggers = _actionSettings.GetAllActions();
            if (triggers != null)
            {
                foreach (var trigger in triggers)
                {
                    bindingListTriggerAll.Add(trigger);
                    TriggerName.Text = trigger.ToString();
                }
            }
            TriggerName.DataSource = bindingListTriggerAll;

            _gridControlTriggers.DataSource = null;
            _triggersControlsTriggers = new BindingList<Action>();
            if (_currentSchedule.Actions == null) return;
            foreach (var trigger in _currentSchedule.Actions)
            {
                _triggersControlsTriggers.Add(trigger);
            }
            _gridControlTriggers.DataSource = _triggersControlsTriggers;
        }

        private void SetEnabledControls(bool b)
        {
            NameSchedule.Enabled = b;
            ScheduleQuartz.Enabled = b;
            IsEnabledShedule.Enabled = b;
            _gridControlTriggers.Enabled = b;
            TriggerName.Enabled = b;
            button1.Enabled = b;
            button2.Enabled = b;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _triggersControlsTriggers.Add((Action)TriggerName.SelectedItem);
            _gridControlTriggers.DataSource = _triggersControlsTriggers;
        }

        private void _toolBtnAddLink_Click(object sender, EventArgs e)
        {
            _bindingList.Add(new Schedule(Guid.NewGuid(),"расписание","0 0 * * * ?",false, new List<Action>()));
            if (_bindingList.Count>0)
                _gridViewSchedule.FocusedRowHandle = _bindingList.Count -1;
            _setting.AddSchedule(_bindingList.Last());
        }

        private void _toolBtnSaveLink_Click(object sender, EventArgs e)
        {
            if (_currentSchedule == null) return;
            UpdateBindingFromForm();
            _setting.UpdateSchedule(_currentSchedule);
            
        }

        private void UpdateBindingFromForm()
        {
            if(_currentSchedule == null) return;
            _currentSchedule.IsEnabledShedule = IsEnabledShedule.Checked;
            _currentSchedule.ScheduleQuartz = ScheduleQuartz.Text;
            _currentSchedule.NameSchedule = NameSchedule.Text;
            _currentSchedule.Actions.Clear();
            foreach (var trigger in _triggersControlsTriggers)
            {
                _currentSchedule.Actions.Add(trigger);
            }

            if (_gridViewSchedule.GetSelectedRows()==null)return;
            var rowsHandle = _gridViewSchedule.GetSelectedRows().First();

            if (_bindingList[_gridViewSchedule.GetDataSourceRowIndex(rowsHandle)].GuidSchedule == _currentSchedule.GuidSchedule)
                    _bindingList[_gridViewSchedule.GetDataSourceRowIndex(rowsHandle)] = _currentSchedule; //schedule = _currentSchedule;
            
        }

        private void _toolBtnUpdate_Click(object sender, EventArgs e)
        {
            SetData();
        }

        private void _toolBtnDelLink_Click(object sender, EventArgs e)
        {
            if (_currentSchedule == null) return;
            _setting.DeleteSchedule(_currentSchedule);
            SetData();
            if (_bindingList.Count == 0)
            {
                SetEnabledControls(false);
                NameSchedule.Text = "";
                ScheduleQuartz.Text = "";
                IsEnabledShedule.Checked = false;
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                var rowsHandle = _gridViewTriggers.GetSelectedRows();
                var bindingSource = new BindingSource() { DataSource = _gridViewTriggers.DataSource };
                foreach (var rowHandle in rowsHandle)
                {
                    var index = _gridViewTriggers.GetDataSourceRowIndex(rowHandle);
                    if (index >= 0 && bindingSource.Count >= index)
                        _triggersControlsTriggers.Remove((Action)bindingSource[index]);
                }
            }
            catch (Exception)
            {
                RsduMessageForm.ShowDialog("Не удалось удалить действие",this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        
        private void ScheduleQuartz_Leave(object sender, EventArgs e)
        {
            try
            {
                var job = JobBuilder.Create<JobToExport>()
                        .WithIdentity("group", "group")
                        .Build();
                var trigger = TriggerBuilder.Create()
                        .WithIdentity("Trigger", "group")
                        .WithCronSchedule(ScheduleQuartz.Text, x => x.WithMisfireHandlingInstructionDoNothing())
                        .ForJob("group", "group")
                        .Build();
                var shedFactory = new StdSchedulerFactory();
                var _shed = shedFactory.GetScheduler();
                _shed.Start();
                _shed.ScheduleJob(job, trigger);
                _shed.Shutdown();
                ScheduleQuartz.BackColor = Color.White;
            }
            catch (Exception)
            {
                ScheduleQuartz.BackColor = Color.Red;
            }
        }
    }

    internal class JobToExport : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            
        }
    }
}
