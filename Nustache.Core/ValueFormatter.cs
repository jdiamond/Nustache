using System;
using System.Collections.Generic;
using System.Text;

namespace Nustache.Core
{
    /// <summary>
    /// Will format the the value with custom delegate
    /// </summary>
    public static class ValueFormatter
    {
        public delegate TResult Func<TInput, TResult>(TInput arg);
        private static Dictionary<Type, Func<object, object>> _typeFormatters = new Dictionary<Type, Func<object, object>>();
        private static bool _hasRegisteredFormatters;

        /// <summary>
        /// Gets whether the ValueFormatter has registered delegates, and if it should be used
        /// </summary>
        public static bool HasRegisteredFormatters
        {
            get
            {
                return _hasRegisteredFormatters;
            }
        }
        /// <summary>
        /// Adds a type and func to the collection
        /// </summary>
        /// <param name="typeFormatter">The delegate to execute on the object</param>
        /// <typeparam name="T">The type to format</typeparam>
        public static void Add<T>(Func<T, object> typeFormatter)
        {
            var type = typeof(T);
            if (!_typeFormatters.ContainsKey(type))
            {
                Func<object, object> valueFormatter = value => typeFormatter((T)value);
                _typeFormatters.Add(type, valueFormatter);
                _hasRegisteredFormatters = true;
            }
        }
        /// <summary>
        /// removes the formatting function from the collection
        /// </summary>
        /// <param name="type"></param>
        public static void Remove(Type type)
        {
            if (_typeFormatters.ContainsKey(type))
            {
                _typeFormatters.Remove(type);
                if (_typeFormatters.Count == 0)
                {
                    _hasRegisteredFormatters = false;
                }
            }
        }
        /// <summary>
        /// Formats an object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object Format(object obj)
        {
            if (!HasRegisteredFormatters)
            {
                return obj;
            }
            var type = obj.GetType();
            var func = _typeFormatters[type];
            if (func == null)
                return obj;
            try
            {
                return func(obj);
            }
            catch { return obj; }
        }
    }
}