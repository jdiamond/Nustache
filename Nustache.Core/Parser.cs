using System.Collections.Generic;

namespace Nustache.Core
{
    public class Parser
    {
        public IEnumerable<Part> Parse(IEnumerable<Part> parts)
        {
            var sectionStack = new Stack<StartSection>();
            StartSection startSection = null;

            foreach (var part in parts)
            {
                if (startSection != null)
                {
                    startSection.Add(part);
                }
                else
                {
                    if (!(part is StartSection))
                    {
                        yield return part;
                    }
                }

                if (part is StartSection)
                {
                    sectionStack.Push(startSection);
                    startSection = (StartSection)part;
                }
                else if (part is EndSection)
                {
                    var endSection = (EndSection)part;

                    if (startSection == null)
                    {
                        throw new NustacheException(
                            string.Format(
                                "End section {0} does not match any start section!",
                                endSection.Name));
                    }

                    if (endSection.Name != startSection.Name)
                    {
                        throw new NustacheException(
                            string.Format(
                                "End section {0} does not match start section {1}!",
                                endSection.Name,
                                startSection.Name));
                    }

                    var lastStartSection = sectionStack.Pop();

                    if (lastStartSection == null)
                    {
                        yield return startSection;
                    }

                    startSection = lastStartSection;
                }
            }
        }
    }
}