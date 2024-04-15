using System;
using System.Windows.Forms;

namespace GT.RText
{
    public partial class RowEditor : Form
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public string Data { get; set; }
        public bool ApplyToAllLocales { get; set; }

        private bool _isUiProject;

        public RowEditor()
        {
            InitializeComponent();
        }

        public RowEditor(bool isWithoutId, bool isUiProject)
        {
            InitializeComponent();

            if (isWithoutId) HandleId(-1);

            _isUiProject = isUiProject;
            if (isUiProject)
                applyAllLocalesCheckBox.Visible = true;
        }

        public RowEditor(int id, string label, string data, bool isUiProject)
        {
            InitializeComponent();

            Id = id;
            Label = label;
            Data = data;

            _isUiProject = isUiProject;
            if (_isUiProject)
                applyAllLocalesCheckBox.Visible = true;

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

        private void applyAllLocalesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            ApplyToAllLocales = applyAllLocalesCheckBox.Checked;
        }

        //コピーする
        private void copy_Click(object sender, System.EventArgs e)
        {
            if (richTextBox_data.SelectionLength > 0)
            {
                richTextBox_data.Copy();
            }
        }

        //切り取る
        private void cut_Click(object sender, System.EventArgs e)
        {
            if (richTextBox_data.SelectionLength > 0)
            {
                richTextBox_data.Cut();
            }
        }

        //貼り付ける
        private void paste_Click(object sender, System.EventArgs e)
        {
            IDataObject data = Clipboard.GetDataObject();
            if (data != null && data.GetDataPresent(DataFormats.Text) == true)
            {
                richTextBox_data.Paste();
            }
        }

        //元に戻す
        private void undo_Click(object sender, System.EventArgs e)
        {
            if (richTextBox_data.CanUndo)
            {
                richTextBox_data.Undo();
                richTextBox_data.ClearUndo();
            }
        }

        private void contextMenuStrip1_Opened(object sender, EventArgs e)
        {
            richTextBox_data.ReadOnly = true;
        }

        private void contextMenuStrip1_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            richTextBox_data.ReadOnly = false;
        }
    }
}
