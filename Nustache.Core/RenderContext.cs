using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace Nustache.Core
{
    public delegate Template TemplateLocator(string name);

    public delegate TResult Lambda<TResult>();
    public delegate TResult Lambda<T, TResult>(T arg);

    public delegate string RenderFunc(RenderContext context);

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
        public string ActiveStartDelimiter { get; set; }
        public string ActiveEndDelimiter { get; set; }

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

        public RenderContext(RenderContext baseContext, TextWriter writer)
        {
            _sectionStack = baseContext._sectionStack;
            _dataStack = baseContext._dataStack;
            _writer = writer;
            _templateLocator = baseContext._templateLocator;
            _renderContextBehaviour = baseContext._renderContextBehaviour;
            _includeLevel = baseContext._includeLevel;
            _indent = baseContext._indent;
            _lineEnded = baseContext._lineEnded;
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

            string name;
            IList<object> arguments;
            IDictionary<string, object> options;

            Helpers.Parse(this, path, out name, out arguments, out options);

            if (Helpers.Contains(name))
            {
                var helper = Helpers.Get(name);

                return (HelperProxy)((fn, inverse) => helper(this, arguments, options, fn, inverse));
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
                for (int i = 0; i < names.Length; i++ )
                {
                    data = ValueGetter.GetValue(data, names[i]);

                    if (data == null || ReferenceEquals(data, ValueGetter.NoValue))
                    {
                        if (i > 0)
                        {
                            partialMatch = true;
                        }

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
            else if (value.GetType().ToString().Equals("Newtonsoft.Json.Linq.JValue"))
            {
                yield return value;
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
            else if (value is DataTable)
            {
                foreach (var item in ((DataTable)value).Rows)
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
            return IsTruthy(GetValue(path));
        }

        public bool IsTruthy(object value)
        {
            if (value == null)
            {
                return false;
            }
            
            if (value is bool)
            {
                return (bool)value;
            }
            
            if (value is string)
            {
                return !string.IsNullOrEmpty((string)value);
            }
            
            if (value is IEnumerable)
            {
                return ((IEnumerable)value).GetEnumerator().MoveNext();
            }

            if (value is DataTable)
            {
                return ((DataTable)value).Rows.Count > 0;
            }

            return true;
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
					// push the included template on the stack so that internally defined templates can be resolved properly later.
					// designed to pass test Describe_Template_Render.It_can_include_templates_over_three_levels_with_external_includes()
                    this.Enter(template);
                    template.Render(this);
                    this.Exit();
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

        public void Enter(Section section)
        {
            _sectionStack.Push(section);
        }

        public void Exit()
        {
            _sectionStack.Pop();
        }

        public void Push(object data)
        {
            _dataStack.Push(data);
        }

        public void Pop()
        {
            _dataStack.Pop();
        }
    }
}
