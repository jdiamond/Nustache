using System.Collections.Generic;

namespace Nustache.Core
{
    public delegate void Helper(RenderContext context, IList<object> arguments, IDictionary<string, object> options, InnerHelper fn);
    public delegate void InnerHelper(RenderContext context);

    public static class Helpers
    {
        private static Dictionary<string, Helper> _helpers = new Dictionary<string, Helper>();

        public static void Register(string name, Helper helper)
        {
            _helpers.Add(name, helper);
        }

        public static bool Contains(string name)
        {
            return _helpers.ContainsKey(name);
        }

        public static Helper Get(string name)
        {
            return _helpers[name];
        }
    }
}
