using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.DebuggerVisualizers;
using System.Diagnostics;

[assembly: DebuggerVisualizer(typeof(EPocalipse.Json.Visualizer.JsonVisualizer), 
    Target = typeof(string), Description = "JSON")]

namespace EPocalipse.Json.Visualizer
{
    public class JsonVisualizer : DialogDebuggerVisualizer
    {
        protected override void Show(IDialogVisualizerService windowService, 
            IVisualizerObjectProvider objectProvider)
        {
            using (JsonVisualizerForm form = new JsonVisualizerForm())
            {
                form.Viewer.Json = objectProvider.GetObject().ToString();
                windowService.ShowDialog(form);
            }
        }
    }
}
