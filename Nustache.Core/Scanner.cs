using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Text;

namespace Nustache.Core
{
    public class Scanner
    {
        private static readonly Regex _markerRegex = new Regex(@"\{\{([\{]?[^}]+?\}?)\}\}");
        // Remove standalone lines (lines which have only a non-variable expression on them)
        private static readonly Regex _standaloneRegex = new Regex(
            @"(^|\r?\n)[\r\t\v ]*({\{\s*[#\/!\<\>^]+[^}]+?\}\})[\r\t\v ]*(\r?\n|$)");
        // Special case for standalone lines that have multiple non-variable expressions with only newlines separating them
        // E.g.: {{#begin}}\n{{/end}}\n   (this is straight from the mustache specs)
        private static readonly Regex _standaloneSpecialCaseRegex = new Regex(
            @"(^|\r?\n)[\r\t\v ]*({\{\s*[#\/!\<\>^]+[^}]+?\}\})[\r\t\v\n ]*({\{\s*[#\/!\<\>^]+[^}]+?\}\})[\r\t\v ]*(\r?\n|$)");

        public IEnumerable<Part> Scan(string template)
        {
            int i = 0;
            Match m;

            // remove standalone expressions before parsing as this greatly simplifies things.  
            // See https://github.com/defunkt/mustache/blob/master/lib/mustache/parser.rb for how complex parsing is without this
            template = _standaloneRegex.Replace(template, match => match.Groups[1].Value + match.Groups[2].Value);
            template = _standaloneSpecialCaseRegex.Replace(template, match => match.Groups[1].Value + match.Groups[2].Value + match.Groups[3].Value);
            
            while (true)
            {
                if ((m = _markerRegex.Match(template, i)).Success)
                {
                    string literal = template.Substring(i, m.Index - i);

                    if (literal != "")
                    {
                        yield return new LiteralText(literal);
                    }

                    string marker = m.Groups[1].Value;

                    marker = marker.Trim();

                    if (marker[0] == '#')
                    {
                        yield return new Block(marker.Substring(1).Trim());
                    }
                    else if (marker[0] == '^')
                    {
                        yield return new InvertedBlock(marker.Substring(1).Trim());
                    }
                    else if (marker[0] == '<')
                    {
                        yield return new TemplateDefinition(marker.Substring(1).Trim());
                    }
                    else if (marker[0] == '/')
                    {
                        yield return new EndSection(marker.Substring(1).Trim());
                    }
                    else if (marker[0] == '>')
                    {
                        yield return new TemplateInclude(marker.Substring(1).Trim());
                    }
                    else if (marker[0] != '!')
                    {
                        yield return new VariableReference(marker.Trim());
                    }

                    i = m.Index + m.Length;
                }
                else
                {
                    break;
                }
            }

            if (i < template.Length)
            {
                yield return new LiteralText(template.Substring(i));
            }
        }
    }
}