using System;
using System.Windows.Forms;
using RetroToFileExporter.Core.Objects;

namespace RetroToFileExporter.Config.Controls
{
    public partial class QueryConditionForm : Form
    {
        public QueryCondition CurrentQueryCondition = new QueryCondition();
        public QueryConditionForm()
        {
            InitializeComponent();
            IntervalCount.Maximum = decimal.MaxValue;
            IntervalCount.Minimum = 1;
            Offset.Maximum = decimal.MaxValue;
            Offset.Minimum = decimal.MinValue;
            Direction.Text = @"Вперед";
            Interval.Text = @"День";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CurrentQueryCondition.Name = NameQuery.Text;
            CurrentQueryCondition.DirectionTime = Direction.Text;
            CurrentQueryCondition.Interval = Interval.Text;
            CurrentQueryCondition.IntervalCount = (int)IntervalCount.Value;
            CurrentQueryCondition.Offset = (int)Offset.Value;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
