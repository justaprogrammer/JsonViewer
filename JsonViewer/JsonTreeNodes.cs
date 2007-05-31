using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace EPocalipse.Json.Viewer
{
    public class JsonTreeNodes : IEnumerable<JsonTreeNode>
    {
        private List<JsonTreeNode> _nodes = new List<JsonTreeNode>();
        private Dictionary<string, JsonTreeNode> _nodesById = new Dictionary<string, JsonTreeNode>();
        private JsonTreeNode _parent;

        public JsonTreeNodes(JsonTreeNode parent)
        {
            _parent = parent;
        }

        public IEnumerator<JsonTreeNode> GetEnumerator()
        {
            return _nodes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(JsonTreeNode node)
        {
            node.Parent = _parent;
            _nodes.Add(node);
            _nodesById[node.Id] = node;
            _parent.Modified();
        }

        public int Count
        {
            get
            {
                return _nodes.Count;
            }
        }

        public JsonTreeNode this[int index]
        {
            get
            {
                return _nodes[index];
            }
        }

        public JsonTreeNode this[string id]
        {
            get
            {
                JsonTreeNode result;
                if (_nodesById.TryGetValue(id, out result))
                    return result;
                return null;
            }
        }

        public bool ContainId(string id)
        {
            return _nodesById.ContainsKey(id);
        }
    }
}
