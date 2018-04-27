namespace RetroToFileExporter.Config
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this._pageSettings = new System.Windows.Forms.TabPage();
            this._trigger = new System.Windows.Forms.TabPage();
            this._schedule = new System.Windows.Forms.TabPage();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this._pageSettings);
            this.tabControl1.Controls.Add(this._trigger);
            this.tabControl1.Controls.Add(this._schedule);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(813, 492);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.tabControl1_Selecting);
            // 
            // _pageSettings
            // 
            this._pageSettings.Location = new System.Drawing.Point(4, 22);
            this._pageSettings.Name = "_pageSettings";
            this._pageSettings.Padding = new System.Windows.Forms.Padding(3);
            this._pageSettings.Size = new System.Drawing.Size(805, 466);
            this._pageSettings.TabIndex = 0;
            this._pageSettings.Text = "Настройка";
            this._pageSettings.UseVisualStyleBackColor = true;
            // 
            // _trigger
            // 
            this._trigger.Location = new System.Drawing.Point(4, 22);
            this._trigger.Name = "_trigger";
            this._trigger.Padding = new System.Windows.Forms.Padding(3);
            this._trigger.Size = new System.Drawing.Size(914, 447);
            this._trigger.TabIndex = 4;
            this._trigger.Text = "Действия";
            this._trigger.UseVisualStyleBackColor = true;
            // 
            // _schedule
            // 
            this._schedule.Location = new System.Drawing.Point(4, 22);
            this._schedule.Name = "_schedule";
            this._schedule.Padding = new System.Windows.Forms.Padding(3);
            this._schedule.Size = new System.Drawing.Size(914, 447);
            this._schedule.TabIndex = 3;
            this._schedule.Text = "Расписание";
            this._schedule.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(813, 492);
            this.Controls.Add(this.tabControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(821, 519);
            this.Name = "MainForm";
            this.Text = "Сервис автоматической выгрузки кадров ретроспективы";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage _schedule;
        private System.Windows.Forms.TabPage _trigger;
        private System.Windows.Forms.TabPage _pageSettings;
    }
}