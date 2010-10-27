using System.Collections.Generic;

namespace Nustache.Core
{
    public class Parser
    {
        public IEnumerable<Part> Parse(IEnumerable<Part> parts)
        {
            var enumerator = parts.GetEnumerator();

            while (enumerator.MoveNext())
            {
                var part = enumerator.Current;

                var startSection = part as StartSection;

                if (startSection != null)
                {
                    while (enumerator.MoveNext())
                    {
                        var child = enumerator.Current;

                        startSection.Children.Add(child);

                        var endSection = child as EndSection;

                        if (endSection != null)
                        {
                            if (endSection.Name != startSection.Name)
                            {
                                throw new NustacheException(
                                    string.Format(
                                        "End section {0} does not match start section {1}!",
                                        endSection.Name,
                                        startSection.Name));
                            }

                            yield return startSection;

                            break;
                        }
                    }
                }
                else
                {
                    yield return part;
                }
            }
        }
    }
}