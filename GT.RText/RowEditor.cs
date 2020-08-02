using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GT.RText
{
    public partial class RowEditor : Form
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public string Data { get; set; }

        public RowEditor()
        {
            InitializeComponent();
        }

        public RowEditor(int id, string label, string data)
        {
            InitializeComponent();

            Id = id;
            Label = label;
            Data = data;

            numericUpDown_id.Value = id;
            textBox_label.Text = label;
            richTextBox_data.Text = data;
        }

        private void numericUpDown_id_ValueChanged(object sender, EventArgs e)
        {
            Id = (int)numericUpDown_id.Value;
        }

        private void textBox_label_TextChanged(object sender, EventArgs e)
        {
            Label = textBox_label.Text;
        }

        private void richTextBox_data_TextChanged(object sender, EventArgs e)
        {
            Data = richTextBox_data.Text;
        }

        private void button_save_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
