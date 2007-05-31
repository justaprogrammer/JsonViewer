using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace EPocalipse.Json.Viewer
{
    public partial class NodeVisualizer : UserControl, IJsonVisualizer
    {
        public NodeVisualizer()
        {
            InitializeComponent();
        }

        string IJsonViewerPlugin.DisplayName
        {
            get { return "Property Grid"; }
        }

        Control IJsonVisualizer.GetControl(JsonTreeNode node)
        {
            return this;
        }

        void IJsonVisualizer.Visualize(JsonTreeNode node)
        {
            this.pgJsonObject.SelectedObject = new JsonTreeNodeTypeDescriptor(node);
        }


        bool IJsonViewerPlugin.CanVisualize(JsonTreeNode node)
        {
            return true;
        }
    }
}
