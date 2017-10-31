namespace Nustache.Core
{
    public class InvertedBlock : Section
    {
        public InvertedBlock(string name, params Part[] parts)
            : base(name)
        {
            Load(parts);
        }

        public override void Render(RenderContext context)
        {
            // According to mustache(5), we only need to render an inverted section if the value is truthful 
            if (context.IsTruthy(Name)) {
                return;
            }

            context.Enter(this);

            base.Render(context);

            context.Exit();
        }

        public override string ToString()
        {
            return string.Format("InvertedBlock(\"{0}\")", Name);
        }
    }
}
