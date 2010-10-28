using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Nustache.Core
{
    public class RenderContext
    {
        private const int IncludeLimit = 1024;
        private readonly Stack<object> _dataStack = new Stack<object>();
        private readonly Stack<Template> _templateStack = new Stack<Template>();
        private object _data;
        private readonly TextWriter _writer;
        private readonly Func<string, Template> _templateLocator;
        private int _includeLevel;

        public RenderContext(object data, TextWriter writer, Func<string, Template> templateLocator)
        {
            _data = data;
            _writer = writer;
            _templateLocator = templateLocator;
            _includeLevel = 0;
        }

        public object GetValue(string name)
        {
            if (name == ".") return _data;

            var value = GetValue(name, _data);

            if (value != null)
            {
                return value;
            }

            foreach (var data in _dataStack)
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

        public void PushData(object data)
        {
            _dataStack.Push(_data);
            _data = data;
        }

        public void PopData()
        {
            _data = _dataStack.Pop();
        }

        public void PushTemplate(Template template)
        {
            _templateStack.Push(template);
        }

        public void PopTemplate()
        {
            _templateStack.Pop();
        }

        public void Write(string text)
        {
            _writer.Write(text);
        }

        public void Include(string templateName)
        {
            if (_includeLevel >= IncludeLimit)
            {
                throw new NustacheException(
                    string.Format("You have reached the include limit of {0}. Are you trying to render infinitely recursive templates or data?", IncludeLimit));
            }

            _includeLevel++;

            var currentTemplate = _templateStack.Peek();

            var childTemplate = currentTemplate.GetTemplate(templateName);

            if (childTemplate != null)
            {
                childTemplate.Render(this);
            }
            else if (_templateLocator != null)
            {
                var externalTemplate = _templateLocator(templateName);
                externalTemplate.Render(this);
            }

            _includeLevel--;
        }
    }
}