using System;

namespace Nustache.Core
{
    public class VariableReference : Part
    {
        private readonly string _name;

        public VariableReference(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            _name = name;
        }

        public string Name { get { return _name; } }

        public override void Render(RenderContext context)
        {
            object value = context.GetValue(_name);

            if (value != null)
            {
                context.Write(value.ToString());
            }
        }

        public override string ToString()
        {
            return string.Format("VariableReference(\"{0}\")", _name);
        }
    }
}