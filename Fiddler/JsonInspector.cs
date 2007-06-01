using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using EPocalipse.Json.Viewer;

namespace EPocalipse.Json.Fiddler
{
    public class JsonInspector : Inspector, IResponseInspector
    {
        private byte[] _body;
        JsonViewer viewer;
        HTTPResponseHeaders _headers;

        public override void AddToTab(TabPage tabPage)
        {
            viewer = new JsonViewer();
            tabPage.Text = "JSON";
            tabPage.Controls.Add(viewer);
            viewer.Dock = DockStyle.Fill;
        }

        public override void Announce()
        {
            MessageBox.Show("Json Inspector 0.1\nCopyright (c) 2007 Eyal Post\nhttp://www.epocalipse.com/blog", "Json Viewer", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public override void ShowAboutBox()
        {
            MessageBox.Show("Json Inspector 0.1\nCopyright (c) 2007 Eyal Post\nhttp://www.epocalipse.com/blog", "Json Viewer", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                string json=Encoding.UTF8.GetString(_body);
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
}
