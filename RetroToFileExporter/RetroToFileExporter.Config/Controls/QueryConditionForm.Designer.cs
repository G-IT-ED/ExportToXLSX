namespace RetroToFileExporter.Config.Controls
{
    partial class QueryConditionForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QueryConditionForm));
            this.label1 = new System.Windows.Forms.Label();
            this.NameQuery = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.Offset = new System.Windows.Forms.NumericUpDown();
            this.IntervalCount = new System.Windows.Forms.NumericUpDown();
            this.Interval = new System.Windows.Forms.ComboBox();
            this.Direction = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Offset)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.IntervalCount)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(128, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Наименование запроса";
            // 
            // NameQuery
            // 
            this.NameQuery.Location = new System.Drawing.Point(140, 17);
            this.NameQuery.Name = "NameQuery";
            this.NameQuery.Size = new System.Drawing.Size(289, 20);
            this.NameQuery.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.Offset);
            this.groupBox1.Controls.Add(this.IntervalCount);
            this.groupBox1.Controls.Add(this.Interval);
            this.groupBox1.Controls.Add(this.Direction);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.NameQuery);
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(434, 149);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Настройка";
            // 
            // Offset
            // 
            this.Offset.Location = new System.Drawing.Point(140, 122);
            this.Offset.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.Offset.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.Offset.Name = "Offset";
            this.Offset.Size = new System.Drawing.Size(289, 20);
            this.Offset.TabIndex = 13;
            // 
            // IntervalCount
            // 
            this.IntervalCount.Location = new System.Drawing.Point(140, 96);
            this.IntervalCount.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.IntervalCount.Name = "IntervalCount";
            this.IntervalCount.Size = new System.Drawing.Size(289, 20);
            this.IntervalCount.TabIndex = 12;
            // 
            // Interval
            // 
            this.Interval.AutoCompleteCustomSource.AddRange(new string[] {
            "Час",
            "День",
            "Месяц",
            "Неделя",
            "Год"});
            this.Interval.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Interval.FormattingEnabled = true;
            this.Interval.Items.AddRange(new object[] {
            "Сутки",
            "Месяц",
            "Год"});
            this.Interval.Location = new System.Drawing.Point(140, 69);
            this.Interval.Name = "Interval";
            this.Interval.Size = new System.Drawing.Size(289, 21);
            this.Interval.TabIndex = 11;
            // 
            // Direction
            // 
            this.Direction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Direction.FormattingEnabled = true;
            this.Direction.Items.AddRange(new object[] {
            "Вперед",
            "Назад"});
            this.Direction.Location = new System.Drawing.Point(140, 42);
            this.Direction.Name = "Direction";
            this.Direction.Size = new System.Drawing.Size(289, 21);
            this.Direction.TabIndex = 10;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 98);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(128, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Количество интервалов";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 124);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(61, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Смещение";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 46);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(122, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Направление времени";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 72);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Интервал";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(266, 155);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 14;
            this.button1.Text = "Ок";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(347, 155);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 15;
            this.button2.Text = "Отмена";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // QueryConditionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 181);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "QueryConditionForm";
            this.Text = "Условие запроса";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Offset)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.IntervalCount)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox NameQuery;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox Interval;
        private System.Windows.Forms.ComboBox Direction;
        private System.Windows.Forms.NumericUpDown Offset;
        private System.Windows.Forms.NumericUpDown IntervalCount;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}