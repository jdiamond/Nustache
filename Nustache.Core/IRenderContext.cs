namespace Nustache.Core
{
    public interface IRenderContext
    {
        void Write(string text);
        object GetValue(string name);
        object CurrentValue { get; set; }
    }
}