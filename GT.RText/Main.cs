using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

// Required for the non crappy folder picker 
// https://stackoverflow.com/q/11624298
using Microsoft.WindowsAPICodePack.Dialogs;

using GT.RText.Core;
using GT.RText.Core.Exceptions;
using GT.Shared.Logging;
using System.Linq;

namespace GT.RText
{
    public partial class Main : Form
    {
        private bool _isUiFolderProject;

        /// <summary>
        /// List of the current RText's curently openned.
        /// </summary>
        private List<RTextParser> _rTexts;

        private ListViewColumnSorter _columnSorter;

        public RTextParser CurrentRText => _rTexts[tabControlLocalFiles.SelectedIndex];

        public Main()
        {
            InitializeComponent();

            listViewCategories.Columns.Add("Category", -2, HorizontalAlignment.Left);

            _rTexts = new List<RTextParser>();
            _columnSorter = new ListViewColumnSorter();
            this.listViewEntries.ListViewItemSorter = _columnSorter;
            this.listViewEntries.Sorting = SortOrder.Ascending;
        }

        #region Events
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog(this) != DialogResult.OK) return;

            _rTexts.Clear();
            _isUiFolderProject = false;

            ClearListViews();
            ClearTabs();

            var rtext = ReadRTextFile(openFileDialog.FileName);
            if (rtext != null)
            {
                var tab = new TabPage(openFileDialog.FileName);
                tabControlLocalFiles.TabPages.Add(tab);
                DisplayCategories();
            }
        }

        private void openFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.EnsureFileExists = true;
            dialog.EnsurePathExists = true;

            dialog.IsFolderPicker = true;

            if (dialog.ShowDialog() != CommonFileDialogResult.Ok) return;

            _rTexts.Clear();

            _isUiFolderProject = true;

            ClearListViews();
            ClearTabs();

            string[] folders = Directory.GetDirectories(dialog.FileName, "*", SearchOption.TopDirectoryOnly);

            bool firstTab = true;
            foreach (var folder in folders)
            {
                string actualDirName = Path.GetFileName(folder);
                if (RTextParser.Locales.TryGetValue(actualDirName, out string localeName))
                {
                    var rt2File = Path.Combine(folder, "rtext.rt2");
                    if (!File.Exists(rt2File))
                        continue;

                    var rtext = ReadRTextFile(rt2File);
                    if (rtext != null)
                    {
                        rtext.LocaleCode = actualDirName;
                        var tab = new TabPage(localeName);
                        tabControlLocalFiles.TabPages.Add(tab);

                        if (firstTab)
                        {
                            DisplayCategories();
                            firstTab = false;
                        }
                    }
                }
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (_isUiFolderProject)
                {
                    var dialog = new CommonOpenFileDialog();
                    dialog.EnsureFileExists = true;
                    dialog.EnsurePathExists = true;

                    dialog.IsFolderPicker = true;

                    if (dialog.ShowDialog() != CommonFileDialogResult.Ok) return;

                    foreach (var rtext in _rTexts)
                    {
                        string localePath = Path.Combine(dialog.FileName, rtext.LocaleCode);
                        Directory.CreateDirectory(localePath);

                        rtext.RText.Save(Path.Combine(localePath, "rtext.rt2"));
                    }

                    toolStripStatusLabel.Text = $"{saveFileDialog.FileName} - saved successfully {_rTexts.Count} locales.";
                }
                else
                {
                    if (saveFileDialog.ShowDialog(this) != DialogResult.OK) return;

                    CurrentRText.RText.Save(saveFileDialog.FileName);
                    toolStripStatusLabel.Text = $"{saveFileDialog.FileName} - saved successfully.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                toolStripStatusLabel.Text = $"Failed to save, unknown error, please contact the developer.";
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Application.MessageLoop)
            {
                // WinForms app
                Application.Exit();
            }
            else
            {
                // Console app
                Environment.Exit(1);
            }
        }


        private void Main_SizeChanged(object sender, EventArgs e)
        {
            listViewCategories.BeginUpdate();
            listViewCategories.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listViewCategories.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            listViewCategories.EndUpdate();

            listViewEntries.BeginUpdate();
            listViewEntries.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listViewEntries.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            listViewEntries.EndUpdate();
        }


        private void listViewCategories_SelectedIndexChanged(object sender, EventArgs e)
        {
            listViewEntries.Items.Clear();

            if (listViewCategories.SelectedItems.Count <= 0 || listViewCategories.SelectedItems[0] == null) return;

            try
            {
                var lViewItem = listViewCategories.SelectedItems[0];
                var category = (IRTextCategory)lViewItem.Tag;

                DisplayEntries(category);

                toolStripStatusLabel.Text = $"{category.Name} - parsed with {category.Entries.Count} entries.";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                toolStripStatusLabel.Text = ex.Message;
            }
        }

        private void listViewEntries_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listViewEntries_DoubleClick(object sender, EventArgs e)
        {
            editToolStripMenuItem_Click(null, null);
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewCategories.SelectedItems.Count <= 0 || listViewCategories.SelectedItems[0] == null) return;
            if (listViewEntries.SelectedItems.Count <= 0 || listViewEntries.SelectedItems[0] == null) return;

            try
            {
                var categoryLViewItem = listViewCategories.SelectedItems[0];
                var category = (IRTextCategory)categoryLViewItem.Tag;

                var lViewItem = listViewEntries.SelectedItems[0];
                var rowData = ((int Index, int Id, string Label, string Data))lViewItem.Tag;

                var rowEditor = new RowEditor(rowData.Id, rowData.Label, rowData.Data, _isUiFolderProject);
                if (rowEditor.ShowDialog() == DialogResult.OK)
                {
                    if (_isUiFolderProject && rowEditor.ApplyToAllLocales)
                    {
                        foreach (var rt in _rTexts)
                        {
                            var rCat = rt.RText.GetCategories().Where(cat => cat.Name == category.Name).FirstOrDefault();
                            rCat.EditRow(rowData.Index, rowEditor.Id, rowEditor.Label, rowEditor.Data);
                        }

                        toolStripStatusLabel.Text = $"{rowData.Index} - edited to {_rTexts.Count} locales";
                    }
                    else
                    {
                        category.EditRow(rowData.Index, rowEditor.Id, rowEditor.Label, rowEditor.Data);
                        toolStripStatusLabel.Text = $"{rowData.Index} - edited";
                    }

                    DisplayEntries(category);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                toolStripStatusLabel.Text = ex.Message;
            }
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewCategories.SelectedItems.Count <= 0 || listViewCategories.SelectedItems[0] == null) return;

            try
            {
                var categoryLViewItem = listViewCategories.SelectedItems[0];
                var category = (IRTextCategory)categoryLViewItem.Tag;

                var rowEditor = new RowEditor(CurrentRText.RText is RT03) { ApplyToAllLocales = _isUiFolderProject };
                if (rowEditor.ShowDialog() == DialogResult.OK)
                {
                    if (_isUiFolderProject && rowEditor.ApplyToAllLocales)
                    {
                        foreach (var rt in _rTexts)
                        {
                            var rCat = rt.RText.GetCategories().Where(cat => cat.Name == category.Name).FirstOrDefault();
                            category.AddRow(rowEditor.Id, rowEditor.Label, rowEditor.Data);
                        }

                        toolStripStatusLabel.Text = $"{rowEditor.Label} - added to {_rTexts.Count} locales";
                    }
                    else
                    {
                        var rowId = category.AddRow(rowEditor.Id, rowEditor.Label, rowEditor.Data);
                        toolStripStatusLabel.Text = $"{rowEditor.Label} - added";
                    }

                    DisplayEntries(category);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                toolStripStatusLabel.Text = ex.Message;
            }
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewCategories.SelectedItems.Count <= 0 || listViewCategories.SelectedItems[0] == null) return;
            if (listViewEntries.SelectedItems.Count <= 0 || listViewEntries.SelectedItems[0] == null) return;

            try
            {
                var categoryLViewItem = listViewCategories.SelectedItems[0];
                var category = (IRTextCategory)categoryLViewItem.Tag;

                var lViewItem = listViewEntries.SelectedItems[0];
                var rowData = ((int Index, int Id, string Label, string Data))lViewItem.Tag;

                if (MessageBox.Show($"Are you sure you want to delete {rowData.Label}?", "Delete confirmation", MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    category.DeleteRow(rowData.Index);

                    toolStripStatusLabel.Text = $"{rowData.Index} - deleted";

                    DisplayEntries(category);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                toolStripStatusLabel.Text = ex.Message;
            }
        }

        private void listViewEntries_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == _columnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                _columnSorter.Order = _columnSorter.Order == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                _columnSorter.SortColumn = e.Column;
                _columnSorter.Order = SortOrder.Ascending;
            }

            // Adjust the sort icon
            this.listViewEntries.SetSortIcon(e.Column, _columnSorter.Order);

            // Perform the sort with these new sort options.
            this.listViewEntries.Sort();
        }
        #endregion

        private void ClearTabs()
        {
            tabControlLocalFiles.TabPages.Clear();
        }

        private void ClearListViews()
        {
            ClearCategoriesLView();
            ClearEntriesLView();
        }

        private void ClearCategoriesLView()
        {
            listViewCategories.BeginUpdate();
            listViewCategories.Items.Clear();
            listViewCategories.EndUpdate();
        }

        private void ClearEntriesLView()
        {
            listViewEntries.BeginUpdate();
            listViewEntries.Items.Clear();
            listViewEntries.EndUpdate();
        }

        private RTextParser ReadRTextFile(string filePath)
        {
            try
            {
                var rText = new RTextParser(filePath, new ConsoleWriter());
                _rTexts.Add(rText);
                return rText;
            }
            catch (XorKeyTooShortException ex)
            {
                toolStripStatusLabel.Text = $"Error reading the file: {filePath}";
                MessageBox.Show("Couldn't decrypt all strings. Please contact xfileFIN for more information.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                toolStripStatusLabel.Text = $"Error reading the file: {filePath}";
            }

            return null;
        }

        private void DisplayCategories()
        {
            if (CurrentRText == null)
            {
                MessageBox.Show("Read a valid RT04 file first.", "Oops...", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            listViewCategories.BeginUpdate();
            listViewCategories.Items.Clear();
            var categories = CurrentRText.RText.GetCategories();
            var items = new ListViewItem[categories.Count];
            for (var i = 0; i < categories.Count; i++)
            {
                items[i] = new ListViewItem(categories[i].Name) { Tag = categories[i] };
            }

            listViewCategories.Items.AddRange(items);

            listViewCategories.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listViewCategories.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            listViewCategories.EndUpdate();
        }

        private void DisplayEntries(IRTextCategory category)
        {
            listViewEntries.BeginUpdate();
            SortEntriesListView(0);
            listViewEntries.Clear();

            // Set the view to show details.
            listViewEntries.View = View.Details;
            // Allow the user to edit item text.
            listViewEntries.LabelEdit = true;
            // Show item tooltips.
            listViewEntries.ShowItemToolTips = true;
            // Allow the user to rearrange columns.
            //lView.AllowColumnReorder = true;
            // Select the item and subitems when selection is made.
            listViewEntries.FullRowSelect = true;
            // Display grid lines.
            listViewEntries.GridLines = true;

            // Add column headers
            listViewEntries.Columns.Add("RecNo", -2, HorizontalAlignment.Left);
            if ((CurrentRText.RText is RT03) == false)
                listViewEntries.Columns.Add("Id", -2, HorizontalAlignment.Left);
            listViewEntries.Columns.Add("Label", -2, HorizontalAlignment.Left);
            listViewEntries.Columns.Add("String", -2, HorizontalAlignment.Left);

            // Add entries
            var entries = category.Entries;
            var items = new ListViewItem[entries.Count];
            for (var i = 0; i < entries.Count; i++)
            {
                if ((CurrentRText.RText is RT03) == false)
                    items[i] = new ListViewItem(new[] { (i + 1).ToString(), entries[i].Id.ToString(), entries[i].Label, entries[i].Data }) { Tag = entries[i] };
                else
                    items[i] = new ListViewItem(new[] { (i + 1).ToString(), entries[i].Label, entries[i].Data }) { Tag = entries[i] };
            }

            listViewEntries.Items.AddRange(items);

            listViewEntries.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listViewEntries.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            listViewEntries.EndUpdate();
        }

        private void SortEntriesListView(int columnIndex)
        {
            // Set the column number that is to be sorted; default to ascending.
            _columnSorter.SortColumn = columnIndex;
            _columnSorter.Order = SortOrder.Ascending;

            // Adjust the sort icon
            this.listViewEntries.SetSortIcon(columnIndex, _columnSorter.Order);

            // Perform the sort with these new sort options.
            this.listViewEntries.Sort();
        }

        private void tabControlLocalFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_isUiFolderProject || tabControlLocalFiles.TabCount <= 0)
                return;

            ClearListViews();
            DisplayCategories();
        }
    }
}
