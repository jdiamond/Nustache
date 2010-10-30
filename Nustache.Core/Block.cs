namespace Nustache.Core
{
    public class Block : Section
    {
        public Block(string name, params Part[] parts)
            : base(name)
        {
            Load(parts);
        }

        public override void Render(RenderContext context)
        {
            foreach (var value in context.GetValues(Name))
            {
                context.Push(this, value);

                base.Render(context);

                context.Pop();
            }
        }

        public override string ToString()
        {
            return string.Format("Block(\"{0}\")", Name);
        }
    }
}