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
        }

        public string Path { get { return _path; } }
        internal bool Escaped { get { return _escaped; } }

        public override void Render(RenderContext context)
        {
            object value = context.GetValue(_path);

            if (value != null)
            {
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