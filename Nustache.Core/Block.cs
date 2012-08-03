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
            bool checkLambda = true;
            foreach (var value in context.GetValues(Name))
            {
                if (checkLambda)
                {
                    checkLambda = false;
                    var lambda = value as Lambda;
                    if (lambda != null)
                    {
                        context.Write(lambda(InnerSource()).ToString());
                        return;
                    }
                }
                
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