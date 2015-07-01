#pragma warning disable 1584,1711,1572,1581,1580

using System;

namespace Nustache.Core
{
    /// <summary>
    ///   Service resonsible for identifying whether a value is a <see cref="Newtonsoft.Json.Linq.JValue" />, determining its
    ///   type and returns its value.
    /// </summary>
    public static class JValueIdentifier
    {
        #region Public Members

        /// <summary>
        ///   Determines whether the <paramref name="obj" /> is a <see cref="Newtonsoft.Json.Linq.JValue" />.
        /// </summary>
        /// <param name="obj">
        ///   The <see cref="object" /> retrieved from the model.
        /// </param>
        /// <returns>
        ///   <see langword="True" /> whether the <paramref name="obj" /> is a <see cref="Newtonsoft.Json.Linq.JValue" /> object
        ///   otherwise <see langword="False" />.
        /// </returns>
        public static bool IsJValue(object obj)
        {
            return obj.GetType().ToString().Equals("Newtonsoft.Json.Linq.JValue");
        }

        /// <summary>
        ///   Determines whether the value can be used as a <see cref="bool" /> and returns its representation.
        /// </summary>
        /// <param name="jValue">
        ///   The <see cref="object" /> retrieved from the model.
        /// </param>
        /// <returns>
        ///   The representation of the <paramref name="jValue" /> as a <see cref="bool" /> otherwise <see langword="null" />.
        /// </returns>
        public static object GetValue(object jValue)
        {
            var jValueType = jValue.GetType();
            var typeOfValue = jValueType.GetProperty("Type").GetValue(jValue, null).ToString();

            if (typeOfValue == "Boolean") // JTokenType.Boolean
            {
                return GetValue<bool>(jValueType, jValue);
            }

            if (typeOfValue == "String") // JTokenType.String
            {
                return GetValue<string>(jValueType, jValue);
            }

            if (typeOfValue == "Integer") // JTokenType.Integer
            {
                return GetValue<Int64>(jValueType, jValueType);
            }

            if (typeOfValue == "Float") // JTokenType.Float
            {
                return GetValue<double>(jValueType, jValueType);
            }

            return null;
        }

        #endregion

        #region Private Members

        private static T GetValue<T>(Type type, object obj)
        {
            var value = type.GetProperty("Value").GetValue(obj, null);
            var valid = value is T;

            return valid ? (T) value : default(T);
        }

        #endregion
    }
}