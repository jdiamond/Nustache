using System;
using System.Collections;
using System.Collections.Generic;

namespace Nustache.Core
{
    public delegate void Helper(RenderContext context, IList<object> arguments, IDictionary<string, object> options, RenderBlock fn, RenderBlock inverse);
    public delegate void HelperProxy(RenderBlock fn, RenderBlock inverse);
    public delegate void RenderBlock(object data);

    public static class Helpers
    {
        private static readonly Dictionary<string, Helper> DefaultHelpers = new Dictionary<string, Helper>();
        private static readonly Dictionary<string, Helper> CustomHelpers = new Dictionary<string, Helper>();

        public static void Register(string name, Helper helper)
        {
            CustomHelpers.Add(name, helper);
        }

        public static bool Contains(string name)
        {
            return CustomHelpers.ContainsKey(name) || DefaultHelpers.ContainsKey(name);
        }

        public static Helper Get(string name)
        {
            return CustomHelpers.ContainsKey(name) ? CustomHelpers[name] : DefaultHelpers[name];
        }

        public static void Clear()
        {
            CustomHelpers.Clear();
        }

        static Helpers()
        {
            DefaultHelpers["each"] = EachHelper;
            DefaultHelpers["if"] = IfHelper;
            DefaultHelpers["unless"] = UnlessHelper;
            DefaultHelpers["with"] = WithHelper;
        }

        public static void EachHelper(RenderContext context, IList<object> arguments, IDictionary<string, object> options, RenderBlock fn, RenderBlock inverse)
        {
            foreach (var item in (IEnumerable) arguments[0])
            {
                fn(item);
            }
        }

        public static void IfHelper(RenderContext context, IList<object> arguments, IDictionary<string, object> options, RenderBlock fn, RenderBlock inverse)
        {
            var value = arguments[0];

            if (context.IsTruthy(value))
            {
                fn(null);
            }
            else
            {
                inverse(null);
            }
        }

        public static void UnlessHelper(RenderContext context, IList<object> arguments, IDictionary<string, object> options, RenderBlock fn, RenderBlock inverse)
        {
            IfHelper(context, arguments, options, inverse, fn);
        }

        public static void WithHelper(RenderContext context, IList<object> arguments, IDictionary<string, object> options, RenderBlock fn, RenderBlock inverse)
        {
            fn(arguments[0]);
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

                    var splits2 = arg.Split(new[] { '=' }, 2);
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
    }
}
