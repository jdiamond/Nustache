using System;
using System.Collections;
using System.ComponentModel;
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

#if NET40

        public static System.Linq.Expressions.Expression CompiledGetter(Type targetType, string path, System.Linq.Expressions.Expression dataParameter)
        {
            var getter = ValueGetterFactories.Factories.GetCompiledGetter(targetType, path);
            var visitor = new Nustache.Core.Compilation.CompiledValueGetterVisitor(targetType, dataParameter);
            if (getter != null)
            {
                getter.Accept(visitor);
                return visitor.CompiledGetter;
            }
            else
                return null;
        }
#endif

        #endregion

        #region Abstract methods

        public abstract object GetValue();

        internal void Accept(ValueGetterVisitor visitor)
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
            return _propertyDescriptor.GetValue(_target);
        }
    }

    internal class MethodInfoValueGetter : ValueGetter
    {
        private readonly object _target;
        internal readonly MethodInfo MethodInfo;

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

    internal class PropertyInfoValueGetter : ValueGetter
    {
        private readonly object _target;
        internal readonly PropertyInfo PropertyInfo;

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

    internal class FieldInfoValueGetter : ValueGetter
    {
        private readonly object _target;
        internal readonly FieldInfo FieldInfo;

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

    internal class GenericDictionaryValueGetter : ValueGetter
    {
        private readonly object _target;
        internal readonly string Key;
        internal readonly MethodInfo GetMethod;
        internal readonly Type DictionaryType;

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

    internal class NoValueGetter : ValueGetter
    {
        public override object GetValue()
        {
            return NoValue;
        }
    }
}