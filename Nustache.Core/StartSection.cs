using System.Collections;
using System.Collections.Generic;

namespace Nustache.Core
{
    public class StartSection : Part
    {
        private readonly string _name;
        private readonly List<Part> _children = new List<Part>();

        public StartSection(string name)
        {
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }

        public List<Part> Children { get { return _children; } }

        public override void Render(IRenderContext context)
        {
            object value = context.GetValue(_name);

            object currentValue = context.CurrentValue;

            foreach (var item in GetItems(value))
            {
                context.CurrentValue = item;

                foreach (var child in _children)
                {
                    child.Render(context);
                }
            }

            context.CurrentValue = currentValue;
        }

        private static IEnumerable<object> GetItems(object value)
        {
            if (value is bool)
            {
                if ((bool)value)
                {
                    yield return value;
                }
            }
            else if (value is IEnumerable && !(value is string))
            {
                foreach (var item in ((IEnumerable)value))
                {
                    yield return item;
                }
            }
            else if (value != null)
            {
                yield return value;
            }
        }

        #region Boring stuff

        public bool Equals(StartSection other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._name, _name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(StartSection)) return false;
            return Equals((StartSection)obj);
        }

        public override int GetHashCode()
        {
            return (_name != null ? _name.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return string.Format("StartSection(\"{0}\")", _name);
        }

        #endregion
    }
}