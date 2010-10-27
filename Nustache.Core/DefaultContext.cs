using System.Reflection;

namespace Nustache.Core
{
    public class DefaultContext : IContext
    {
        private const BindingFlags DefaultBindingFlags =
            BindingFlags.Instance |
            BindingFlags.Public;

        public DefaultContext(object data)
        {
            Current = data;
        }

        public object GetValue(string name)
        {
            if (name == ".") return Current;

            if (Current == null) return null;

            var propertyInfo = Current.GetType().GetProperty(name, DefaultBindingFlags);

            if (propertyInfo == null) return "";

            var value = propertyInfo.GetValue(Current, null);

            return value;
        }

        public object Current { get; set; }
    }
}