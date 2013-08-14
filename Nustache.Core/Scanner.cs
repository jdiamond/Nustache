using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Nustache.Core
{
    public class Scanner
    {
        public IEnumerable<Part> Scan(string template)
        {
            var regex = MakeRegex("{{", "}}");
            var i = 0;
            var lineEnded = false;

            while (true)
            {
                Match m;

                if ((m = regex.Match(template, i)).Success)
                {
                    var leadingWhiteSpace = m.Groups[1];
                    var leadingLineEnd = m.Groups[2];
                    var marker = m.Groups[3].Value.Trim();
                    var trailingWhiteSpace = m.Groups[4];
                    var trailingLineEnd = m.Groups[5];

                    var previousLiteral = template.Substring(i, m.Index - i);

                    var isStandalone = (leadingLineEnd.Success || (lineEnded && m.Index == i)) && trailingLineEnd.Success;

                    Part part = null;

                    if (marker[0] == '=')
                    {
                        var delimiters = _delimitersRegex.Match(marker);

                        if (delimiters.Success)
                        {
                            var start = delimiters.Groups[1].Value;
                            var end = delimiters.Groups[2].Value;
                            regex = MakeRegex(start, end);
                        }
                    }
                    else if (marker[0] == '#')
                    {
                        part = new Block(marker.Substring(1).Trim());
                    }
                    else if (marker[0] == '^')
                    {
                        part = new InvertedBlock(marker.Substring(1).Trim());
                    }
                    else if (marker[0] == '<')
                    {
                        part = new TemplateDefinition(marker.Substring(1).Trim());
                    }
                    else if (marker[0] == '/')
                    {
                        part = new EndSection(marker.Substring(1).Trim());
                    }
                    else if (marker[0] == '>')
                    {
                        part = new TemplateInclude(marker.Substring(1).Trim());
                    }
                    else if (marker[0] != '!')
                    {
                        part = new VariableReference(marker.Trim());
                        isStandalone = false;
                    }

                    if (!isStandalone)
                    {
                        previousLiteral += leadingWhiteSpace;
                    }
                    else
                    {
                        previousLiteral += leadingLineEnd;
                    }

                    if (previousLiteral != "")
                    {
                        yield return new LiteralText(previousLiteral);
                    }

                    if (part != null)
                    {
                        yield return part;
                    }

                    i = m.Index + m.Length;

                    if (!isStandalone)
                    {
                        i -= trailingWhiteSpace.Length;
                    }

                    lineEnded = trailingLineEnd.Success;
                }
                else
                {
                    break;
                }
            }

            if (i < template.Length)
            {
                var remainingLiteral = template.Substring(i);

                yield return new LiteralText(remainingLiteral);
            }
        }

        private static readonly Regex _delimitersRegex = new Regex(@"^=\s*(\S+)\s+(\S+)\s*=$");

        private static Regex MakeRegex(string start, string end)
        {
            return new Regex(
                @"((^|\r?\n)?[\r\t\v ]*)" +
                Regex.Escape(start) +
                @"([\{]?[^" + Regex.Escape(end.Substring(0, 1)) + @"]+?\}?)" +
                Regex.Escape(end) +
                @"([\r\t\v ]*(\r?\n|$)?)"
            );
        }
    }
}
