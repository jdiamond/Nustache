namespace Nustache.Core
{
    public class RenderContextBehaviour
    {
        public bool RaiseExceptionOnDataContextMiss { get; private set; }

        public static RenderContextBehaviour GetDefaultRenderContextBehaviour()
        {
            return new RenderContextBehaviour() {RaiseExceptionOnDataContextMiss = false}; 
        }
        public static RenderContextBehaviour GetRenderContextBehaviour(bool raiseExceptionOnContextMiss)
        {
            return new RenderContextBehaviour() { RaiseExceptionOnDataContextMiss = raiseExceptionOnContextMiss };
        }
    }
}