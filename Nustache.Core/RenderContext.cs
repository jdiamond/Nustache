using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Nustache.Core
{
    public delegate Template TemplateLocator(string name);

    public delegate Object Lambda(string text);

    public class RenderContext
    {
        private const int IncludeLimit = 1024;
        private readonly Stack<Section> _sectionStack = new Stack<Section>();
        private readonly Stack<object> _dataStack = new Stack<object>();
        private readonly TextWriter _writer;
        private readonly TemplateLocator _templateLocator;
        private int _includeLevel;

        public RenderContext(Section section, object data, TextWriter writer, TemplateLocator templateLocator)
        {
            _sectionStack.Push(section);
            _dataStack.Push(data);
            _writer = writer;
            _templateLocator = templateLocator;
            _includeLevel = 0;
        }

        public object GetValue(string name)
        {
            if (name == ".")
            {
                return _dataStack.Peek();
            }

            foreach (var data in _dataStack)
            {
                if (data != null)
                {
                    var value = ValueGetter.GetValue(data, name);

                    if (value != null)
                    {
                        return value;
                    }
                }
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
            foreach (var section in _sectionStack)
            {
                var templateDefinition = section.GetTemplateDefinition(name);

                if (templateDefinition != null)
                {
                    return templateDefinition;
                }
            }

            return null;
        }

        public void Push(Section section, object data)
        {
            _sectionStack.Push(section);
            _dataStack.Push(data);
        }

        public void Pop()
        {
            _sectionStack.Pop();
            _dataStack.Pop();
        }
    }
}