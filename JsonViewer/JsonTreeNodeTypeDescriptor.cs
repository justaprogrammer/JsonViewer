using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace EPocalipse.Json.Viewer
{
    class JsonTreeNodeTypeDescriptor : ICustomTypeDescriptor
    {
        JsonTreeNode _node;
        PropertyDescriptorCollection _propertyCollection;

        public JsonTreeNodeTypeDescriptor(JsonTreeNode node)
        {
            _node = node;
            InitPropertyCollection();
        }

        private void InitPropertyCollection()
        {
            List<PropertyDescriptor> propertyDescriptors = new List<PropertyDescriptor>();

            foreach (JsonTreeNode childNode in _node.Nodes)
            {
                PropertyDescriptor pd = new JsonTreeNodePropertyDescriptor(childNode);
                propertyDescriptors.Add(pd);
            }
            _propertyCollection = new PropertyDescriptorCollection(propertyDescriptors.ToArray());
        }

        AttributeCollection ICustomTypeDescriptor.GetAttributes()
        {
            return TypeDescriptor.GetAttributes(_node, true);
        }

        string ICustomTypeDescriptor.GetClassName()
        {
            return TypeDescriptor.GetClassName(_node, true);
        }

        string ICustomTypeDescriptor.GetComponentName()
        {
            return TypeDescriptor.GetComponentName(_node, true);
        }

        TypeConverter ICustomTypeDescriptor.GetConverter()
        {
            return TypeDescriptor.GetConverter(_node, true);
        }

        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(_node, true);
        }

        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
        {
            return null;
        }

        object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(_node, editorBaseType, true);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
        {
            return TypeDescriptor.GetEvents(_node, true);
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
        {
            return _propertyCollection;
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            return ((ICustomTypeDescriptor)this).GetProperties(null);
        }

        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
        {
            return _node;
        }
    }

    class JsonTreeNodePropertyDescriptor : PropertyDescriptor
    {
        JsonTreeNode _node;
        JsonTreeNodeTypeDescriptor[] _nodes;

        public JsonTreeNodePropertyDescriptor(JsonTreeNode node)
            : base(node.Id, null)
        {
            _node = node;
            if (_node.JsonType == JsonType.Array)
                InitNodes();
        }

        private void InitNodes()
        {
            List<JsonTreeNodeTypeDescriptor> nodeList = new List<JsonTreeNodeTypeDescriptor>();
            foreach (JsonTreeNode child in _node.Nodes)
            {
                nodeList.Add(new JsonTreeNodeTypeDescriptor(child));
            }
            _nodes = nodeList.ToArray();
        }

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override Type ComponentType
        {
            get
            {
                return null;
            }
        }

        public override object GetValue(object component)
        {
            switch (_node.JsonType)
            {
                case JsonType.Array:
                    return _nodes;
                case JsonType.Object:
                    return _node;
                default:
                    return _node.Value;
            }
        }

        public override bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public override Type PropertyType
        {
            get
            {
                switch (_node.JsonType)
                {
                    case JsonType.Array:
                        return typeof(object[]);
                    case JsonType.Object:
                        return typeof(JsonTreeNode);
                    default:
                        return _node.Value == null ? typeof(string) : _node.Value.GetType();
                }
            }
        }

        public override void ResetValue(object component)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void SetValue(object component, object value)
        {
            //_node.Value = value;
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }

        public override object GetEditor(Type editorBaseType)
        {
            return base.GetEditor(editorBaseType);
        }
    }
}
