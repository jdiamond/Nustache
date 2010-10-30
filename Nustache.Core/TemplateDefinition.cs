namespace Nustache.Core
{
    public class TemplateDefinition : Section
    {
        public TemplateDefinition(string name)
            : base(name)
        {
        }

        public override string ToString()
        {
            return string.Format("TemplateDefinition(\"{0}\")", Name);
        }
    }
}