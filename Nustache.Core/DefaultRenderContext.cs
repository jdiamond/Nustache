using System.IO;
using System.Reflection;

namespace Nustache.Core
{
    public class DefaultRenderContext : IRenderContext
    {
        private const BindingFlags DefaultBindingFlags =
            BindingFlags.Instance |
            BindingFlags.Public;

        private readonly TextWriter _writer;
        private readonly object _data;

        public DefaultRenderContext(TextWriter writer, object data)
        {
            _writer = writer;
            _data = data;
            CurrentValue = data;
        }

        public void Write(string text)
        {
            _writer.Write(text);
        }

        public object GetValue(string name)
        {
            if (name == ".") return CurrentValue;

            if (_data == null) return null;

            var propertyInfo = _data.GetType().GetProperty(name, DefaultBindingFlags);

            if (propertyInfo == null) return "";

            var value = propertyInfo.GetValue(_data, null);

            return value;
        }

        public object CurrentValue { get; set; }
    }
}