namespace Nustache.Mvc
{
    public class NustachViewEngineOptions
    {
        public string Extension { get; set; }
        public NustacheViewEngineRootContext RootContext { get; set; }
    }

    public enum NustacheViewEngineRootContext
    {
        ViewData,
        Model
    }
}