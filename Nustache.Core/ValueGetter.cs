using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;

namespace Nustache.Core
{
    public abstract class ValueGetter
    {
        #region Static helper methods

        public static ValueGetter GetValueGetter(object target, string name)
        {
            return PropertyDescriptorValueGetter.GetPropertyDescriptorValueGetter(target, name)
                ?? MethodInfoValueGetter.GetMethodInfoValueGetter(target, name)
                ?? PropertyInfoValueGetter.GetPropertyInfoValueGetter(target, name)
                ?? FieldInfoValueGetter.GetFieldInfoValueGetter(target, name)
                ?? DictionaryValueGetter.GetDictionaryValueGetter(target, name)
                ?? (ValueGetter)new NullValueGetter();
        }

        public static bool CanGetValue(object target, string name)
        {
            return GetValueGetter(target, name).CanGetValue();
        }

        public static Type GetValueType(object target, string name)
        {
            return GetValueGetter(target, name).GetValueType();
        }

        public static object GetValue(object target, string name)
        {
            return GetValueGetter(target, name).GetValue();
        }

        #endregion

        #region Abstract methods

        public abstract bool CanGetValue();
        public abstract Type GetValueType();
        public abstract object GetValue();

        #endregion

        #region Constants for derived classes that use reflection

        protected const BindingFlags DefaultBindingFlags = BindingFlags.Public | BindingFlags.Instance;

        #endregion
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
                    if (property.Name ==  name)
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

        public override bool CanGetValue()
        {
            return true;
        }

        public override Type GetValueType()
        {
            return _propertyDescriptor.PropertyType;
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

        public override bool CanGetValue()
        {
            return true;
        }

        public override Type GetValueType()
        {
            return _methodInfo.ReturnType;
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

        public override bool CanGetValue()
        {
            return true;
        }

        public override Type GetValueType()
        {
            return _propertyInfo.PropertyType;
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

            if (field != null && FieldCanGetValue(field))
            {
                return new FieldInfoValueGetter(target, field);
            }

            return null;
        }

        private static bool FieldCanGetValue(FieldInfo field)
        {
            return !field.IsInitOnly;
        }

        private readonly object _target;
        private readonly FieldInfo _fieldInfo;

        private FieldInfoValueGetter(object target, FieldInfo fieldInfo)
        {
            _target = target;
            _fieldInfo = fieldInfo;
        }

        public override bool CanGetValue()
        {
            return true;
        }

        public override Type GetValueType()
        {
            return _fieldInfo.FieldType;
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

        public override bool CanGetValue()
        {
            return true;
        }

        public override Type GetValueType()
        {
            object value = GetValue();
            return value != null ? value.GetType() : typeof(object);
        }

        public override object GetValue()
        {
            return _target[_key];
        }
    }

    internal class NullValueGetter : ValueGetter
    {
        public override bool CanGetValue()
        {
            return false;
        }

        public override Type GetValueType()
        {
            throw new InvalidOperationException();
        }

        public override object GetValue()
        {
            throw new InvalidOperationException();
        }
    }
}