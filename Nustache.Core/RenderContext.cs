using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
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
        private readonly RenderContextBehaviour _renderContextBehaviour;
        private int _includeLevel;
        private string _indent;
        private bool _lineEnded;
        private readonly Regex _indenter = new Regex("\n(?!$)");

        public RenderContext(Section section, object data, TextWriter writer, TemplateLocator templateLocator, RenderContextBehaviour renderContextBehaviour = null) 
        {
            _sectionStack.Push(section);
            _dataStack.Push(data);
            _writer = writer;
            _templateLocator = templateLocator;
            _includeLevel = 0;

            _renderContextBehaviour = renderContextBehaviour ??
                                      RenderContextBehaviour.GetDefaultRenderContextBehaviour();
        }

        public object GetValue(string path)
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
                    bool partialMatch;
                    var value = GetValueFromPath(data, path, out partialMatch);

                    if (partialMatch) break;

                    if (!ReferenceEquals(value, ValueGetter.NoValue))
                    {
                        if (value is string)
                        {
                            var valueAsString = (string)value;
                            if (string.IsNullOrEmpty(valueAsString) && _renderContextBehaviour.RaiseExceptionOnEmptyStringValue)
                            {
                                throw new NustacheEmptyStringException(
                                    string.Format("Path : {0} is an empty string, RaiseExceptionOnEmptyStringValue : true.", path));
                            }
                        }
                        return value;
                    }
                }
            }

            if (Helpers.Contains(path))
            {
                return Helpers.Get(path);
            }

            if (_renderContextBehaviour.RaiseExceptionOnDataContextMiss)
            {
                throw new NustacheDataContextMissException(string.Format("Path : {0} is undefined, RaiseExceptionOnDataContextMiss : true.", path));
            }

            return null;
        }

        private static object GetValueFromPath(object data, string path, out bool partialMatch)
        {
            partialMatch = false;

            var value = ValueGetter.GetValue(data, path);

            if (value != null && !ReferenceEquals(value, ValueGetter.NoValue))
            {
                return value;
            }

            var names = path.Split('.');

            if (names.Length > 1)
            {
                foreach (var name in names)
                {
                    data = ValueGetter.GetValue(data, name);

                    if (data == null || ReferenceEquals(data, ValueGetter.NoValue))
                    {
                        partialMatch = true;
                        break;
                    }
                }

                return data;
            }

            return value;
        }

        public IEnumerable<object> GetValues(string path)
        {
            object value = GetValue(path);

            if (value == null)
            {
                if (_renderContextBehaviour.RaiseExceptionOnDataContextMiss)
                {
                    throw new NustacheDataContextMissException(string.Format("Path : {0} is undefined, RaiseExceptionOnDataContextMiss : true.", path));
                }
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

        public bool IsTruthy(string path)
        {
            return GetValues(path).GetEnumerator().MoveNext();
        }

        public void WriteLiteral(string text)
        {
            if (_indent != null)
            {
                text = _indenter.Replace(text, m => "\n" + _indent);
            }

            Write(text);

            _lineEnded = text.Length > 0 && text[text.Length - 1] == '\n';
        }

        public void Write(string text)
        {
            // Sometimes a literal gets cut in half by a variable and needs to be indented.
            if (_indent != null && _lineEnded)
            {
                text = _indent + text;
                _lineEnded = false;
            }

            _writer.Write(text);
        }

        public void Include(string templateName, string indent)
        {
            if (_includeLevel >= IncludeLimit)
            {
                throw new NustacheException(
                    string.Format("You have reached the include limit of {0}. Are you trying to render infinitely recursive templates or data?", IncludeLimit));
            }

            _includeLevel++;

            var oldIndent = _indent;
            _indent = (_indent ?? "") + (indent ?? "");

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

            _indent = oldIndent;

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