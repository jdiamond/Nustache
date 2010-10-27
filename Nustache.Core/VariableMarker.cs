namespace Nustache.Core
{
    public class VariableMarker : Part
    {
        private readonly string _name;

        public VariableMarker(string name)
        {
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

        public bool Equals(VariableMarker other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._name, _name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(VariableMarker)) return false;
            return Equals((VariableMarker)obj);
        }

        public override int GetHashCode()
        {
            return (_name != null ? _name.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return string.Format("VariableMarker(\"{0}\")", _name);
        }

        #endregion
    }
}