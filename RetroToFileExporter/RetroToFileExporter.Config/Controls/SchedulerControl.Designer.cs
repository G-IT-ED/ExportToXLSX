namespace RetroToFileExporter.Config.Controls
{
    partial class SchedulerControl
    {
        /// <summary> 
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Обязательный метод для поддержки конструктора - не изменяйте 
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
            this._scheduleGridControl = new DevExpress.XtraGrid.GridControl();
            this._gridViewSchedule = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.NameScheduleCol = new DevExpress.XtraGrid.Columns.GridColumn();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.IsEnabledShedule = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.ScheduleQuartz = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.NameSchedule = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this._gridControlTriggers = new DevExpress.XtraGrid.GridControl();
            this._gridViewTriggers = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.GuidTrigger = new DevExpress.XtraGrid.Columns.GridColumn();
            this.NameTrigger = new DevExpress.XtraGrid.Columns.GridColumn();
            this.TriggerName = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this._toolBtnUpdate = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this._toolBtnAddLink = new System.Windows.Forms.ToolStripButton();
            this._toolBtnSaveLink = new System.Windows.Forms.ToolStripButton();
            this._toolBtnDelLink = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).BeginInit();
            this.splitContainerControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._scheduleGridControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._gridViewSchedule)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._gridControlTriggers)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._gridViewTriggers)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainerControl1
            // 
            this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerControl1.Location = new System.Drawing.Point(0, 27);
            this.splitContainerControl1.Name = "splitContainerControl1";
            this.splitContainerControl1.Padding = new System.Windows.Forms.Padding(3);
            this.splitContainerControl1.Panel1.Controls.Add(this._scheduleGridControl);
            this.splitContainerControl1.Panel1.MinSize = 200;
            this.splitContainerControl1.Panel1.Padding = new System.Windows.Forms.Padding(3);
            this.splitContainerControl1.Panel1.Text = "Panel1";
            this.splitContainerControl1.Panel2.Controls.Add(this.groupBox1);
            this.splitContainerControl1.Panel2.Controls.Add(this.panel1);
            this.splitContainerControl1.Panel2.MinSize = 300;
            this.splitContainerControl1.Panel2.Padding = new System.Windows.Forms.Padding(3);
            this.splitContainerControl1.Panel2.Text = "Panel2";
            this.splitContainerControl1.Size = new System.Drawing.Size(815, 365);
            this.splitContainerControl1.SplitterPosition = 221;
            this.splitContainerControl1.TabIndex = 4;
            this.splitContainerControl1.Text = "splitContainerControl1";
            // 
            // _scheduleGridControl
            // 
            this._scheduleGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this._scheduleGridControl.Location = new System.Drawing.Point(3, 3);
            this._scheduleGridControl.MainView = this._gridViewSchedule;
            this._scheduleGridControl.Name = "_scheduleGridControl";
            this._scheduleGridControl.Size = new System.Drawing.Size(215, 353);
            this._scheduleGridControl.TabIndex = 0;
            this._scheduleGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this._gridViewSchedule});
            // 
            // _gridViewSchedule
            // 
            this._gridViewSchedule.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.NameScheduleCol});
            this._gridViewSchedule.GridControl = this._scheduleGridControl;
            this._gridViewSchedule.Name = "_gridViewSchedule";
            this._gridViewSchedule.OptionsBehavior.Editable = false;
            this._gridViewSchedule.OptionsCustomization.AllowFilter = false;
            this._gridViewSchedule.OptionsCustomization.AllowSort = false;
            this._gridViewSchedule.OptionsDetail.EnableMasterViewMode = false;
            this._gridViewSchedule.OptionsView.ColumnAutoWidth = false;
            this._gridViewSchedule.OptionsView.ShowGroupPanel = false;
            this._gridViewSchedule.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this._gridView_FocusedRowChanged);
            // 
            // NameScheduleCol
            // 
            this.NameScheduleCol.Caption = "Наименование расписания";
            this.NameScheduleCol.FieldName = "NameSchedule";
            this.NameScheduleCol.Name = "NameScheduleCol";
            this.NameScheduleCol.Visible = true;
            this.NameScheduleCol.VisibleIndex = 0;
            this.NameScheduleCol.Width = 194;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.IsEnabledShedule);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.ScheduleQuartz);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.NameSchedule);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(577, 103);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Настройка расписания";
            // 
            // IsEnabledShedule
            // 
            this.IsEnabledShedule.AutoSize = true;
            this.IsEnabledShedule.Enabled = false;
            this.IsEnabledShedule.Location = new System.Drawing.Point(11, 75);
            this.IsEnabledShedule.Name = "IsEnabledShedule";
            this.IsEnabledShedule.Size = new System.Drawing.Size(77, 17);
            this.IsEnabledShedule.TabIndex = 4;
            this.IsEnabledShedule.Text = "Включено";
            this.IsEnabledShedule.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 50);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(97, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "CRON выражение";
            // 
            // ScheduleQuartz
            // 
            this.ScheduleQuartz.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ScheduleQuartz.Enabled = false;
            this.ScheduleQuartz.Location = new System.Drawing.Point(153, 47);
            this.ScheduleQuartz.Name = "ScheduleQuartz";
            this.ScheduleQuartz.Size = new System.Drawing.Size(418, 21);
            this.ScheduleQuartz.TabIndex = 3;
            this.ScheduleQuartz.Text = "0 0 0 0/1  * ? *";
            this.ScheduleQuartz.Leave += new System.EventHandler(this.ScheduleQuartz_Leave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(141, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Наименование расписания";
            // 
            // NameSchedule
            // 
            this.NameSchedule.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.NameSchedule.Enabled = false;
            this.NameSchedule.Location = new System.Drawing.Point(153, 20);
            this.NameSchedule.Name = "NameSchedule";
            this.NameSchedule.Size = new System.Drawing.Size(418, 21);
            this.NameSchedule.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(577, 353);
            this.panel1.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.button2);
            this.groupBox2.Controls.Add(this.button1);
            this.groupBox2.Controls.Add(this._gridControlTriggers);
            this.groupBox2.Controls.Add(this.TriggerName);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(0, 106);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(571, 246);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Привязанные действия";
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Enabled = false;
            this.button2.Image = global::RetroToFileExporter.Config.Properties.Resources.delete_16;
            this.button2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button2.Location = new System.Drawing.Point(535, 19);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(28, 23);
            this.button2.TabIndex = 5;
            this.button2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Enabled = false;
            this.button1.Image = global::RetroToFileExporter.Config.Properties.Resources.new_16;
            this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button1.Location = new System.Drawing.Point(505, 19);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(27, 23);
            this.button1.TabIndex = 4;
            this.button1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // _gridControlTriggers
            // 
            this._gridControlTriggers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._gridControlTriggers.Enabled = false;
            this._gridControlTriggers.Location = new System.Drawing.Point(3, 51);
            this._gridControlTriggers.MainView = this._gridViewTriggers;
            this._gridControlTriggers.Name = "_gridControlTriggers";
            this._gridControlTriggers.Size = new System.Drawing.Size(562, 192);
            this._gridControlTriggers.TabIndex = 3;
            this._gridControlTriggers.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this._gridViewTriggers});
            // 
            // _gridViewTriggers
            // 
            this._gridViewTriggers.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.GuidTrigger,
            this.NameTrigger});
            this._gridViewTriggers.GridControl = this._gridControlTriggers;
            this._gridViewTriggers.Name = "_gridViewTriggers";
            this._gridViewTriggers.OptionsBehavior.Editable = false;
            this._gridViewTriggers.OptionsCustomization.AllowFilter = false;
            this._gridViewTriggers.OptionsCustomization.AllowSort = false;
            this._gridViewTriggers.OptionsDetail.EnableMasterViewMode = false;
            this._gridViewTriggers.OptionsPrint.AutoWidth = false;
            this._gridViewTriggers.OptionsView.ShowGroupPanel = false;
            // 
            // GuidTrigger
            // 
            this.GuidTrigger.Caption = "GUID действия";
            this.GuidTrigger.FieldName = "GuidAction";
            this.GuidTrigger.Name = "GuidTrigger";
            this.GuidTrigger.Visible = true;
            this.GuidTrigger.VisibleIndex = 0;
            // 
            // NameTrigger
            // 
            this.NameTrigger.Caption = "Наименование действия";
            this.NameTrigger.FieldName = "NameTrigger";
            this.NameTrigger.Name = "NameTrigger";
            this.NameTrigger.Visible = true;
            this.NameTrigger.VisibleIndex = 1;
            // 
            // TriggerName
            // 
            this.TriggerName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TriggerName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.TriggerName.Enabled = false;
            this.TriggerName.FormattingEnabled = true;
            this.TriggerName.Location = new System.Drawing.Point(153, 20);
            this.TriggerName.Name = "TriggerName";
            this.TriggerName.Size = new System.Drawing.Size(346, 21);
            this.TriggerName.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(102, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Выбрать действие";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._toolBtnUpdate,
            this.toolStripSeparator1,
            this._toolBtnAddLink,
            this._toolBtnSaveLink,
            this._toolBtnDelLink,
            this.toolStripSeparator2});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Margin = new System.Windows.Forms.Padding(5);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(2, 2, 10, 2);
            this.toolStrip1.Size = new System.Drawing.Size(815, 27);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // _toolBtnUpdate
            // 
            this._toolBtnUpdate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._toolBtnUpdate.Image = global::RetroToFileExporter.Config.Properties.Resources.update_16;
            this._toolBtnUpdate.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._toolBtnUpdate.Name = "_toolBtnUpdate";
            this._toolBtnUpdate.Size = new System.Drawing.Size(23, 20);
            this._toolBtnUpdate.Text = "Обновить";
            this._toolBtnUpdate.Click += new System.EventHandler(this._toolBtnUpdate_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 23);
            // 
            // _toolBtnAddLink
            // 
            this._toolBtnAddLink.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._toolBtnAddLink.Image = global::RetroToFileExporter.Config.Properties.Resources.new_16;
            this._toolBtnAddLink.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._toolBtnAddLink.Name = "_toolBtnAddLink";
            this._toolBtnAddLink.Size = new System.Drawing.Size(23, 20);
            this._toolBtnAddLink.Text = "Создать";
            this._toolBtnAddLink.Click += new System.EventHandler(this._toolBtnAddLink_Click);
            // 
            // _toolBtnSaveLink
            // 
            this._toolBtnSaveLink.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._toolBtnSaveLink.Image = global::RetroToFileExporter.Config.Properties.Resources.save_16;
            this._toolBtnSaveLink.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._toolBtnSaveLink.Name = "_toolBtnSaveLink";
            this._toolBtnSaveLink.Size = new System.Drawing.Size(23, 20);
            this._toolBtnSaveLink.Text = "Сохранить";
            this._toolBtnSaveLink.Click += new System.EventHandler(this._toolBtnSaveLink_Click);
            // 
            // _toolBtnDelLink
            // 
            this._toolBtnDelLink.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._toolBtnDelLink.Image = global::RetroToFileExporter.Config.Properties.Resources.delete_16;
            this._toolBtnDelLink.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._toolBtnDelLink.Name = "_toolBtnDelLink";
            this._toolBtnDelLink.Size = new System.Drawing.Size(23, 20);
            this._toolBtnDelLink.Text = "Удалить";
            this._toolBtnDelLink.Click += new System.EventHandler(this._toolBtnDelLink_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 23);
            // 
            // SchedulerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainerControl1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "SchedulerControl";
            this.Size = new System.Drawing.Size(815, 392);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).EndInit();
            this.splitContainerControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._scheduleGridControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._gridViewSchedule)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._gridControlTriggers)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._gridViewTriggers)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton _toolBtnUpdate;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton _toolBtnAddLink;
        private System.Windows.Forms.ToolStripButton _toolBtnSaveLink;
        private System.Windows.Forms.ToolStripButton _toolBtnDelLink;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox NameSchedule;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label2;
        private DevExpress.XtraGrid.GridControl _gridControlTriggers;
        private DevExpress.XtraGrid.Views.Grid.GridView _gridViewTriggers;
        private System.Windows.Forms.ComboBox TriggerName;
        private System.Windows.Forms.CheckBox IsEnabledShedule;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox ScheduleQuartz;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private DevExpress.XtraGrid.Columns.GridColumn GuidTrigger;
        private DevExpress.XtraGrid.Columns.GridColumn NameTrigger;
        private DevExpress.XtraGrid.GridControl _scheduleGridControl;
        private DevExpress.XtraGrid.Views.Grid.GridView _gridViewSchedule;
        private DevExpress.XtraGrid.Columns.GridColumn NameScheduleCol;
    }
}
