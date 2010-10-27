using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Nustache.Core
{
    public class RenderContext
    {
        private const BindingFlags DefaultBindingFlags =
            BindingFlags.Instance |
            BindingFlags.Public;

        private readonly Stack<object> _stack = new Stack<object>();
        private object _data;
        private readonly TextWriter _writer;

        public RenderContext(object data, TextWriter writer)
        {
            _data = data;
            _writer = writer;
        }

        public object GetValue(string name)
        {
            if (name == ".") return _data;

            if (_data == null) return null;

            var propertyInfo = _data.GetType().GetProperty(name, DefaultBindingFlags);

            if (propertyInfo == null) return "";

            var value = propertyInfo.GetValue(_data, null);

            return value;
        }

        public IEnumerable<object> GetValues(string name)
        {
            object value = GetValue(name);

            if (value is bool)
            {
                if ((bool)value)
                {
                    yield return value;
                }
            }
            else if (value is IEnumerable && !(value is string))
            {
                foreach (var item in ((IEnumerable)value))
                {
                    yield return item;
                }
            }
            else if (value != null)
            {
                yield return value;
            }
        }

        public void Push(object data)
        {
            _stack.Push(_data);
            _data = data;
        }

        public void Pop()
        {
            _data = _stack.Pop();
        }

        public void Write(string text)
        {
            _writer.Write(text);
        }
    }
}