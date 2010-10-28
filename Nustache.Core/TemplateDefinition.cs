namespace Nustache.Core
{
    public class TemplateDefinition : StartSection
    {
        public TemplateDefinition(string name)
            : base(name)
        {
        }

        public override void Render(RenderContext context)
        {
        }

        public Template GetTemplate()
        {
            var template = new Template();
            template.Load(Children);
            return template;
        }

        #region Boring stuff

        public bool Equals(TemplateDefinition other)
        {
            return base.Equals(other);
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