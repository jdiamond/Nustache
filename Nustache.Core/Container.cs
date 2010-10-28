using System.Collections.Generic;

namespace Nustache.Core
{
    public class Container : Part
    {
        private readonly string _name;
        private readonly List<Part> _children = new List<Part>();
        private readonly Dictionary<string, TemplateDefinition> _templateDefinitions =
            new Dictionary<string, TemplateDefinition>();

        public Container(string name)
        {
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }

        public IEnumerable<Part> Children { get { return _children; } }

        public void Load(IEnumerable<Part> parts)
        {
            foreach (var part in parts)
            {
                Add(part);
            }
        }

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
            foreach (var child in _children)
            {
                child.Render(context);
            }
        }
    }
}