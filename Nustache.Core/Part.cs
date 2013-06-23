using System.Linq.Expressions;
using System.Text;
namespace Nustache.Core
{
    public abstract class Part
    {
        public abstract void Render(RenderContext context);

        internal abstract Expression Compile(CompileContext context);

        public abstract string Source();
    }
}