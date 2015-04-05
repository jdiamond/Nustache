
using System;
using System.IO;

namespace Nustache.Core
{
    public class Block : Section
    {
        public Block(string name, params Part[] parts)
            : base(name)
        {
            Load(parts);
        }

        public override void Render(RenderContext context)
        {
            var value = context.GetValue(Name);

            var lambda = value as Lambda;

            if (lambda != null)
            {
                RenderFunc render = c =>
                {
                    var textWriter = new StringWriter();
                    var lambdaContext = new RenderContext(context, textWriter);
                    RenderParts(lambdaContext);
                    return textWriter.GetStringBuilder().ToString();
                };

                context.Write(lambda(InnerSource(), context, render).ToString());

                return;
            }

            var helper = value as HelperProxy;

            if (helper != null)
            {
                helper(data =>
                {
                    context.Enter(this);
                    context.Push(data);

                    RenderParts(context);

                    context.Pop();
                    context.Exit();
                }, data =>
                {
                    if (Inverse != null)
                    {
                        context.Enter(Inverse);
                        context.Push(data);

                        Inverse.RenderParts(context);

                        context.Pop();
                        context.Exit();
                    }
                });

                return;
            }

            foreach (var item in context.GetValues(Name))
            {
                context.Enter(this);
                context.Push(item);

                base.Render(context);

                context.Pop();
                context.Exit();
            }
        }

        public override string ToString()
        {
            return string.Format("Block(\"{0}\")", Name);
        }
    }
}