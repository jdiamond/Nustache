using System.Collections.Generic;

namespace Nustache.Core
{
    public class Parser
    {
        public IEnumerable<Part> Parse(IEnumerable<Part> parts)
        {
            var containerStack = new Stack<Container>();
            Container container = null;

            foreach (var part in parts)
            {
                if (container != null)
                {
                    container.Add(part);
                }
                else
                {
                    if (!(part is Container))
                    {
                        yield return part;
                    }
                }

                if (part is Container)
                {
                    containerStack.Push(container);
                    container = (Container)part;
                }
                else if (part is EndSection)
                {
                    var endSection = (EndSection)part;

                    if (container == null)
                    {
                        throw new NustacheException(
                            string.Format(
                                "End section {0} does not match any start section!",
                                endSection.Name));
                    }

                    if (endSection.Name != container.Name)
                    {
                        throw new NustacheException(
                            string.Format(
                                "End section {0} does not match start section {1}!",
                                endSection.Name,
                                container.Name));
                    }

                    var lastStartSection = containerStack.Pop();

                    if (lastStartSection == null)
                    {
                        yield return container;
                    }

                    container = lastStartSection;
                }
            }
        }
    }
}