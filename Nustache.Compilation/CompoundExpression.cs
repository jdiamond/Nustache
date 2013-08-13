using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Collections;

namespace Nustache.Compilation
{
    // TODO: The framework has a way of doing custom expressions. Use that.
    public static class CompoundExpression
    {
        public static Expression Enumerator(Func<Expression, Expression> itemCallback, Expression enumerable)
        {
            var enumerableInterface = enumerable.Type.Name == "IEnumerable`1" ? 
                enumerable.Type :
                enumerable.Type.GetInterface("IEnumerable`1");
            var listType = enumerableInterface.GetGenericArguments().First();
            var enumeratorMethod = enumerableInterface
                .GetMethod("GetEnumerator");

            var enumerator = Expression.Variable(typeof(IEnumerator<>)
                .MakeGenericType(listType), "enumerator");
            var list = Expression.Variable(typeof(List<>).MakeGenericType(typeof(string)), "list");
            var label = Expression.Label();
            var concatenatedString = Expression.Variable(typeof(string), "concatenatedString");

            var block = Expression.Block(
                new[] { enumerator, list, concatenatedString },
                Expression.IfThen(
                    Expression.ReferenceNotEqual(enumerable, Expression.Constant(null)),
                        Expression.Block(
                            Expression.Assign(list, Expression.New(list.Type)),
                            Expression.Assign(enumerator, Expression.Call(enumerable, enumeratorMethod)),
                            
                            Expression.Loop(
                                Expression.IfThenElse(
                                    Expression.Call(enumerator, typeof(IEnumerator).GetMethod("MoveNext")),
                                    
                                    Expression.Call(list, list.Type.GetMethod("Add"),
                                        itemCallback(Expression.Property(enumerator, "Current"))),
                                    
                                    Expression.Break(label)
                                ),
                                label
                            ),
                            
                            Expression.Assign(concatenatedString, Expression.Call(
                                typeof(string).GetMethod("Concat",
                                    new Type[] { typeof(IEnumerable<>).MakeGenericType(typeof(string)) }),
                                list))
                        )
                ),
                concatenatedString
            );
            return block;
        }

        internal static Expression NullCheck(Expression expression, string nullValue = "", Expression returnIfNotNull = null)
        {
            if (returnIfNotNull == null) returnIfNotNull = expression;

            if (!expression.Type.IsValueType)
            {
                return Expression.Condition(
                    Expression.ReferenceNotEqual(expression, Expression.Constant(null)),
                        returnIfNotNull,
                        Expression.Constant(nullValue));
            }
            else
            {
                return returnIfNotNull;
            }
        }
    }
}
