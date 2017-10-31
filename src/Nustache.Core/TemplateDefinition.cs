namespace Nustache.Core
{
    public class TemplateDefinition : Template // This derives from Template so that it can
                                               // be returned from a TemplateLocator.
                                               // Should we make it implement an interface instead?
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