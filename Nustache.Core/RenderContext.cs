using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Nustache.Core
{
    public class RenderContext
    {
        private const int IncludeLimit = 1024;
        private readonly Stack<Container> _containerStack = new Stack<Container>();
        private readonly Stack<object> _dataStack = new Stack<object>();
        private readonly TextWriter _writer;
        private readonly Func<string, Template> _templateLocator;
        private int _includeLevel;

        public RenderContext(Template template, object data, TextWriter writer, Func<string, Template> templateLocator)
        {
            _containerStack.Push(template);
            _dataStack.Push(data);
            _writer = writer;
            _templateLocator = templateLocator;
            _includeLevel = 0;
        }

        public object GetValue(string name)
        {
            if (name == ".") return _dataStack.Peek();

            var value = GetValue(name, _dataStack.Peek());

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

            TemplateDefinition templateDefinition = GetTemplateDefinition(templateName);

            if (templateDefinition != null)
            {
                templateDefinition.Render(this);
            }
            else if (_templateLocator != null)
            {
                var template = _templateLocator(templateName);
                template.Render(this);
            }

            _includeLevel--;
        }

        private TemplateDefinition GetTemplateDefinition(string name)
        {
            foreach (var container in _containerStack)
            {
                var templateDefinition = container.GetTemplateDefinition(name);

                if (templateDefinition != null)
                {
                    return templateDefinition;
                }
            }

            return null;
        }

        public void Push(Container section, object data)
        {
            _containerStack.Push(section);
            _dataStack.Push(data);
        }

        public void Pop()
        {
            _containerStack.Pop();
        }
    }
}