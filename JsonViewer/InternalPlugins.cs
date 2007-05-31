using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace EPocalipse.Json.Viewer
{
    class AjaxNetDateTime: IJsonTextVisualizer
    {
        static readonly long epoch=new DateTime(1970, 1, 1).Ticks;

        public string GetText(JsonTreeNode node)
        {
            string text = (string)node.Value; 
            return "Ajax.Net Date:"+ConvertJSTicksToDateTime(Convert.ToInt64(text.Substring(1, text.Length - 2))).ToString();
        }

        private DateTime ConvertJSTicksToDateTime(long ticks)
        {
            return new DateTime((ticks * 10000) + epoch);
        }

        public string DisplayName
        {
            get { return "Ajax.Net DateTime"; }
        }

        public bool CanVisualize(JsonTreeNode node)
        {
            if (node.JsonType == JsonType.Value && node.Value is string)
            {
                string text = (string)node.Value;
                return (text.Length > 2 && text[0] == '@' && text[text.Length - 1] == '@');
            }
            return false;
        }
    }

    class Sample : IJsonVisualizer
    {
        TextBox tb;

        public Control GetControl(JsonTreeNode node)
        {
            if (tb == null)
            {
                tb = new TextBox();
                tb.Multiline = true;
            }
            return tb;
        }

        public void Visualize(JsonTreeNode node)
        {
            tb.Text = String.Format("Array {0} has {1} items", node.Id, node.Nodes.Count);
        }

        public string DisplayName
        {
            get { return "Sample"; }
        }

        public bool CanVisualize(JsonTreeNode node)
        {
            return (node.JsonType == JsonType.Array) && (node.ContainsFields("[0]"));
        }
    }
}
