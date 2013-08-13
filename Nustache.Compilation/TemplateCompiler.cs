using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Nustache.Core;

namespace Nustache.Compilation
{
    public static class TemplateCompiler
    {
        /// <summary>
        /// Compiles the template into a Lambda function which can be executed later.
        /// </summary>
        /// <typeparam name="T">The type which the template will render against.  Missing
        /// properties/methods/fields will result in CompilationExceptions.</typeparam>
        /// <param name="templateLocator">The delegate to use to locate templates for inclusion.</param>
        /// <returns>A lambda expression representing the compiled template.</returns>
        public static Func<T, string> Compile<T>(this Template template, TemplateLocator templateLocator) where T : class
        {
            var param = Expression.Parameter(typeof(T), "data");

            var context = new CompileContext(template, typeof(T), param, templateLocator);

            var expression = Compile(template, context);

            return (Expression.Lambda<Func<T, string>>(expression, param)).Compile();
        }

        public static Expression Compile(this Template template, CompileContext context)
        {
            var visitor = new CompilePartVisitor(context);
            visitor.Visit(template);
            return visitor.Result();
        }

        /// <summary>
        /// Compiles the template into a Lambda function which can be executed later.
        /// This version allows reflective compilation of templates.
        /// </summary>
        /// <param name="compileFor">The type to compile the template for.Missing
        /// properties/methods/fields will result in CompilationExceptions.</param>
        /// <param name="templateLocator">The delegate to use to locate templates for inclusion.</param>
        /// <returns>A lambda expression representing the compiled template.  The lambda takes and object
        /// and immediately casts it to <paramref name="compileFor"/>.</returns>
        public static Func<object, string> Compile(this Template template, Type compileFor, TemplateLocator templateLocator)
        {
            var param = Expression.Parameter(typeof(object), "data");

            var context = new CompileContext(template, compileFor,
                Expression.Convert(param, compileFor), templateLocator);

            var expression = Compile(template, context);

            return (Expression.Lambda<Func<object, string>>(expression, param)).Compile();
        }
    }
}
