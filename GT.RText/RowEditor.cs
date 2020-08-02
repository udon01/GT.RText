using System;
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

        public RowEditor(bool isWithoutId)
        {
            InitializeComponent();

            if (isWithoutId) HandleId(-1);
        }

        public RowEditor(int id, string label, string data)
        {
            InitializeComponent();

            Id = id;
            Label = label;
            Data = data;

            textBox_label.Text = label;
            richTextBox_data.Text = data;

            HandleId(id);
        }

        private void HandleId(int id)
        {
            if (id == -1)
            {
                // RT03 doesn't have ID
                label_id.Visible = false;
                numericUpDown_id.Visible = false;
            }
            else
            {
                label_id.Visible = true;
                numericUpDown_id.Visible = true;
                numericUpDown_id.Value = id;
            }
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
