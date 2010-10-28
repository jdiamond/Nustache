namespace Nustache.Core
{
    public class TemplateInclude : Part
    {
        private readonly string _name;

        public TemplateInclude(string name)
        {
            _name = name;
        }

        public override void Render(RenderContext context)
        {
            context.Include(_name);
        }

        #region Boring stuff

        public bool Equals(TemplateInclude other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._name, _name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(TemplateInclude)) return false;
            return Equals((TemplateInclude)obj);
        }

        public override int GetHashCode()
        {
            return (_name != null ? _name.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return string.Format("TemplateInclude(\"{0}\")", _name);
        }

        #endregion
    }
}