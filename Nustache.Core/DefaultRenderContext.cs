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

        public DefaultRenderContext(TextWriter writer, object data)
        {
            _writer = writer;
            CurrentValue = data;
        }

        public void Write(string text)
        {
            _writer.Write(text);
        }

        public object GetValue(string name)
        {
            if (name == ".") return CurrentValue;

            if (CurrentValue == null) return null;

            var propertyInfo = CurrentValue.GetType().GetProperty(name, DefaultBindingFlags);

            if (propertyInfo == null) return "";

            var value = propertyInfo.GetValue(CurrentValue, null);

            return value;
        }

        public object CurrentValue { get; set; }
    }
}