namespace GT.RText
{
    partial class RowEditor
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
            this.components = new System.ComponentModel.Container();
            this.label_id = new System.Windows.Forms.Label();
            this.numericUpDown_id = new System.Windows.Forms.NumericUpDown();
            this.label_label = new System.Windows.Forms.Label();
            this.textBox_label = new System.Windows.Forms.TextBox();
            this.label_data = new System.Windows.Forms.Label();
            this.richTextBox_data = new System.Windows.Forms.RichTextBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cut = new System.Windows.Forms.ToolStripMenuItem();
            this.copy = new System.Windows.Forms.ToolStripMenuItem();
            this.paste = new System.Windows.Forms.ToolStripMenuItem();
            this.undo = new System.Windows.Forms.ToolStripMenuItem();
            this.button_save = new System.Windows.Forms.Button();
            this.applyAllLocalesCheckBox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_id)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label_id
            // 
            this.label_id.Dock = System.Windows.Forms.DockStyle.Top;
            this.label_id.Location = new System.Drawing.Point(0, 0);
            this.label_id.Name = "label_id";
            this.label_id.Size = new System.Drawing.Size(537, 21);
            this.label_id.TabIndex = 0;
            this.label_id.Text = "String ID";
            this.label_id.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // numericUpDown_id
            // 
            this.numericUpDown_id.Dock = System.Windows.Forms.DockStyle.Top;
            this.numericUpDown_id.Location = new System.Drawing.Point(0, 21);
            this.numericUpDown_id.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.numericUpDown_id.Name = "numericUpDown_id";
            this.numericUpDown_id.Size = new System.Drawing.Size(537, 19);
            this.numericUpDown_id.TabIndex = 1;
            this.numericUpDown_id.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericUpDown_id.ValueChanged += new System.EventHandler(this.numericUpDown_id_ValueChanged);
            // 
            // label_label
            // 
            this.label_label.Dock = System.Windows.Forms.DockStyle.Top;
            this.label_label.Location = new System.Drawing.Point(0, 40);
            this.label_label.Name = "label_label";
            this.label_label.Size = new System.Drawing.Size(537, 21);
            this.label_label.TabIndex = 2;
            this.label_label.Text = "Label (Must be Unique)";
            this.label_label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textBox_label
            // 
            this.textBox_label.AllowDrop = true;
            this.textBox_label.Dock = System.Windows.Forms.DockStyle.Top;
            this.textBox_label.Location = new System.Drawing.Point(0, 61);
            this.textBox_label.Name = "textBox_label";
            this.textBox_label.Size = new System.Drawing.Size(537, 19);
            this.textBox_label.TabIndex = 3;
            this.textBox_label.TextChanged += new System.EventHandler(this.textBox_label_TextChanged);
            // 
            // label_data
            // 
            this.label_data.Dock = System.Windows.Forms.DockStyle.Top;
            this.label_data.Location = new System.Drawing.Point(0, 80);
            this.label_data.Name = "label_data";
            this.label_data.Size = new System.Drawing.Size(537, 21);
            this.label_data.TabIndex = 4;
            this.label_data.Text = "Data";
            this.label_data.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // richTextBox_data
            // 
            this.richTextBox_data.ContextMenuStrip = this.contextMenuStrip1;
            this.richTextBox_data.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox_data.EnableAutoDragDrop = true;
            this.richTextBox_data.Location = new System.Drawing.Point(0, 101);
            this.richTextBox_data.Name = "richTextBox_data";
            this.richTextBox_data.Size = new System.Drawing.Size(537, 163);
            this.richTextBox_data.TabIndex = 5;
            this.richTextBox_data.Text = "";
            this.richTextBox_data.TextChanged += new System.EventHandler(this.richTextBox_data_TextChanged);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cut,
            this.copy,
            this.paste,
            this.undo});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(181, 114);
            this.contextMenuStrip1.Closed += new System.Windows.Forms.ToolStripDropDownClosedEventHandler(this.contextMenuStrip1_Closed);
            this.contextMenuStrip1.Opened += new System.EventHandler(this.contextMenuStrip1_Opened);
            // 
            // cut
            // 
            this.cut.Name = "cut";
            this.cut.Size = new System.Drawing.Size(180, 22);
            this.cut.Text = "Cut(&T)";
            this.cut.Click += new System.EventHandler(this.cut_Click);
            // 
            // copy
            // 
            this.copy.Name = "copy";
            this.copy.Size = new System.Drawing.Size(180, 22);
            this.copy.Text = "Copy(&C)";
            this.copy.Click += new System.EventHandler(this.copy_Click);
            // 
            // paste
            // 
            this.paste.Name = "paste";
            this.paste.Size = new System.Drawing.Size(180, 22);
            this.paste.Text = "Paste(&P)";
            this.paste.Click += new System.EventHandler(this.paste_Click);
            // 
            // undo
            // 
            this.undo.Name = "undo";
            this.undo.Size = new System.Drawing.Size(180, 22);
            this.undo.Text = "Undo(&U)";
            this.undo.Click += new System.EventHandler(this.undo_Click);
            // 
            // button_save
            // 
            this.button_save.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.button_save.Location = new System.Drawing.Point(0, 264);
            this.button_save.Name = "button_save";
            this.button_save.Size = new System.Drawing.Size(537, 21);
            this.button_save.TabIndex = 6;
            this.button_save.Text = "Save";
            this.button_save.UseVisualStyleBackColor = true;
            this.button_save.Click += new System.EventHandler(this.button_save_Click);
            // 
            // applyAllLocalesCheckBox
            // 
            this.applyAllLocalesCheckBox.AutoSize = true;
            this.applyAllLocalesCheckBox.Location = new System.Drawing.Point(424, 4);
            this.applyAllLocalesCheckBox.Name = "applyAllLocalesCheckBox";
            this.applyAllLocalesCheckBox.Size = new System.Drawing.Size(123, 16);
            this.applyAllLocalesCheckBox.TabIndex = 7;
            this.applyAllLocalesCheckBox.Text = "Apply to all locales";
            this.applyAllLocalesCheckBox.UseVisualStyleBackColor = true;
            this.applyAllLocalesCheckBox.Visible = false;
            this.applyAllLocalesCheckBox.CheckedChanged += new System.EventHandler(this.applyAllLocalesCheckBox_CheckedChanged);
            // 
            // RowEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(537, 285);
            this.Controls.Add(this.applyAllLocalesCheckBox);
            this.Controls.Add(this.richTextBox_data);
            this.Controls.Add(this.button_save);
            this.Controls.Add(this.label_data);
            this.Controls.Add(this.textBox_label);
            this.Controls.Add(this.label_label);
            this.Controls.Add(this.numericUpDown_id);
            this.Controls.Add(this.label_id);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "RowEditor";
            this.Text = "Editor";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_id)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label_id;
        private System.Windows.Forms.NumericUpDown numericUpDown_id;
        private System.Windows.Forms.Label label_label;
        private System.Windows.Forms.TextBox textBox_label;
        private System.Windows.Forms.Label label_data;
        private System.Windows.Forms.RichTextBox richTextBox_data;
        private System.Windows.Forms.Button button_save;
        private System.Windows.Forms.CheckBox applyAllLocalesCheckBox;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem cut;
        private System.Windows.Forms.ToolStripMenuItem copy;
        private System.Windows.Forms.ToolStripMenuItem paste;
        private System.Windows.Forms.ToolStripMenuItem undo;
    }
}
