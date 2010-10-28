using System.Collections.Generic;

namespace Nustache.Core
{
    public class StartSection : Part
    {
        private readonly string _name;
        private readonly List<Part> _children;
        private readonly Dictionary<string, TemplateDefinition> _templateDefinitions =
            new Dictionary<string, TemplateDefinition>();

        public StartSection(string name, params Part[] children)
        {
            _name = name;
            _children = new List<Part>(children);
        }

        public string Name
        {
            get { return _name; }
        }

        public IEnumerable<Part> Children { get { return _children; } }

        public void Add(Part child)
        {
            if (child is TemplateDefinition)
            {
                var templateDefinition = (TemplateDefinition)child;
                _templateDefinitions.Add(templateDefinition.Name, templateDefinition);
            }
            else
            {
                _children.Add(child);
            }
        }

        public TemplateDefinition GetTemplateDefinition(string name)
        {
            TemplateDefinition templateDefinition;
            _templateDefinitions.TryGetValue(name, out templateDefinition);
            return templateDefinition;
        }

        public override void Render(RenderContext context)
        {
            foreach (var value in context.GetValues(_name))
            {
                context.PushData(value);

                foreach (var child in _children)
                {
                    child.Render(context);
                }

                context.PopData();
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