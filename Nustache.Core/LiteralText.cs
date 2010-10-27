namespace Nustache.Core
{
    public class LiteralText : Part
    {
        private readonly string _text;

        public LiteralText(string text)
        {
            _text = text;
        }

        public override void Render(IRenderContext context)
        {
            context.Write(_text);
        }

        #region Boring stuff

        public bool Equals(LiteralText other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._text, _text);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(LiteralText)) return false;
            return Equals((LiteralText)obj);
        }

        public override int GetHashCode()
        {
            return (_text != null ? _text.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return string.Format("LiteralText(\"{0}\")", _text);
        }

        #endregion
    }
}