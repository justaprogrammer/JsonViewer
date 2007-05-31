using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using EPocalipse.Json.Viewer;

namespace EPocalipse.Json.JsonView
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            string[] args = Environment.GetCommandLineArgs();
            foreach (string arg in args)
            {
                if (arg.Equals("/c", StringComparison.OrdinalIgnoreCase))
                {
                    LoadFromClipboard();
                }
            }
        }

        private void LoadFromClipboard()
        {
            string json = Clipboard.GetText();
            if (!String.IsNullOrEmpty(json))
            {
                JsonViewer.ShowTab(Tabs.Viewer);
                JsonViewer.Json = json;
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            JsonViewer.ShowTab(Tabs.Text);
        }
    }
}