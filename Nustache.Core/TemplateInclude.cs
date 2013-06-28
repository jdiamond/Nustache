using System;

namespace Nustache.Core
{
    public class TemplateInclude : Part
    {
        private readonly string _name;

        public TemplateInclude(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            _name = name.Trim();
        }

        public string Name { get { return _name; } }

        public override void Render(RenderContext context)
        {
            context.Include(_name);
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