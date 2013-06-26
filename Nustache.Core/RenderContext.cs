using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

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

            if (section != null)
            {
                section.Data = data;
            }
        }

        /// <summary>
        /// get the binding object in the render context by section name.
        /// </summary>
        /// <param name="sectionName">section name</param>
        /// <returns></returns>
        public object GetSectionData(string sectionName)
        {
            foreach (var item in this._sectionStack)
            {
                if (string.Equals(item.Name, sectionName, StringComparison.CurrentCultureIgnoreCase))
                {
                    return item.Data;
                }
            }
            return null;
        }

        internal object GetValue(string path)
        {
            if (path == ".")
            {
                object peekedObject = _dataStack.Peek();
                if (peekedObject as XmlElement != null)
                {
                    return ((XmlElement)peekedObject).InnerText;
                }
                return peekedObject;
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

        private object GetValueFromPath(object data, string path)
        {
            var value = ValueGetter.GetValue(data, path, this);

            if (value != null && !ReferenceEquals(value, ValueGetter.NoValue))
            {
                return value;
            }

            var names = path.Split('.');

            foreach (var name in names)
            {
                data = ValueGetter.GetValue(data, name, this);

                if (data == null || ReferenceEquals(data, ValueGetter.NoValue))
                {
                    break;
                }
            }

            return data;
        }

        internal IEnumerable<object> GetValues(string path)
        {
            object value = GetValue(path);

            if (value == null)
            {
                yield break;
            }
            else if (value is bool)
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
            else if (GenericIDictionaryUtil.IsInstanceOfGenericIDictionary(value))
            {
                if ((value as IEnumerable).GetEnumerator().MoveNext())
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
            else
            {
                yield return value;
            }
        }

        internal void Write(string text)
        {
            _writer.Write(text);
        }

        internal void Include(string templateName)
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
                    template.Render(this);
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

        internal void Push(Section section, object data)
        {
            _sectionStack.Push(section);
            _dataStack.Push(data);
            if (section != null)
            {
                section.Data = data;
            }
        }

        internal void Pop()
        {
            _sectionStack.Pop();
            _dataStack.Pop();
        }
    }
}