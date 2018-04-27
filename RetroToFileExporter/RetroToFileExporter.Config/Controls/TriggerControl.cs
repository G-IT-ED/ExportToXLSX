using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using RetroToFileExporter.Core.Interfaces;
using RetroToFileExporter.Core.Models;
using RetroToFileExporter.Core.Objects;
using Action = RetroToFileExporter.Core.Models.Action;

namespace RetroToFileExporter.Config.Controls
{
    public partial class TriggerControl : UserControl
    {
        private IActionSettings _settings;
        private Action _currentAction;
        private BindingList<QueryCondition> _bindingConditions =new BindingList<QueryCondition>();
        private BindingList<Action> _bindingTriggers = new BindingList<Action>();
        private IScheduleSettings _scheduleSettings;

        public TriggerControl(IActionSettings settings, IScheduleSettings scheduleSettings)
        {
            _scheduleSettings = scheduleSettings;
            _settings = settings;
            InitializeComponent();
            LevelReliability.Text = @"Обычный";
            VersionExcelСomboBox.Text = @"Книга Excel (*.xlsx)";
            FileName.Text = @"Ретро_{YYYY}{MM}";
            UI_RunTimeOffset.Value = DateTime.Now.Date;
            SetData();
        }

        private void SetData()
        {
            _bindingTriggers.Clear();
            var allTriggers = _settings.GetAllActions();
            foreach (var trigger in allTriggers)
            {
                _bindingTriggers.Add(trigger);
            }
            _triggerGridControl.DataSource = _bindingTriggers;
            _bindingConditions.AllowNew = true;
        }

        private void _gridViewTrigger_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            _currentAction = null;

            var rowsHandle = _gridViewTrigger.GetSelectedRows();
            var bindingSource = new BindingSource() { DataSource = _gridViewTrigger.DataSource };
            foreach (var rowHandle in rowsHandle)
            {
                var index = _gridViewTrigger.GetDataSourceRowIndex(rowHandle);
                if (index >= 0 && bindingSource.Count >= index)
                    _currentAction = (Action)bindingSource[index];
            }
            
            SetTrigger();
        }

        private void SetTrigger()
        {
            if (_currentAction == null) return;
            SetEnabledControls();
            NameOfTrigger.Text = _currentAction.NameTrigger;
            KadrId.Text = _currentAction.KadrId.ToString();
            InfoCheckBox.Checked = _currentAction.Info;
            ObjectFieldCheckBox.Checked = _currentAction.ObjectField;
            TypeParamCheckBox.Checked = _currentAction.TypeParam;
            IdParamCheckBox.Checked = _currentAction.IdParam;
            IdTableParamCheckBox.Checked = _currentAction.IdTableParam;
            NameArchTableCheckBox.Checked = _currentAction.NameArchTable;
            StateColumnHeaderCheckBox.Checked = _currentAction.StateColumnHeader;
            FilesFolder.Text = _currentAction.FilesFolder;
            FileName.Text = _currentAction.FileName;
            LevelReliability.Text = _currentAction.IsHighLevelReliability ? @"Высокий" : @"Обычный";
            VersionExcelСomboBox.Text = _currentAction.VersionExcel == ".xlsx" ? @"Книга Excel (*.xlsx)" : @"Книга Excel 97-2003 (*.xls)";
            GuidTrigger.Text = _currentAction.GuidAction.ToString();
            UI_RunTimeOffset.Value = DateTime.Now.Date.AddSeconds(_currentAction.RunTimeOffset);

            _queryGridControl.DataSource = null;
            _bindingConditions = new BindingList<QueryCondition>();
            if (_currentAction.QueryConditions == null) return;
            foreach (var condition in _currentAction.QueryConditions)
            {
                _bindingConditions.Add(condition);
            }
            _queryGridControl.DataSource = _bindingConditions;
        }

        private void SetEnabledControls()
        {
            NameOfTrigger.Enabled = true;
            KadrId.Enabled = true;
            InfoCheckBox.Enabled = true;
            ObjectFieldCheckBox.Enabled = true;
            TypeParamCheckBox.Enabled = true;
            IdParamCheckBox.Enabled = true;
            IdTableParamCheckBox.Enabled = true;
            NameArchTableCheckBox.Enabled = true;
            StateColumnHeaderCheckBox.Enabled = true;
            FilesFolder.Enabled = true;
            FileName.Enabled = true;
            LevelReliability.Enabled = true;
            VersionExcelСomboBox.Enabled = true;
            GuidTrigger.Enabled = true;
            UI_RunTimeOffset.Enabled = true;
        }

        private void _toolBtnUpdate_Click(object sender, EventArgs e)
        {
            SetData();
        }

