using System;
using System.Collections.Generic;

namespace Nustache.Core
{
    public class Parser
    {
        public void Parse(Section section, IEnumerable<Part> parts)
        {
            if (section == null)
                throw new ArgumentNullException("section");

            var sectionStack = new Stack<Section>();
            sectionStack.Push(section);

            foreach (var part in parts)
            {
                section.Add(part);

                if (part is Section)
                {
                    sectionStack.Push(section);
                    section = (Section)part;
                }
                else if (part is EndSection)
                {
                    var endSection = (EndSection)part;

                    if (sectionStack.Count == 1)
                    {
                        throw new NustacheException(
                            string.Format(
                                "End section {0} does not match any start section!",
                                endSection.Name));
                    }

                    if (endSection.Name.Split()[0].Trim() != section.Name.Split()[0].Trim())
                    {
                        throw new NustacheException(
                            string.Format(
                                "End section {0} does not match start section {1}!",
                                endSection.Name,
                                section.Name));
                    }

                    section = sectionStack.Pop();
                }
            }
        }
    }
}