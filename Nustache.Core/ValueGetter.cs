using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Xml;
using System.Linq.Expressions;

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

        public static Expression CompiledGetter(Type targetType, string path, Expression dataParameter)
        {
            var factory = ValueGetterFactories.Factories.GetCompiledGetter(targetType, path);
            if (factory != null)
                return factory.CompiledGetter(targetType, dataParameter);
            else
                return null;
        }

        #endregion

        #region Abstract methods

        public abstract object GetValue();

        public abstract Expression CompiledGetter(Type targetType, Expression dataParameter);

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

        public override Expression CompiledGetter(Type targetType, Expression dataParameter)
        {
            throw new NotSupportedException();
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

        public override Expression CompiledGetter(Type targetType, Expression dataParameter)
        {
            throw new NotSupportedException();
        }
    }

    internal class MethodInfoValueGetter : ValueGetter
    {
        private readonly object _target;
        private readonly MethodInfo _methodInfo;

        internal MethodInfoValueGetter(object target, MethodInfo methodInfo)
        {
            _target = target;
            _methodInfo = methodInfo;
        }

        public override object GetValue()
        {
            return _methodInfo.Invoke(_target, null);
        }

        public override Expression CompiledGetter(Type targetType, Expression dataParameter)
        {
            return Expression.Call(dataParameter, _methodInfo);
        }
    }

    internal class PropertyInfoValueGetter : ValueGetter
    {
        private readonly object _target;
        private readonly PropertyInfo _propertyInfo;

        internal PropertyInfoValueGetter(object target, PropertyInfo propertyInfo)
        {
            _target = target;
            _propertyInfo = propertyInfo;
        }

        public override object GetValue()
        {
            return _propertyInfo.GetValue(_target, null);
        }

        public override Expression CompiledGetter(Type targetType, Expression dataParameter)
        {
            return Expression.Property(dataParameter, _propertyInfo.GetGetMethod());
        }
    }

    internal class FieldInfoValueGetter : ValueGetter
    {
        private readonly object _target;
        private readonly FieldInfo _fieldInfo;

        internal FieldInfoValueGetter(object target, FieldInfo fieldInfo)
        {
            _target = target;
            _fieldInfo = fieldInfo;
        }

        public override object GetValue()
        {
            return _fieldInfo.GetValue(_target);
        }

        public override Expression CompiledGetter(Type targetType, Expression dataParameter)
        {
            return Expression.Field(dataParameter, _fieldInfo);
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

        public override Expression CompiledGetter(Type targetType, Expression dataParameter)
        {
            throw new NotSupportedException();
        }
    }

    internal class GenericDictionaryValueGetter : ValueGetter
    {
        private readonly object _target;
        private readonly string _key;
        private readonly MethodInfo _getMethod;
        private readonly Type _dictionaryType;

        internal GenericDictionaryValueGetter(object target, string key, Type dictionaryType)
        {
            _target = target;
            _key = key;
            _getMethod = dictionaryType.GetMethod("get_Item");
            _dictionaryType = dictionaryType;
        }

        public override object GetValue()
        {
            return _getMethod.Invoke(_target, new object[] { _key });
        }

        public override Expression CompiledGetter(Type targetType, Expression dataParameter)
        {
            var containsKeyMethod = _dictionaryType.GetMethod("ContainsKey");

            return
                Expression.Condition(
                    Expression.Call(dataParameter, containsKeyMethod, Expression.Constant(_key)),
                    Expression.Call(dataParameter, _getMethod, Expression.Constant(_key)),
                    Expression.Default(_getMethod.ReturnType));
        }
    }

    internal class NoValueGetter : ValueGetter
    {
        public override object GetValue()
        {
            return NoValue;
        }

        public override Expression CompiledGetter(Type targetType, Expression dataParameter)
        {
            throw new NotSupportedException();
        }
    }
}