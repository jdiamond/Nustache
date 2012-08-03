using System;
using System.Collections;
using System.Collections.Generic;
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
            return GetValueGetter(target, name).GetValue();
        }

        private static ValueGetter GetValueGetter(object target, string name)
        {
            return XmlNodeValueGetter.GetXmlNodeValueGetter(target, name)
                ?? PropertyDescriptorValueGetter.GetPropertyDescriptorValueGetter(target, name)
                ?? GenericDictionaryValueGetter.GetGenericDictionaryValueGetter(target, name)
                ?? DictionaryValueGetter.GetDictionaryValueGetter(target, name)
                ?? MethodInfoValueGetter.GetMethodInfoValueGetter(target, name)
                ?? PropertyInfoValueGetter.GetPropertyInfoValueGetter(target, name)
                ?? FieldInfoValueGetter.GetFieldInfoValueGetter(target, name)
                ?? ComparableValueGetter.GetEvalutatorValueGetter(target, name)
                ?? (ValueGetter)new NoValueGetter();
        }

        #endregion

        #region Abstract methods

        public abstract object GetValue();

        #endregion

        #region Constants for derived classes that use reflection

        protected const BindingFlags DefaultBindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase;
        protected const StringComparison DefaultNameComparison = StringComparison.CurrentCultureIgnoreCase;

        #endregion
    }

    internal class XmlNodeValueGetter : ValueGetter
    {
        internal static XmlNodeValueGetter GetXmlNodeValueGetter(object target, string name)
        {
            if (target is XmlNode)
            {
                return new XmlNodeValueGetter((XmlNode)target, name);
            }

            return null;
        }

        private readonly XmlNode _target;
        private readonly string _name;

        private XmlNodeValueGetter(XmlNode target, string name)
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
        internal static PropertyDescriptorValueGetter GetPropertyDescriptorValueGetter(object target, string name)
        {
            if (target is ICustomTypeDescriptor)
            {
                var typeDescriptor = (ICustomTypeDescriptor)target;
                PropertyDescriptorCollection properties = typeDescriptor.GetProperties();

                foreach (PropertyDescriptor property in properties)
                {
                    if (String.Equals(property.Name, name, DefaultNameComparison))
                    {
                        return new PropertyDescriptorValueGetter(target, property);
                    }
                }
            }

            return null;
        }

        private readonly object _target;
        private readonly PropertyDescriptor _propertyDescriptor;

        private PropertyDescriptorValueGetter(object target, PropertyDescriptor propertyDescriptor)
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
        internal static MethodInfoValueGetter GetMethodInfoValueGetter(object target, string name)
        {
            MemberInfo[] methods = target.GetType().GetMember(name, MemberTypes.Method, DefaultBindingFlags);

            foreach (MethodInfo method in methods)
            {
                if (MethodCanGetValue(method))
                {
                    return new MethodInfoValueGetter(target, method);
                }
            }

            return null;
        }

        private static bool MethodCanGetValue(MethodInfo method)
        {
            return method.ReturnType != typeof(void) &&
                   method.GetParameters().Length == 0;
        }

        private readonly object _target;
        private readonly MethodInfo _methodInfo;

        private MethodInfoValueGetter(object target, MethodInfo methodInfo)
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
        internal static PropertyInfoValueGetter GetPropertyInfoValueGetter(object target, string name)
        {
            PropertyInfo property = target.GetType().GetProperty(name, DefaultBindingFlags);

            if (property != null && PropertyCanGetValue(property))
            {
                return new PropertyInfoValueGetter(target, property);
            }

            return null;
        }

        private static bool PropertyCanGetValue(PropertyInfo property)
        {
            return property.CanRead;
        }

        private readonly object _target;
        private readonly PropertyInfo _propertyInfo;

        private PropertyInfoValueGetter(object target, PropertyInfo propertyInfo)
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
        internal static FieldInfoValueGetter GetFieldInfoValueGetter(object target, string name)
        {
            FieldInfo field = target.GetType().GetField(name, DefaultBindingFlags);

            if (field != null)
            {
                return new FieldInfoValueGetter(target, field);
            }

            return null;
        }

        private readonly object _target;
        private readonly FieldInfo _fieldInfo;

        private FieldInfoValueGetter(object target, FieldInfo fieldInfo)
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
        internal static DictionaryValueGetter GetDictionaryValueGetter(object target, string name)
        {
            if (target is IDictionary)
            {
                var dictionary = (IDictionary)target;

                if (dictionary.Contains(name))
                {
                    return new DictionaryValueGetter(dictionary, name);
                }
            }

            return null;
        }

        private readonly IDictionary _target;
        private readonly string _key;

        private DictionaryValueGetter(IDictionary target, string key)
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
        internal static GenericDictionaryValueGetter GetGenericDictionaryValueGetter(object target, string name)
        {
            Type dictionaryType = null;

            foreach (var interfaceType in target.GetType().GetInterfaces())
            {
                if (interfaceType.IsGenericType &&
                    interfaceType.GetGenericTypeDefinition() == typeof(IDictionary<,>) &&
                    interfaceType.GetGenericArguments()[0] == typeof(string))
                {
                    dictionaryType = interfaceType;

                    break;
                }
            }

            if (dictionaryType != null)
            {
                var containsKeyMethod = dictionaryType.GetMethod("ContainsKey");

                if ((bool)containsKeyMethod.Invoke(target, new object[] { name }))
                {
                    return new GenericDictionaryValueGetter(target, name, dictionaryType);
                }
            }

            return null;
        }

        private readonly object _target;
        private readonly string _key;
        private readonly MethodInfo _getMethod;

        private GenericDictionaryValueGetter(object target, string key, Type dictionaryType)
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

    internal class ComparableValueGetter : ValueGetter
    {
        private readonly IComparable _left;
        private readonly object _right;
        public enum ComparisonType
        {
            Gte,
            Gt,
            Lt,
            Lte,
            Eq,
            Ne
        }

        private readonly ComparisonType _comparisonType;

        private ComparableValueGetter(IComparable left, object right, ComparisonType comparisonType)
        {
            _left = left;
            _right = right;
            _comparisonType = comparisonType;
        }

        public override object GetValue()
        {
            int result = _left.CompareTo(_right);

            switch (_comparisonType)
            {
                case ComparisonType.Gte:
                    return result >= 0;
                case ComparisonType.Gt:
                    return result > 0;
                case ComparisonType.Lt:
                    return result < 0;
                case ComparisonType.Lte:
                    return result <= 0;
                case ComparisonType.Eq:
                    return result == 0;
                case ComparisonType.Ne:
                    return result != 0;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        internal static ValueGetter GetEvalutatorValueGetter(object target, string name)
        {
            string[] parameters = null;
            if (name.StartsWith("$") && name.Contains("(") && name.EndsWith(")"))
            {
                int start = name.IndexOf('(') + 1;
                string tkParams = name.Substring(start, name.Length - start - 1);
                parameters = tkParams.Split(',');
                name = name.Substring(0, start - 1);

                if (target is IComparable && parameters.Length == 1)
                {
                    var comparableTarget = target as IComparable;
                    var typedParam = Convert.ChangeType(parameters[0], target.GetType());
                    switch (name.ToLowerInvariant())
                    {
                        case "$gt":
                            return new ComparableValueGetter(comparableTarget, typedParam, ComparisonType.Gt);
                        case "$lt":
                            return new ComparableValueGetter(comparableTarget, typedParam, ComparisonType.Lt);
                        case "$eq":
                            return new ComparableValueGetter(comparableTarget, typedParam, ComparisonType.Eq);
                        case "$ne":
                            return new ComparableValueGetter(comparableTarget, typedParam, ComparisonType.Ne);
                        case "$gte":
                            return new ComparableValueGetter(comparableTarget, typedParam, ComparisonType.Gte);
                        case "$lte":
                            return new ComparableValueGetter(comparableTarget, typedParam, ComparisonType.Lte);
                    }
                }
            }
            return null;
        }
    }
}