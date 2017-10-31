
namespace Nustache.Core
{
    public class RenderContextBehaviour
    {
        public bool RaiseExceptionOnDataContextMiss { get; set; }
        public bool RaiseExceptionOnEmptyStringValue { get; set; }

        public Encoders.HtmlEncoder HtmlEncoder { get; set; }

        public static RenderContextBehaviour GetDefaultRenderContextBehaviour()
        {
            return new RenderContextBehaviour
                        {
                            RaiseExceptionOnDataContextMiss = false, 
                            RaiseExceptionOnEmptyStringValue  = false
                        }; 
        }
    }
}