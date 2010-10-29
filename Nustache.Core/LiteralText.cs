using System;

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

        public override void Render(RenderContext context)
        {
            context.Write(_text);
        }

        #region Boring stuff

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(LiteralText)) return false;
            return Equals((LiteralText)obj);
        }

        public bool Equals(LiteralText other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._text, _text);
        }

        public override int GetHashCode()
        {
            return _text.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("LiteralText(\"{0}\")", _text);
        }

        #endregion
    }
}