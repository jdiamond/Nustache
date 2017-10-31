
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

            var lambda = CheckValueIsDelegateOrLambda(value);

            if (lambda != null)
            {
                var lambdaResult = lambda(InnerSource()).ToString();
                using(TextReader sr = new StringReader(lambdaResult))
                {
                    var template = new Template();
                    template.StartDelimiter = context.ActiveStartDelimiter;
                    template.EndDelimiter = context.ActiveEndDelimiter;

                    template.Load(sr);
                    context.Enter(template);
                    template.Render(context);
                    context.Exit();
                }

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

        public Lambda<string, object> CheckValueIsDelegateOrLambda(object value)
        {
            var lambda = value as Lambda<string, object>;
            if (lambda != null) return lambda;

            if (value is Delegate && !(value is HelperProxy))
            {
                var delegateValue = (Delegate)value;
                return (Lambda<string, object>)((body) => (object)delegateValue.DynamicInvoke(body));
            }

            return null;
        }

        public override string ToString()
        {
            return string.Format("Block(\"{0}\")", Name);
        }
    }
}