using System;

namespace Nustache.Core
{
    public class EndSection : Part
    {
        private readonly string _name;

        public EndSection(string name)
        {
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }

        public override void Render(IRenderContext context)
        {
        }

        #region Boring stuff

        public bool Equals(EndSection other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._name, _name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(EndSection)) return false;
            return Equals((EndSection)obj);
        }

        public override int GetHashCode()
        {
            return (_name != null ? _name.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return string.Format("EndSection(\"{0}\")", _name);
        }

        #endregion
    }
}