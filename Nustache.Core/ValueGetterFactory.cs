using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Xml;
using System.Linq;

namespace Nustache.Core
{
    public abstract class ValueGetterFactory
    {
        // jon_wingfield: this is still problematic for me. target will be null when compiling, which
        // really is a POLA violation when you're coding for non-compiled templates.  Feedback/suggestions welcome.
        /// <param name="target">Can be null if we're compiling.</param>
        public abstract ValueGetter GetValueGetter(object target, Type targetType, string name);

        protected const BindingFlags DefaultBindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase;
        protected const StringComparison DefaultNameComparison = StringComparison.CurrentCultureIgnoreCase;
    }

    public class ValueGetterFactoryCollection : Collection<ValueGetterFactory>
    {
        protected override void InsertItem(int index, ValueGetterFactory item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            base.InsertItem(index, item);
        }

        protected override void SetItem(int index, ValueGetterFactory item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            base.SetItem(index, item);
        }

        public ValueGetter GetValueGetter(object target, string name)
        {
            return GetValueGetterOrDefault(Items, target, name) ?? new NoValueGetter();
        }

        public ValueGetter GetCompiledGetter(Type targetType, string name)
        {
            foreach (var factory in Items)
            {
                var getter = factory.GetValueGetter(null, targetType, name);
                if (getter != null)
                {
                    return getter;
                }
            }

            return null;
        }
        
        private static ValueGetter GetValueGetterOrDefault(IEnumerable<ValueGetterFactory> factories, object target, string name)
        {
            foreach (var factory in factories)
            {
                var getter = factory.GetValueGetter(target, target.GetType(), name);
                if (getter != null)
                {
                    return getter;
                }
            }

            return null;
        }
    }

    public static class ValueGetterFactories
    {
        private static readonly ValueGetterFactoryCollection _factories = new ValueGetterFactoryCollection
        {
            new XmlNodeValueGetterFactory(),
            new PropertyDescriptorValueGetterFactory(),
            new GenericDictionaryValueGetterFactory(),
            new DictionaryValueGetterFactory(),
            new MethodInfoValueGatterFactory(),
            new PropertyInfoValueGetterFactory(),
            new FieldInfoValueGetterFactory()
        };

        public static ValueGetterFactoryCollection Factories
        {
            get { return _factories; }
        }
    }

    internal class XmlNodeValueGetterFactory : ValueGetterFactory
    {
        public override ValueGetter GetValueGetter(object target, Type targetType, string name)
        {
            if (target is XmlNode)
            {
                return new XmlNodeValueGetter((XmlNode)target, name);
            }

            return null;
        }
    }

    internal class PropertyDescriptorValueGetterFactory : ValueGetterFactory
    {
        public override ValueGetter GetValueGetter(object target, Type targetType, string name)
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
    }

    internal class MethodInfoValueGatterFactory : ValueGetterFactory
    {
        public override ValueGetter GetValueGetter(object target, Type targetType, string name)
        {
            MemberInfo[] methods = targetType.GetMember(name, MemberTypes.Method, DefaultBindingFlags);

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
    }

    internal class PropertyInfoValueGetterFactory : ValueGetterFactory
    {
        public override ValueGetter GetValueGetter(object target, Type targetType, string name)
        {
            PropertyInfo property = targetType.GetProperty(name, DefaultBindingFlags);

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
    }

    internal class FieldInfoValueGetterFactory : ValueGetterFactory
    {
        public override ValueGetter GetValueGetter(object target, Type targetType, string name)
        {
            FieldInfo field = targetType.GetField(name, DefaultBindingFlags);

            if (field != null)
            {
                return new FieldInfoValueGetter(target, field);
            }

            return null;
        }
    }

    internal class DictionaryValueGetterFactory : ValueGetterFactory
    {
        public override ValueGetter GetValueGetter(object target, Type targetType, string name)
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
    }

    internal class GenericDictionaryValueGetterFactory : ValueGetterFactory
    {
        public override ValueGetter GetValueGetter(object target, Type targetType, string name)
        {
            Type dictionaryType = GetSupportedInterfaceType(targetType);

            if (dictionaryType != null)
            {
                if (target == null)
                {
                    return new GenericDictionaryValueGetter(null, name, dictionaryType);
                }
                else 
                {
                    var containsKeyMethod = dictionaryType.GetMethod("ContainsKey");
                    if ((bool)containsKeyMethod.Invoke(target, new object[] { name }))
                    {
                        return new GenericDictionaryValueGetter(target, name, dictionaryType);
                    }
                }
            }

            return null;
        }

        private static Type GetSupportedInterfaceType(Type targetType)
        {
            Func<Type, bool> supportedInteface = interfaceType => interfaceType.IsGenericType &&
                    interfaceType.GetGenericTypeDefinition() == typeof(IDictionary<,>) &&
                    interfaceType.GetGenericArguments()[0] == typeof(string);

            if (supportedInteface(targetType))
            {
                return targetType;
            }
            else
            {
                return targetType.GetInterfaces().FirstOrDefault(supportedInteface);
            }
        }
    }
}