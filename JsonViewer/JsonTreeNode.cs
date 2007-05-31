using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace EPocalipse.Json.Viewer
{
    [DebuggerDisplay("Text = {Text}")]
    public class JsonTreeNode
    {
        private string _id;
        private object _value;
        private JsonType _jsonType;
        private JsonTreeNodes _nodes;
        private JsonTreeNode _parent;
        private string _text;

        public JsonTreeNode()
        {
            _nodes=new JsonTreeNodes(this);
        }

        public string Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        public object Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        public JsonType JsonType
        {
            get
            {
                return _jsonType;
            }
            set
            {
                _jsonType = value;
            }
        }

        public JsonTreeNode Parent
        {
            get
            {
                return _parent;
            }
            set
            {
                _parent = value;
            }
        }

        public string Text
        {
            get
            {
                if (_text == null)
                {
                    if (JsonType == JsonType.Value)
                    {
                        string val = (Value == null ? "<null>" : Value.ToString());
                        if (Value is string)
                            val = "\"" + val + "\"";
                        _text = String.Format("{0} : {1}", Id, val);
                    }
                    else
                        _text = Id;
                }
                return _text;
            }
        }

        public JsonTreeNodes Nodes
        {
            get
            {
                return _nodes;
            }
        }

        internal void Modified()
        {
            _text = null;
        }

        public bool ContainsFields(params string[] ids )
        {
            foreach (string s in ids)
            {
            if (!_nodes.ContainId(s))
                return false;
            }
            return true;
        }
    }
}
