using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using EPocalipse.Json.Viewer;
using Fiddler;
using System.Drawing;

namespace EPocalipse.Json.Fiddler
{
    public class JsonInspector : Inspector2
    {
        private byte[] _body;
        JsonViewer viewer;
        protected HTTPHeaders _headers;

        public override void AddToTab(TabPage tabPage)
        {
            viewer = new JsonViewer();
            tabPage.Text = "JSON";
            tabPage.Controls.Add(viewer);
            viewer.Dock = DockStyle.Fill;
        }

        public override void ShowAboutBox()
        {
            MessageBox.Show("Json Inspector 1.1\nCopyright (c) 2007 Eyal Post\nhttp://www.epocalipse.com/blog", "Json Viewer", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public override int ScoreForContentType(string sMIMEType)
        {
            if (string.Compare(sMIMEType, "text/json", true) == 0)
            {
                return 50;
            }
            return 0;
        }

        public override int GetOrder()
        {
            return 0;
        }

        public void Clear()
        {
            _body = null;
            viewer.Clear();
        }

        public bool bDirty
        {
            get
            {
                return false;
            }
        }

        public bool bReadOnly
        {
            get
            {
                return false;
            }
            set
            {
                //_readOnly = value;
            }
        }

        public byte[] body
        {
            get
            {
                return _body;
            }
            set
            {
                _body = value;
                string json = Encoding.UTF8.GetString(_body);
                bool encoded = _headers.Exists("Transfer-Encoding") || _headers.Exists("Content-Encoding");
                if (_headers.ExistsAndEquals("Transfer-Encoding", "chunked"))
                {
                    //response in chuncked but still may contain the entire json string. Try to fix it.
                    json = json.Trim();
                    int startPos = json.IndexOf('\n');
                    int endPos = json.LastIndexOf('\r');
                    if (startPos >= 0 && endPos >= 0 && startPos != endPos)
                    {
                        json = json.Substring(startPos, endPos - startPos + 1);
                        json = json.Trim();
                    }
                }
                viewer.ShowTab(Tabs.Viewer);
                viewer.Json = json;
                if (viewer.HasErrors && encoded)
                {
                    viewer.ShowInfo("Error parsing JSON. Try using the Transformer to remove the encoding from this response");
                }
            }
        }

        public override void SetFontSize(float flSizeInPoints)
        {
            viewer.Font = new Font(viewer.Font.FontFamily, flSizeInPoints, FontStyle.Regular, GraphicsUnit.Point);
        }
    }

    public class JsonResponseInspector : JsonInspector, IResponseInspector2
    {
        public HTTPResponseHeaders headers
        {
            get
            {
                return null;
            }
            set
            {
                _headers = value;
            }
        }
    }

    public class JsonRequestInspector : JsonInspector, IRequestInspector2
    {
        public HTTPRequestHeaders headers
        {
            get
            {
                return null;
            }
            set
            {
                _headers = value;
            }
        }
    }
}
