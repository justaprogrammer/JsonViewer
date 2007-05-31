using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Collections;

namespace EPocalipse.Json.Viewer
{
    public enum JsonType { Object, Array, Value };

    class JsonParseError : ApplicationException
    {
        public JsonParseError() : base() { }
        public JsonParseError(string message) : base(message) { }
        protected JsonParseError(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public JsonParseError(string message, Exception innerException) : base(message, innerException) { }

    }

    public class JsonTree
    {
        private JsonTreeNode _root;
        private static NodeVisualizer _nodeVisualizer = new NodeVisualizer();

        public static JsonTree Parse(string json)
        {
            //Parse the JSON string
            object jsonObject;
            try
            {
                jsonObject = JavaScriptConvert.DeserializeObject(json);
            }
            catch (Exception e)
            {
                throw new JsonParseError(e.Message, e);
            }
            //Parse completed, build the tree
            return new JsonTree(jsonObject);
        }

        public JsonTree(object rootObject)
        {
            _root = ConvertToTreeNode("JSON", rootObject);
        }

        private JsonTreeNode ConvertToTreeNode(string id, object jsonObject)
        {
            JsonTreeNode node = CreateTeeNode(jsonObject);
            node.Id = id;
            AddChildren(jsonObject, node);
            return node;
        }

        private void AddChildren(object jsonObject, JsonTreeNode node)
        {
            JavaScriptObject javaScriptObject = jsonObject as JavaScriptObject;
            if (javaScriptObject != null)
            {
                foreach (KeyValuePair<string, object> pair in javaScriptObject)
                {
                    node.Nodes.Add(ConvertToTreeNode(pair.Key, pair.Value));
                }
            }
            else
            {
                JavaScriptArray javaScriptArray = jsonObject as JavaScriptArray;
                if (javaScriptArray != null)
                {
                    for (int i = 0; i < javaScriptArray.Count; i++)
                    {
                        node.Nodes.Add(ConvertToTreeNode("[" + i.ToString() + "]", javaScriptArray[i]));
                    }
                }
            }
        }

        private JsonTreeNode CreateTeeNode(object jsonObject)
        {
            JsonTreeNode node = new JsonTreeNode();
            if (jsonObject is JavaScriptArray)
                node.JsonType = JsonType.Array;
            else if (jsonObject is JavaScriptObject)
                node.JsonType = JsonType.Object;
            else
            {
                node.JsonType = JsonType.Value;
                node.Value = jsonObject;
            }
            return node;
        }

        public JsonTreeNode Root
        {
            get
            {
                return _root;
            }
        }

    }
}
