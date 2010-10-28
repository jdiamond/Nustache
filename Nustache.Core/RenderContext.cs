using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Nustache.Core
{
    public class RenderContext
    {
        private readonly Stack<object> _stack = new Stack<object>();
        private object _data;
        private readonly TextWriter _writer;
        private readonly Func<string, Template> _templateLocator;

        public RenderContext(object data, TextWriter writer, Func<string, Template> templateLocator)
        {
            _data = data;
            _writer = writer;
            _templateLocator = templateLocator;
        }

        public object GetValue(string name)
        {
            if (name == ".") return _data;

            var value = GetValue(name, _data);

            if (value != null)
            {
                return value;
            }

            foreach (var data in _stack)
            {
                value = GetValue(name, data);

                if (value != null)
                {
                    return value;
                }
            }

            return null;
        }

        private static object GetValue(string name, object data)
        {
            if (data == null)
            {
                return null;
            }

            var getter = ValueGetter.GetValueGetter(data, name);

            if (getter.CanGetValue())
            {
                return getter.GetValue();
            }

            return null;
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

        public void Include(string templateName)
        {
            // TODO: Check for null!

            var template = _templateLocator(templateName);
            template.Render(this);
        }
    }
}