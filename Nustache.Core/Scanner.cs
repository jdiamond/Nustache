using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Nustache.Core
{
    public class Scanner
    {
        private static readonly Regex _markerRegex = new Regex(@"\{\{([\{]?[^}]+?\}?)\}\}");

        public IEnumerable<Part> Scan(string template)
        {
            int i = 0;
            Match m;

            while ((m = _markerRegex.Match(template, i)).Success)
            {
                string literal = template.Substring(i, m.Index - i);

                if (literal != "")
                {
                    yield return new LiteralText(literal);
                }

                string marker = m.Groups[1].Value;

                if (marker[0] == '#')
                {
                    yield return new Block(marker.Substring(1));
                } 
                else if (marker[0] == '^') 
                {
                    yield return new InvertedBlock(marker.Substring(1));
                }
                else if (marker[0] == '<')
                {
                    yield return new TemplateDefinition(marker.Substring(1));
                }
                else if (marker[0] == '/')
                {
                    yield return new EndSection(marker.Substring(1));
                }
                else if (marker[0] == '>')
                {
                    yield return new TemplateInclude(marker.Substring(1));
                }
                else if (marker[0] != '!')
                {
                    yield return new VariableReference(m.Groups[1].Value);
                }

                i = m.Index + m.Length;
            }

            if (i < template.Length)
            {
                yield return new LiteralText(template.Substring(i));
            }
        }
    }
}