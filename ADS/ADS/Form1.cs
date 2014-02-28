using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodeFluent.Runtime.BinaryServices;
using CodeFluent.Runtime.Utilities;

namespace ADS
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private string _path;
        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files)
            {
                if (file == null)
                    continue;

                try
                {
                    IEnumerable<NtfsAlternateStream> ntfsAlternateStreams = NtfsAlternateStream.EnumerateStreams(file);
                    _path = file;
                    listBox1.Items.Clear();
                    foreach (var stream in ntfsAlternateStreams)
                    {
                        listBox1.Items.Add(stream);
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex);
                }
            }
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            Open();
        }

        private void Open()
        {
            if (listBox1.SelectedItem == null)
                return;

            var stream = listBox1.SelectedItem as NtfsAlternateStream;
            if (stream == null)
                return;


            OpenOrSave form = new OpenOrSave();
            DialogResult dialogResult = form.ShowDialog();
            if (dialogResult != DialogResult.OK && dialogResult != DialogResult.Yes)
                return;

            if (form.OpenSave == OpenSave.Open)
            {
                string text = NtfsAlternateStream.ReadAllText(_path + stream.Name);
                TextView textView = new TextView(text);
                textView.Show();
            }
            else if (form.OpenSave == OpenSave.Save)
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.FileName = ConvertUtilities.ToFileName(stream.Name);
                DialogResult showDialog = dialog.ShowDialog();
                if (showDialog != DialogResult.OK && showDialog != DialogResult.Yes)
                    return;

                using (var dstStream = dialog.OpenFile())
                {
                    using (var srcStream = NtfsAlternateStream.Open(_path + stream.Name, FileAccess.Read, FileMode.Open, FileShare.Read))
                    {
                        srcStream.CopyTo(dstStream);
                    }
                }
            }
        }

        private void listBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Open();
            }
            else if (e.KeyCode == Keys.Delete)
            {
                if (listBox1.SelectedItem == null)
                    return;

                var stream = listBox1.SelectedItem as NtfsAlternateStream;
                if (stream == null)
                    return;

                try
                {
                    DialogResult dialogResult = MessageBox.Show(string.Format("Are you sure you want to delete '{0}'", stream.Name), "Confirm", MessageBoxButtons.YesNoCancel);
                    if (dialogResult != DialogResult.OK && dialogResult != DialogResult.Yes)
                        return;

                    NtfsAlternateStream.Delete(_path + stream.Name);
                    listBox1.Items.Remove(listBox1.SelectedItem);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex);
                }
            }
        }
    }
}
