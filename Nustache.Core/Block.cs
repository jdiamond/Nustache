
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
            var value = context.GetValue(Name);

            var lambda = value as Lambda;

            if (lambda != null)
            {
                context.Write(lambda(InnerSource()).ToString());
                return;
            }

            var helper = value as Helper;

            if (helper != null)
            {
                helper(context, null, null, ctx => base.Render(context));
                return;
            }

            foreach (var item in context.GetValues(Name))
            {
                context.Push(this, item);

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