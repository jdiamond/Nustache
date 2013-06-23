using System;
using System.Linq.Expressions;
using System.Text;

namespace Nustache.Core
{
    public class LiteralText : Part
    {
        private readonly string _text;

        public LiteralText(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            _text = text;
        }

        public string Text { get { return _text; } }

        public override void Render(RenderContext context)
        {
            context.Write(_text);
        }

        public override string Source()
        {
            return _text;
        }

        public override string ToString()
        {
            return string.Format("LiteralText(\"{0}\")", _text);
        }

        internal override Expression Compile(CompileContext context)
        {
            return Expression.Constant(_text, typeof(string));
        }
    }
}