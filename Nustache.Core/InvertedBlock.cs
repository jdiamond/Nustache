using System.Linq.Expressions;
using Nustache.Core.Compilation;
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
            if (context.GetValues(Name).GetEnumerator().MoveNext()) {
                return;
            }

            context.Push(this, true);

            base.Render(context);

            context.Pop();
        }

        internal override System.Linq.Expressions.Expression Compile(CompileContext context)
        {
            return context.GetInnerExpressions(Name, value =>
            {
                // the logic here is really confusing, but since GetInnerExpressions does 
                // the truthiness check, it is basically the same as Block.Compile

                var expression = base.Compile(context);

                return expression;
            }, invert: true);
        }

        public override string ToString()
        {
            return string.Format("InvertedBlock(\"{0}\")", Name);
        }
    }
}
