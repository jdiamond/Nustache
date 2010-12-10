using System;
using System.Text.RegularExpressions;
using System.Web;

namespace Nustache.Core
{
    public class VariableReference : Part
    {
        private readonly string _name;
      private readonly bool _escaped;
      private static readonly Regex Inner = new Regex(@"^\{(.+?)\}$");
      public VariableReference(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            _name = name;
        var match = Inner.Match(name);
        _escaped = !match.Success;
        if (match.Success)
        {
          _name = match.Groups[1].Value;
        }

        }

        public string Name { get { return _name; } }

        public override void Render(RenderContext context)
        {
            object value = context.GetValue(_name);

            if (value != null)
            {
              context.Write(_escaped
                ? HttpUtility.HtmlEncode( value.ToString())
                : value.ToString());
            }
        }

        public override string Source()
        {
            return "{{" + _name + "}}";
        }

        public override string ToString()
        {
            return string.Format("VariableReference(\"{0}\")", _name);
        }
    }
}