using System.Collections.Generic;

namespace Nustache.Core
{
    public delegate void Helper(RenderContext context, IList<object> arguments, IDictionary<string, object> options, InnerHelper fn);
    public delegate void InnerHelper(object data);

    public static class Helpers
    {
        private static Dictionary<string, Helper> _helpers = new Dictionary<string, Helper>();

        public static void Register(string name, Helper helper)
        {
            _helpers.Add(name, helper);
        }

        public static void Parse(RenderContext ctx, string path, out string name, out IList<object> arguments, out IDictionary<string, object> options)
        {
            name = path;
            arguments = null;
            options = null;

            if (path.Contains(" "))
            {
                var splits = path.Split();
                name = splits[0];
                ParseArguments(ctx, splits, out arguments);
                ParseOptions(ctx, splits, out options);
            }
        }

        private static void ParseArguments(RenderContext ctx, string[] splits, out IList<object> arguments)
        {
            arguments = null;

            if (splits.Length > 1)
            {
                arguments = new List<object>(splits).GetRange(1, splits.Length - 1);

                for (var i = 0; i < arguments.Count; i++)
                {
                    var arg = (string)arguments[i];

                    if (arg[0] == '"')
                    {
                        arguments[i] = arg.Substring(1, arg.Length - 2);
                    }
                    else
                    {
                        arguments[i] = ctx.GetValue(arg);
                    }
                }
            }
        }

        private static void ParseOptions(RenderContext ctx, string[] splits, out IDictionary<string, object> options)
        {
            options = null;

            for (var i = 0; i < splits.Length; i++)
            {
                var arg = splits[i];

                if (arg.Contains("="))
                {
                    if (options == null)
                    {
                        options = new Dictionary<string, object>();
                    }

                    var splits2 = arg.Split(new[] {'='}, 2);
                    var key = splits2[0];
                    var val = splits2[1];

                    if (val[0] == '"')
                    {
                        options[key] = val.Substring(1, val.Length - 2);
                    }
                    else
                    {
                        options[key] = ctx.GetValue(val);
                    }
                }
            }
        }

        public static bool Contains(string name)
        {
            return _helpers.ContainsKey(name);
        }

        public static Helper Get(string name)
        {
            return _helpers[name];
        }

        public static void Clear()
        {
            _helpers.Clear();
        }
    }
}
