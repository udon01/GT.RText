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
        /// <summary>
        /// Designates whether the currently loaded content is a project folder.
        /// </summary>
        private bool _isUiFolderProject;

        /// <summary>
        /// Designates whether the currently loaded project is a GT6 locale project, 
        /// where all RT2 files are contained within a single global folder with a file for each locale.
        /// </summary>
        private bool _isGT6AndAboveProjectStyle;

        /// <summary>
        /// List of the current RText's curently openned.
        /// </summary>
        private List<RTextParser> _rTexts;

        private ListViewColumnSorter _columnSorter;

        public RTextParser CurrentRText => _rTexts[tabControlLocalFiles.SelectedIndex];
        public RTextPageBase CurrentPage { get; set; }

        public Main()
        {
            InitializeComponent();

            listViewPages.Columns.Add("Category", -2, HorizontalAlignment.Left);

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
                DisplayPages();
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

            bool firstTab = true;
            string[] files = Directory.GetFiles(dialog.FileName, "*", SearchOption.TopDirectoryOnly);

            if (files.Any(f => RTextParser.Locales.ContainsKey(Path.GetFileNameWithoutExtension(f))))
            {
                // Assume GT6+, where all RT2 files are all in one global folder compacted (i.e rtext/common/<LOCALE>.rt2)
                _isGT6AndAboveProjectStyle = true;

                foreach (var file in files)
                {
                    string locale = Path.GetFileNameWithoutExtension(file);
                    if (RTextParser.Locales.TryGetValue(locale, out string localeName))
                    {
                        var rtext = ReadRTextFile(file);
                        if (rtext != null)
                        {
                            rtext.LocaleCode = locale;
                            var tab = new TabPage(localeName);
                            tabControlLocalFiles.TabPages.Add(tab);

                            if (firstTab)
                            {
                                DisplayPages();
                                firstTab = false;
                            }
                        }
                    }
                }
            }
            else
            {
                // Locale files are located per-UI project, in their own folder (i.e arcade/US/rtext.rt2)
                string[] folders = Directory.GetDirectories(dialog.FileName, "*", SearchOption.TopDirectoryOnly);
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
                                DisplayPages();
                                firstTab = false;
                            }
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
                        if (_isGT6AndAboveProjectStyle)
                        {
                            string localePath = Path.Combine(dialog.FileName, $"{rtext.LocaleCode}.rt2");
                            rtext.RText.Save(localePath);
                        }
                        else
                        {
                            string localePath = Path.Combine(dialog.FileName, rtext.LocaleCode);
                            Directory.CreateDirectory(localePath);

                            rtext.RText.Save(Path.Combine(localePath, "rtext.rt2"));
                        }
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
            listViewPages.BeginUpdate();
            listViewPages.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listViewPages.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            listViewPages.EndUpdate();

            listViewEntries.BeginUpdate();
            listViewEntries.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listViewEntries.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            listViewEntries.EndUpdate();
        }


        private void listViewCategories_SelectedIndexChanged(object sender, EventArgs e)
        {
            listViewEntries.Items.Clear();

            if (listViewPages.SelectedItems.Count <= 0 || listViewPages.SelectedItems[0] == null) return;

            try
            {
                var lViewItem = listViewPages.SelectedItems[0];
                var page = (RTextPageBase)lViewItem.Tag;
                CurrentPage = page;

                DisplayEntries(page);

                toolStripStatusLabel.Text = $"{page.Name} - parsed with {page.PairUnits.Count} entries.";
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
            if (listViewPages.SelectedItems.Count <= 0 || listViewPages.SelectedItems[0] == null) return;
            if (listViewEntries.SelectedItems.Count <= 0 || listViewEntries.SelectedItems[0] == null) return;

            try
            {
                var categoryLViewItem = listViewPages.SelectedItems[0];
                var page = (RTextPageBase)categoryLViewItem.Tag;

                var lViewItem = listViewEntries.SelectedItems[0];
                RTextPairUnit rowData = (RTextPairUnit)lViewItem.Tag;

                var rowEditor = new RowEditor(rowData.ID, rowData.Label, rowData.Value, _isUiFolderProject);
                if (rowEditor.ShowDialog() == DialogResult.OK)
                {
                    if (_isUiFolderProject && rowEditor.ApplyToAllLocales)
                    {
                        foreach (var rt in _rTexts)
                        {
                            var rtPage = rt.RText.GetPages()[page.Name];
                            rtPage.DeleteRow(rowData.Label);
                            rtPage.AddRow(rowEditor.Id, rowEditor.Label, rowEditor.Data);
                        }

                        toolStripStatusLabel.Text = $"{rowEditor.Label} - edited to {_rTexts.Count} locales";
                    }
                    else
                    {
                        if (rowEditor.Label != rowEditor.Label && page.PairExists(rowEditor.Label))
                        {
                            MessageBox.Show("This label already exists in this category.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        // Remove, Add - Incase label was changed else we can't track it in our page
                        page.DeleteRow(rowData.Label);
                        page.AddRow(rowEditor.Id, rowEditor.Label, rowEditor.Data);

                        toolStripStatusLabel.Text = $"{rowEditor.Label} - edited";
                    }

                    DisplayEntries(page);
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
            if (listViewPages.SelectedItems.Count <= 0 || listViewPages.SelectedItems[0] == null) return;

            try
            {
                var pageLViewItem = listViewPages.SelectedItems[0];
                var page = (RTextPageBase)pageLViewItem.Tag;

                var rowEditor = new RowEditor(CurrentRText.RText is RT03, _isUiFolderProject);
                rowEditor.Id = page.GetLastId() + 1;

                if (rowEditor.ShowDialog() == DialogResult.OK)
                {
                    if (page.PairExists(rowEditor.Label))
                    {
                        MessageBox.Show("This label already exists in this category.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (_isUiFolderProject && rowEditor.ApplyToAllLocales)
                    {
                        foreach (var rt in _rTexts)
                        {
                            var rPage = rt.RText.GetPages()[page.Name];
                            rPage.AddRow(rowEditor.Id, rowEditor.Label, rowEditor.Data);
                        }

                        toolStripStatusLabel.Text = $"{rowEditor.Label} - added to {_rTexts.Count} locales";
                    }
                    else
                    {
                        var rowId = page.AddRow(rowEditor.Id, rowEditor.Label, rowEditor.Data);
                        toolStripStatusLabel.Text = $"{rowEditor.Label} - added";
                    }

                    DisplayEntries(page);
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
            if (listViewPages.SelectedItems.Count <= 0 || listViewPages.SelectedItems[0] == null) return;
            if (listViewEntries.SelectedItems.Count <= 0 || listViewEntries.SelectedItems[0] == null) return;

            try
            {
                var pageLViewItem = listViewPages.SelectedItems[0];
                var page = (RTextPageBase)pageLViewItem.Tag;

                var lViewItem = listViewEntries.SelectedItems[0];
                RTextPairUnit rowData = (RTextPairUnit)lViewItem.Tag;

                if (MessageBox.Show($"Are you sure you want to delete {rowData.Label}?", "Delete confirmation", MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    page.DeleteRow(rowData.Label);

                    toolStripStatusLabel.Text = $"{rowData.Label} - deleted";

                    DisplayEntries(page);
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

        private void tabControlLocalFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_isUiFolderProject || tabControlLocalFiles.TabCount <= 0)
                return;

            ClearListViews();
            DisplayPages();
        }

        private void addEditFromCSVFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!_rTexts.Any() || CurrentRText is null || CurrentPage is null) return;

            if (csvOpenFileDialog.ShowDialog(this) != DialogResult.OK) return;

            Dictionary<string, string> kv = new Dictionary<string, string>();
            try
            {
                using (var file = File.OpenText(csvOpenFileDialog.FileName))
                {
                    while (!file.EndOfStream)
                    {
                        string line = file.ReadLine();
                        if (string.IsNullOrEmpty(line))
                            continue;

                        string[] spl = line.Split(',');
                        if (spl.Length != 2)
                            continue;

                        string key = spl[0];
                        string value = spl[1];

                        if (!kv.TryGetValue(key, out _))
                            kv.Add(key, value);
                    }
                }
            }
            catch (Exception ex)
            {
                toolStripStatusLabel.Text = $"Unable to read CSV file: {ex.Message}";
                return;
            }

            if (!kv.Any())
            {
                toolStripStatusLabel.Text = "Error: No valid key/value pairs found in CSV file.";
                return;
            }

            if (_isUiFolderProject)
            {
                var res = MessageBox.Show($"Add to all opened locales?", "Confirmation", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
                if (res == DialogResult.Yes)
                {

                    foreach (var rtext in _rTexts)
                    {
                        if (rtext.RText.GetPages().TryGetValue(CurrentPage.Name, out var localePage))
                            localePage.AddPairs(kv);
                    }
                    toolStripStatusLabel.Text = $"Added/Edited {kv.Count} entries for {_rTexts.Count} locales.";
                }
                else if (res == DialogResult.No)
                {
                    CurrentPage.AddPairs(kv);
                    toolStripStatusLabel.Text = $"Added/Edited {kv.Count} entries.";
                }
                else
                    return;
            }
            else
            {
                CurrentPage.AddPairs(kv);
                toolStripStatusLabel.Text = $"Added/Edited {kv.Count} entries.";
            }

            
            DisplayEntries(CurrentPage);
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
            listViewPages.BeginUpdate();
            listViewPages.Items.Clear();
            listViewPages.EndUpdate();
        }

        private void ClearEntriesLView()
        {
            listViewEntries.BeginUpdate();
            listViewEntries.Items.Clear();
            listViewEntries.EndUpdate();
        }

        private RTextParser ReadRTextFile(string filePath)
        {
            var rText = new RTextParser(new ConsoleWriter());
            try
            {
                byte[] data = File.ReadAllBytes(filePath);
                rText.Read(data);
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

        private void DisplayPages()
        {
            if (CurrentRText == null)
            {
                MessageBox.Show("Read a valid RT04 file first.", "Oops...", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            listViewPages.BeginUpdate();
            listViewPages.Items.Clear();
            var pages = CurrentRText.RText.GetPages();
            var items = new ListViewItem[pages.Count];

            int i = 0;
            foreach (var page in pages)
                items[i++] = new ListViewItem(page.Key) { Tag = page.Value };

            listViewPages.Items.AddRange(items);

            listViewPages.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listViewPages.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            listViewPages.EndUpdate();
        }

        private void DisplayEntries(RTextPageBase page)
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
            var entries = page.PairUnits;
            var items = new ListViewItem[entries.Count];

            int i = 0;
            foreach (var entry in entries)
            {
                if ((CurrentRText.RText is RT03) == false)
                    items[i] = new ListViewItem(new[] { i.ToString(), entry.Value.ID.ToString(), entry.Value.Label, entry.Value.Value }) { Tag = entry.Value };
                else
                    items[i] = new ListViewItem(new[] { i.ToString(), entry.Value.Label, entry.Value.Value }) { Tag = entry.Value };
                i++;
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
    }
}
