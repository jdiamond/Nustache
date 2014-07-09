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

            var helper = value as HelperProxy;

            if (helper != null)
            {
                helper(data => { }, data => { });
            }
            else if (value != null)
            {
                if (ValueFormatter.HasRegisteredFormatters)
                {
                    value = ValueFormatter.Format(value);
                }
                context.Write(_escaped
                    ? Encoders.HtmlEncode(value.ToString())
                    : value.ToString());
            }
        }

        public override string Source()
        {
            return "{{" + _path + "}}";
        }

        public override string ToString()
        {
            return string.Format("VariableReference(\"{0}\")", _path);
        }
    }
}