using System;
using System.Collections.Generic;
using System.Text;

namespace Nustache.Core
{
    public class Section : Part
    {
        private readonly string _name;
        private readonly List<Part> _parts = new List<Part>();
        private readonly Dictionary<string, TemplateDefinition> _templateDefinitions =
            new Dictionary<string, TemplateDefinition>();

        public Section(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }

        public IEnumerable<Part> Parts { get { return _parts; } }

        public Section Inverse { get; set; }

        public void Load(IEnumerable<Part> parts)
        {
            foreach (var part in parts)
            {
                Add(part);
            }
        }

        public void Add(Part part)
        {
            if (part is TemplateDefinition)
            {
                var templateDefinition = (TemplateDefinition)part;
                _templateDefinitions.Add(templateDefinition.Name, templateDefinition);
            }
            else
            {
                _parts.Add(part);
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
            RenderParts(context);
        }

        public void RenderParts(RenderContext context)
        {
            foreach (var part in _parts)
            {
                part.Render(context);
            }
        }

        public string InnerSource()
        {
            var sb = new StringBuilder();
            foreach (var part in Parts)
            {
                if (!(part is EndSection))
                {
                    sb.Append(part.Source());
                }
            }
            return sb.ToString();
        }

        public override string Source() 
        {
            return "{{#" + _name + "}}" + InnerSource() + "{{/" + _name + "}}";
        }
    }
}