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

        #endregion

        #region Abstract methods

        public abstract object GetValue();

        #endregion
    }

    internal class XmlNodeValueGetter : ValueGetter
    {
        private readonly XmlNode _target;
        private readonly string _name;

        internal XmlNodeValueGetter(XmlNode target, string name)
        {
            _target = target;
            _name = name;
        }

        public override object GetValue()
        {
            if (_name[0] == '@')
            {
                if (_target.Attributes != null)
                {
                    XmlNode attribute = _target.Attributes.GetNamedItem(_name.Substring(1));

                    if (attribute != null)
                    {
                        return attribute.Value;
                    }
                }
            }
            else
            {
                XmlNodeList list = _target.SelectNodes(_name);

                if (list != null && list.Count > 0)
                {
                    return list;
                }
            }

            return NoValue;
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
        private readonly string _key;
        private readonly MethodInfo _getMethod;

        internal GenericDictionaryValueGetter(object target, string key, Type dictionaryType)
        {
            _target = target;
            _key = key;
            _getMethod = dictionaryType.GetMethod("get_Item");
        }

        public override object GetValue()
        {
            return _getMethod.Invoke(_target, new object[] { _key });
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