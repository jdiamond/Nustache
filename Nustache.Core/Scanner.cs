using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Nustache.Core
{
    public class Scanner
    {
        private static readonly Regex _markerRegex = new Regex(@"\{\{([\{]?[^}]+?\}?)\}\}");
        private static readonly Regex _stripRegex = new Regex(@"\G[\r\t\v ]+\n");

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
                bool stripOutNewLine = false;

                marker = marker.Trim();

                if (marker[0] == '#')
                {
                    yield return new Block(marker.Substring(1));
                    stripOutNewLine = true;
                } 
                else if (marker[0] == '^') 
                {
                    yield return new InvertedBlock(marker.Substring(1));
                    stripOutNewLine = true;
                }
                else if (marker[0] == '<')
                {
                    yield return new TemplateDefinition(marker.Substring(1));
                    stripOutNewLine = true;
                }
                else if (marker[0] == '/')
                {
                    yield return new EndSection(marker.Substring(1));
                    stripOutNewLine = true;
                }
                else if (marker[0] == '>')
                {
                    yield return new TemplateInclude(marker.Substring(1));
                    stripOutNewLine = true;
                }
                else if (marker[0] != '!')
                {
                    yield return new VariableReference(marker.Trim());
                }

                i = m.Index + m.Length;

                Match s;
                if (stripOutNewLine && (s = _stripRegex.Match(template, i)).Success)
                    i += s.Length;
            }

            if (i < template.Length)
            {
                yield return new LiteralText(template.Substring(i));
            }
        }
    }
}