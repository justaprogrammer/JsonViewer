using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace EPocalipse.Json.Viewer
{
    public interface IJsonViewerPlugin
    {
        string DisplayName {get;}
        bool CanVisualize(JsonTreeNode node);
    }

    public interface IJsonTextVisualizer : IJsonViewerPlugin
    {
        string GetText(JsonTreeNode node);
    }

    public interface IJsonVisualizer : IJsonViewerPlugin
    {
        Control GetControl(JsonTreeNode node);
        void Visualize(JsonTreeNode node);
    }
}
