using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using System.Xml;

namespace Nustache.Core
{
    public abstract class ValueGetter
    {
        public static readonly object NoValue = new object();

        #region Static helper methods

        public static object GetValue(object target, string name)
        {
            return ValueGetterFactories.Factories.GetValueGetter(target, name).GetValue();
        }

        #endregion

        #region Abstract methods

        public abstract object GetValue();

        public void Accept(ValueGetterVisitor visitor)
        {
            try
            {
                var method = visitor.GetType().GetMethod("Visit", new Type[] { this.GetType() });
                if (method != null)
                {
                    method.Invoke(visitor, new object[] { this });
                }
                else
                {
                    visitor.NoMatch(this);
                }
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        #endregion
    }

    internal class XmlNodeValueGetter : ValueGetter
    {
        private readonly XmlNode _target;
        private readonly string _name;
        private string _textValueLocated;
        private XmlNodeList _childNodeList;

        internal XmlNodeValueGetter(XmlNode target, string name)
        {
            _target = target;
            _name = name;
        }

        private bool HasChildNodeList()
        {
            _childNodeList = _target.SelectNodes(_name);
            return _childNodeList != null && _childNodeList.Count > 0;
        }

        private bool TryGetSingleTextNodeValue()
        {
            if (_childNodeList.Count != 1)
                return false;

            return TryGetNodeValueAsText(_childNodeList[0]);
        }

        private bool TryGetNodeValueAsText(XmlNode node)
        {
            if (node.ChildNodes.Count == 1
                && node.ChildNodes[0].NodeType == XmlNodeType.Text)
            {
                _textValueLocated = node.ChildNodes[0].Value;
                return true;
            }
            return false;
        }

        public override object GetValue()
        {
            if (_name[0] == '@' && TryGetStringByAttributeName(_name.Substring(1)))
                return _textValueLocated;

            if (HasChildNodeList())
            {
                if (TryGetSingleTextNodeValue())
                    return _textValueLocated;

                return _childNodeList;
            }

            if (TryGetStringByAttributeName(_name))
            {
                return _textValueLocated;
            }

            return NoValue;
        }

        private bool TryGetStringByAttributeName(string attributeName)
        {
            if (_target.Attributes != null)
            {
                XmlNode attribute = _target.Attributes.GetNamedItem(attributeName);
                if (attribute != null)
                {
                    _textValueLocated = attribute.Value;
                    return true;
                }
            }
            return false;
        }
    }

    internal class XmlNodeListIndexGetter : ValueGetter
    {
        private readonly XmlNodeList _target;
        private readonly int _index;
        private object _foundSingleValue;

        public XmlNodeListIndexGetter(XmlNodeList target, int index)
        {
            _target = target;
            _index = index;
        }

        private object GetNodeValue(XmlNode node)
        {
            if (node.ChildNodes.Count == 1
                && (node.ChildNodes[0].NodeType == XmlNodeType.Text || node.ChildNodes[0].NodeType == XmlNodeType.CDATA)
            )
            {
                return node.ChildNodes[0].Value;
            }
            else
            {
                return node;
            }
        }

        public override object GetValue()
        {
            return GetNodeValue(_target[_index]);
        }
    }

    internal class PropertyDescriptorValueGetter : ValueGetter
    {
        private readonly object _target;
        private readonly PropertyDescriptor _propertyDescriptor;

        internal PropertyDescriptorValueGetter(object target, PropertyDescriptor propertyDescriptor)
        {
            _target = target;
            _propertyDescriptor = propertyDescriptor;
        }

        public override object GetValue()
        {
            var value = _propertyDescriptor.GetValue(_target);
            return JValueIdentifier.IsJValue(value) ? JValueIdentifier.GetValue(value) : value;
        }
    }

    public class MethodInfoValueGetter : ValueGetter
    {
        private readonly object _target;
        public readonly MethodInfo MethodInfo;

        internal MethodInfoValueGetter(object target, MethodInfo methodInfo)
        {
            _target = target;
            MethodInfo = methodInfo;
        }

        public override object GetValue()
        {
            return MethodInfo.Invoke(_target, null);
        }
    }

    public class PropertyInfoValueGetter : ValueGetter
    {
        private readonly object _target;
        public readonly PropertyInfo PropertyInfo;

        internal PropertyInfoValueGetter(object target, PropertyInfo propertyInfo)
        {
            _target = target;
            PropertyInfo = propertyInfo;
        }

        public override object GetValue()
        {
            return PropertyInfo.GetValue(_target, null);
        }
    }

    public class FieldInfoValueGetter : ValueGetter
    {
        private readonly object _target;
        public readonly FieldInfo FieldInfo;

        internal FieldInfoValueGetter(object target, FieldInfo fieldInfo)
        {
            _target = target;
            FieldInfo = fieldInfo;
        }

        public override object GetValue()
        {
            return FieldInfo.GetValue(_target);
        }
    }

    internal class DictionaryValueGetter : ValueGetter
    {
        private readonly IDictionary _target;
        private readonly string _key;

        internal DictionaryValueGetter(IDictionary target, string key)
        {
            _target = target;
            _key = key;
        }

        public override object GetValue()
        {
            return _target[_key];
        }
    }

    public class GenericDictionaryValueGetter : ValueGetter
    {
        private readonly object _target;
        public readonly string Key;
        public readonly MethodInfo GetMethod;
        public readonly Type DictionaryType;

        internal GenericDictionaryValueGetter(object target, string key, Type dictionaryType)
        {
            _target = target;
            Key = key;
            GetMethod = dictionaryType.GetMethod("get_Item");
            DictionaryType = dictionaryType;
        }

        public override object GetValue()
        {
            return GetMethod.Invoke(_target, new object[] { Key });
        }
    }

    internal class ListValueByIndexGetter : ValueGetter
    {
        private readonly IList _target;
        private readonly int _index;

        public ListValueByIndexGetter(IList target, int index)
        {
            _target = target;
            _index = index;
        }
        public override object GetValue()
        {
            return _target[_index];
        }
    }

    internal class DataRowValueGetter : ValueGetter
    {
        private readonly DataRow _target;
        private readonly string _name;

        public DataRowValueGetter(DataRow target, string name)
        {
            _target = target;
            _name = name;
        }

        public override object GetValue()
        {
            if(_target.Table.Columns.Contains(_name)) 
            {
                return _target[_name];
            }

            return null;
        }
    }

    internal class NameValueCollectionValueGetter : ValueGetter
    {
        private readonly NameValueCollection _target;
        private readonly string _key;

        internal NameValueCollectionValueGetter(NameValueCollection target, string key)
        {
            _target = target;
            _key = key;
        }

        public override object GetValue()
        {
            return _target[_key];
        }
    }

    internal class NoValueGetter : ValueGetter
    {
        public override object GetValue()
        {
            return NoValue;
        }
    }
}