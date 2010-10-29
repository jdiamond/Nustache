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

        public override void Render(RenderContext context)
        {
            object value = context.GetValue(_name);

            if (value != null)
            {
                context.Write(value.ToString());
            }
        }

        #region Boring stuff

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(VariableReference)) return false;
            return Equals((VariableReference)obj);
        }

        public bool Equals(VariableReference other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._name, _name);
        }

        public override int GetHashCode()
        {
            return _name.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("VariableReference(\"{0}\")", _name);
        }

        #endregion
    }
}