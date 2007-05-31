using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace EPocalipse.Json.Viewer
{
    class AjaxNetDateTime: ICustomTextProvider
    {
        static readonly long epoch=new DateTime(1970, 1, 1).Ticks;

        public string GetText(JsonObject jsonObject)
        {
            string text = (string)jsonObject.Value; 
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

        public bool CanVisualize(JsonObject jsonObject)
        {
            if (jsonObject.JsonType == JsonType.Value && jsonObject.Value is string)
            {
                string text = (string)jsonObject.Value;
                return (text.Length > 2 && text[0] == '@' && text[text.Length - 1] == '@');
            }
            return false;
        }
    }

    class Sample : IJsonVisualizer
    {
        TextBox tb;

        public Control GetControl(JsonObject jsonObject)
        {
            if (tb == null)
            {
                tb = new TextBox();
                tb.Multiline = true;
            }
            return tb;
        }

        public void Visualize(JsonObject jsonObject)
        {
            tb.Text = String.Format("Array {0} has {1} items", jsonObject.Id, jsonObject.Fields.Count);
        }

        public string DisplayName
        {
            get { return "Sample"; }
        }

        public bool CanVisualize(JsonObject jsonObject)
        {
            return (jsonObject.JsonType == JsonType.Array) && (jsonObject.ContainsFields("[0]"));
        }
    }
}