        private void _toolBtnAddLink_Click(object sender, EventArgs e)
        {
            _bindingTriggers.Add(new Action(Guid.NewGuid(), "Действие", "", "Ретро_{YYYY}{MM}", "", "", false, false, false, false, false, false, false, false, 0, new List<QueryCondition>(), 0));
            if (_bindingTriggers.Count > 0)
                _gridViewTrigger.FocusedRowHandle = _bindingTriggers.Count - 1;
            _settings.AddTrigger(_bindingTriggers.Last());
        }

        private void _toolBtnSaveLink_Click(object sender, EventArgs e)
        {
            if (_currentAction == null) return;
            try
            {
                _currentAction.GuidAction = new Guid(GuidTrigger.Text);
                _currentAction.IdParam = IdParamCheckBox.Checked;
                _currentAction.IdTableParam = IdTableParamCheckBox.Checked;
                _currentAction.IsHighLevelReliability = LevelReliability.Text == @"Высокий";
                _currentAction.NameArchTable = NameArchTableCheckBox.Checked;
                _currentAction.StateColumnHeader = StateColumnHeaderCheckBox.Checked;
                var versionExcel = (VersionExcelСomboBox.Text == @"Книга Excel (*.xlsx)") ? ".xlsx" : ".xls";
                _currentAction.VersionExcel = versionExcel;
                _currentAction.FileName = FileName.Text.Replace(".xlsx", "").Replace(".xls","") + versionExcel;
                _currentAction.FilesFolder = FilesFolder.Text;
                _currentAction.NameTrigger = NameOfTrigger.Text;
                _currentAction.KadrId = int.Parse(KadrId.Text);
                _currentAction.Info = InfoCheckBox.Checked;
                _currentAction.ObjectField = ObjectFieldCheckBox.Checked;
                _currentAction.TypeParam = TypeParamCheckBox.Checked;
                _currentAction.StateColumnHeader = StateColumnHeaderCheckBox.Checked;
                _currentAction.RunTimeOffset = (int)UI_RunTimeOffset.Value.TimeOfDay.TotalSeconds;

                _currentAction.QueryConditions.Clear();
                foreach (var condition in _bindingConditions)
                {
                    _currentAction.QueryConditions.Add(condition);
                }
                _settings.UpdateAction(_currentAction);
            }
            catch (Exception)
            {
                RSDU.Messaging.RsduMessageForm.ShowDialog("Не удалось сохранить действие", "Внимание!");
            }
            SetData();
        }

        private void _toolBtnDelLink_Click(object sender, EventArgs e)
        {
            if (_currentAction == null) return;
            var allSchedule = _scheduleSettings.GetAllSchedule();
            foreach (var schedule in allSchedule)
            {
                schedule.Actions.Remove(_currentAction);
                _scheduleSettings.UpdateSchedule(schedule);    
            }

            _settings.DeleteTrigger(_currentAction);

            SetData();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (_currentAction == null) return;
            var queryForm = new QueryConditionForm();
            queryForm.StartPosition = FormStartPosition.CenterParent;
            DialogResult res = queryForm.ShowDialog(this);
            if (res == DialogResult.OK)
            {
                var query = queryForm.CurrentQueryCondition;
                query.GuidQuery = Guid.NewGuid();
                query.GuidTrigger = _currentAction.GuidAction;
                _bindingConditions.Add(query);
            }
            //_queryConditionsGridView.AddNewRow();
            //int rowHandle = _queryConditionsGridView.GetRowHandle(_queryConditionsGridView.DataRowCount);
            //if (_queryConditionsGridView.IsNewItemRow(rowHandle))
            //{
            //    _queryConditionsGridView.SetRowCellValue(rowHandle, "NameQuerry", "Имя" + _queryConditionsGridView.RowCount);
            //    _queryConditionsGridView.SetRowCellValue(rowHandle, "GuidQuery", Guid.NewGuid().ToString());
            //    _queryConditionsGridView.SetRowCellValue(rowHandle, "GuidAction", _currentAction.GuidAction.ToString());
            //}
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _queryConditionsGridView.DeleteRow(_queryConditionsGridView.FocusedRowHandle);
        }

        private void _queryConditionsGridView_InitNewRow(object sender, DevExpress.XtraGrid.Views.Grid.InitNewRowEventArgs e)
        {
            _queryConditionsGridView.SetRowCellValue(e.RowHandle,"NameQuerry","Имя"+e.RowHandle);
        }

        // Только целые числа
        private void KadrId_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar < 48 || e.KeyChar > 57)
            {
                e.Handled = true;
            }
        }
        
        private void KadrId_TextChanged(object sender, EventArgs e)
        {
            int i;
            if (!int.TryParse(KadrId.Text, out i))
                KadrId.BackColor = Color.Red;
            else
            {
                KadrId.BackColor = Color.White;
            }
        }
    }
}
