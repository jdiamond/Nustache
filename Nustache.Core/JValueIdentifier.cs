using System;

namespace Nustache.Core
{
    /// <summary>
    ///     Service resonsible for identifying whether a value is a <see cref="Newtonsoft.Json.Linq.JValue" />, determining its type and determining whether
    ///     the value can be used as a <see cref="Boolean"/>.
    /// </summary>
    public sealed class JValueIdentifier
    {
        #region Public Members

        /// <summary>
        ///     Determines whether the <paramref name="obj"/> is a <see cref="Newtonsoft.Json.Linq.JValue" />.
        /// </summary>
        /// <param name="obj">
        ///     The <see cref="Object"/> retrieved from the model.
        /// </param>
        /// <returns>
        ///     <see langword="True"/> whether the <paramref name="obj"/> is a <see cref="Newtonsoft.Json.Linq.JValue" /> object otherwise <see langword="False"/>.
        /// </returns>
        public bool IsJValue(object obj)
        {
            return obj.GetType().ToString().Equals("Newtonsoft.Json.Linq.JValue");
        }

        /// <summary>
        ///     Determines whether the value can be used as a <see cref="Boolean"/> and returns its representation.
        /// </summary>
        /// <param name="jValue">
        ///     The <see cref="Object"/> retrieved from the model.
        /// </param>
        /// <returns>
        ///     The representation of the <paramref name="jValue"/> as a <see cref="Boolean"/> otherwise <see langword="null"/>.
        /// </returns>
        public bool? IsTruthy(object jValue)
        {
            var jValueType = jValue.GetType();
            var typeOfValue = jValueType.GetProperty("Type").GetValue(jValue, null).ToString();

            if (typeOfValue == "Boolean") // JTokenType.Boolean
            {
                return GetValue<bool>(jValueType, jValue);
            }

            if (typeOfValue == "String") // JTokenType.String
            {
                return !string.IsNullOrEmpty(GetValue<string>(jValueType, jValue));
            }

            if (typeOfValue == "Integer") // JTokenType.Integer
            {
                return GetValue<Int64>(jValueType, jValueType) > 0;
            }

            if (typeOfValue == "Float") // JTokenType.Float
            {
                return GetValue<double>(jValueType, jValueType) > 0;
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