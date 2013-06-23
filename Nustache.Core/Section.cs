using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using System.Linq;

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
            foreach (var part in _parts)
            {
                part.Render(context);
            }
        }

        protected string InnerSource()
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

        internal override Expression Compile(CompileContext context)
        {
            var parts = _parts.Select(part => part.Compile(context))
                .Where(part => part != null)
                .ToList();
            return Concat(parts);
        }

        protected Expression Concat(IEnumerable<Expression> expressions)
        {
            var builder = Expression.Variable(typeof(StringBuilder), "builder");

            var blockExpressions = new List<Expression>();
            blockExpressions.Add(Expression.Assign(builder, Expression.New(typeof(StringBuilder))));
            blockExpressions.AddRange(expressions.Select(item => 
                    Expression.Call(builder, typeof(StringBuilder).GetMethod("Append", new Type[] { typeof(string) }), item)));
            blockExpressions.Add(
                    Expression.Call(builder, typeof(StringBuilder).GetMethod("ToString", new Type[0])));

            return Expression.Block(
                new [] { builder },
                blockExpressions
            );
        }
    }
}