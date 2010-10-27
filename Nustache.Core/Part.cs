using System.IO;

namespace Nustache.Core
{
    public abstract class Part
    {
        public abstract void Render(TextWriter writer, IContext context);
    }
}