namespace Nustache.Core
{
    public interface IContext
    {
        object GetValue(string name);
        object Current { get; set; }
    }
}