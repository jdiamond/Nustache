using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nustache.Compilation.Tests.Mustache_Spec
{
    /// <summary>
    /// Lovingly ripped off from https://stackoverflow.com/questions/6416017/json-net-deserializing-nested-dictionaries
    /// </summary>
    public class NestedDictionaryConverter : CustomCreationConverter<IDictionary<string, object>>
    {
        public override IDictionary<string, object> Create(Type objectType)
        {
            return new Dictionary<string, object>();
        }

        public override bool CanConvert(Type objectType)
        {
            // in addition to handling IDictionary<string, object>
            // we want to handle the deserialization of dict value
            // which is of type object
            return objectType == typeof(object) || base.CanConvert(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject
                || reader.TokenType == JsonToken.Null)
                return base.ReadJson(reader, objectType, existingValue, serializer);

            // if the next token is not an object
            // then fall back on standard deserializer (strings, numbers etc.)
            return serializer.Deserialize(reader);
        }
    }

}
