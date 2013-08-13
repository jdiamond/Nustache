using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Nustache.Core;

namespace Nustache.Compilation
{
    public class CompilePartVisitor : PartVisitor
    {
        private readonly CompileContext context;
        private List<Expression> parts = new List<Expression>();
        private Expression result = null;

        public CompilePartVisitor(CompileContext context)
        {
            this.context = context;
        }

        public Expression Result()
        {
            return result;
        }

        public void Visit(Section section)
        {
            foreach (var part in section.Parts)
            {
                part.Accept(this);
            }
            
            result = Concat(parts.Where(part => part != null));
        }

        protected Expression Concat(IEnumerable<Expression> expressions)
        {
            var builder = Expression.Variable(typeof(StringBuilder), "builder");

            var blockExpressions = new List<Expression>();
            blockExpressions.Add(Expression.Assign(builder, Expression.New(typeof(StringBuilder))));
            blockExpressions.AddRange(expressions.Select(item => 
                    Expression.Call(builder, typeof(StringBuilder).GetMethod("Append", new Type[] { typeof(string) }), item)));
            blockExpressions.Add(
                    Expression.Call(builder, typeof(StringBuilder).GetMethod("ToString", new Type[0])));

            return Expression.Block(
                new [] { builder },
                blockExpressions
            );
        }

        public void Visit(Block block)
        {
            parts.Add(context.GetInnerExpressions(block.Name, value =>
            {
                context.Push(block, value);

                if (typeof(Lambda).BaseType.IsAssignableFrom(value.Type))
                {
                    return
                        Expression.Call(
                            Expression.Call(value, value.Type.GetMethod("Invoke"),
                                Expression.Constant(block.InnerSource())),
                            typeof(object).GetMethod("ToString"));
                }

                var visitor = new CompilePartVisitor(context);
                visitor.Visit((Section)block);
                var expression = CompoundExpression.NullCheck(value,
                    nullValue: "",
                    returnIfNotNull: visitor.Result());

                context.Pop();

                return expression;
            }));

        }

        public void Visit(LiteralText literal)
        {
            parts.Add(Expression.Constant(literal.Text, typeof(string)));
        }

        public void Visit(EndSection endSections)
        {
            
        }

        public void Visit(InvertedBlock invertedBlock)
        {
            parts.Add(context.GetInnerExpressions(invertedBlock.Name, value =>
            {
                // the logic here is really confusing, but since GetInnerExpressions does 
                // the truthiness check, it is basically the same as Block.Compile

                var visitor = new CompilePartVisitor(context);
                visitor.Visit((Section)invertedBlock);
                var expression = visitor.Result();

                return expression;
            }, invert: true));
        }

        public void Visit(TemplateInclude include)
        {
            parts.Add(context.Include(include.Name));
        }

        public void Visit(VariableReference variable)
        {
            var getter = context.CompiledGetter(variable.Path);
            getter = CompoundExpression.NullCheck(getter, "");
            getter = Expression.Call(getter, context.TargetType.GetMethod("ToString"));

            if (variable.Escaped)
            {
                parts.Add(Expression.Call(null, typeof(Encoders).GetMethod("DefaultHtmlEncode"), getter));
            }
            else
            {
                parts.Add(getter);
            }
        }
    }
}
