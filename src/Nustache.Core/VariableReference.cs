using System;
using System.Text.RegularExpressions;

namespace Nustache.Core
{
    public class VariableReference : Part
    {
        private static readonly Regex _notEscapedRegex = new Regex(@"^\{\s*(.+?)\s*\}$");
        private readonly string _path;
        private readonly bool _escaped;

        public VariableReference(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            _path = path;

            var match = _notEscapedRegex.Match(path);
            _escaped = !match.Success;

            if (match.Success)
            {
                _path = match.Groups[1].Value;
            }

            if (_path.StartsWith("&"))
            {
                _escaped = false;
                _path = _path.Substring(1).Trim();
            }
        }

        public string Path { get { return _path; } }
        public bool Escaped { get { return _escaped; } }

        public override void Render(RenderContext context)
        {
            var value = context.GetValue(_path);

            var lambda = CheckValueIsDelegateOrLambda(value);
            if(lambda != null) 
            {
                var lambdaResult = lambda().ToString();

                lambdaResult = _escaped
                    ? context.HtmlEncoder(lambdaResult.ToString())
                    : lambdaResult.ToString(); 

                using (System.IO.TextReader sr = new System.IO.StringReader(lambdaResult))
                {
                    Template template = new Template();
                    template.StartDelimiter = "{{";
                    template.EndDelimiter = "}}";

                    template.Load(sr);
                    context.Enter(template);
                    template.Render(context);
                    context.Exit();

                    return;
                }
            }

            var helper = value as HelperProxy;

            if (helper != null)
            {
                helper(data => { }, data => { });
            }
            else if (value != null)
            {
                context.Write(_escaped
                    ? context.HtmlEncoder(value.ToString())
                    : value.ToString());
            }
        }

        public Lambda<object> CheckValueIsDelegateOrLambda(object value)
        {
            var lambda = value as Lambda<object>;
            if(lambda != null) return lambda;

            if (value is Delegate && !(value is HelperProxy))
            {
                var delegateValue = (Delegate)value;
                return (Lambda<object>)(() => (object)delegateValue.DynamicInvoke());
            }

            return null;
        }

        public override string Source()
        {
            return "{{" + (!Escaped ? "&" : "") + _path + "}}";
        }

        public override string ToString()
        {
            return string.Format("VariableReference(\"{0}\")", _path);
        }
    }
}
