using System;

namespace Nustache.Core
{
    public class TemplateInclude : Part
    {
        private readonly string _name;
        private readonly string _indent;

        public TemplateInclude(string name, string indent = null)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            _name = name.Trim();
            _indent = indent;
        }

        public string Name { get { return _name; } }
        public string Indent { get { return _indent; } }

        public override void Render(RenderContext context)
        {
            context.Include(_name, _indent);
        }

        public override string Source()
        {
            return "{{> " + _name + "}}";
        }

        public override string ToString()
        {
            return string.Format("TemplateInclude(\"{0}\")", _name);
        }
    }
}