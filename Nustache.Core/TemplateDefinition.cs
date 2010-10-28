namespace Nustache.Core
{
    public class TemplateDefinition : Section
    {
        public TemplateDefinition(string name)
            : base(name)
        {
        }

        #region Boring stuff

        public bool Equals(TemplateDefinition other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Name, Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as TemplateDefinition);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("TemplateDefinition(\"{0}\")", Name);
        }

        #endregion
    }
}