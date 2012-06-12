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

        public object GetValue(string path)
        {
            if (path == ".")
            {
                return _dataStack.Peek();
            }

            foreach (var data in _dataStack)
            {
                if (data != null)
                {
                    var value = GetValueFromPath(data, path);

                    if (!ReferenceEquals(value, ValueGetter.NoValue))
                    {
                        return value;
                    }
                }
            }

            return null;
        }

        private static object GetValueFromPath(object data, string path)
        {
            var names = path.Split('.');

            foreach (var name in names)
            {
                data = ValueGetter.GetValue(data, name);

                if (data == null || ReferenceEquals(data, ValueGetter.NoValue))
                {
                    break;
                }
            }

            return data;
        }

        public IEnumerable<object> GetValues(string path)
        {
            object value = GetValue(path);

            if (value is bool)
            {
                if ((bool)value)
                {
                    yield return value;
                }
            }
            else if (value is string)
            {
                if (!string.IsNullOrEmpty((string)value))
                {
                    yield return value;
                }
            }
            else if (value is IDictionary) // Dictionaries also implement IEnumerable
                                           // so this has to be checked before it.
            {
                if (((IDictionary)value).Count > 0)
                {
                    yield return value;
                }
            }
            else if (value is IEnumerable)
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

                if (template != null)
                {
					// push the included template on the stack so that internally defined templates can be resolved properly later.
					// designed to pass test Describe_Template_Render.It_can_include_templates_over_three_levels_with_external_includes()
					this.Push(template, null);
                    template.Render(this);
					this.Pop();
                }
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