using System.Collections;
using Nustache.Core;

namespace Nustache.Core.Tests
{
    internal class PartComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x.GetType() == typeof(Block))
            {
                return ((Block)x).Name.CompareTo(((Block)y).Name);
            }

            if (x.GetType() == typeof(EndSection))
            {
                return ((EndSection)x).Name.CompareTo(((EndSection)y).Name);
            }

            if (x.GetType() == typeof(LiteralText))
            {
                return ((LiteralText)x).Text.CompareTo(((LiteralText)y).Text);
            }

            if (x.GetType() == typeof(TemplateDefinition))
            {
                return ((TemplateDefinition)x).Name.CompareTo(((TemplateDefinition)y).Name);
            }

            if (x.GetType() == typeof(TemplateInclude))
            {
                return ((TemplateInclude)x).Name.CompareTo(((TemplateInclude)y).Name);
            }

            if (x.GetType() == typeof(VariableReference))
            {
                return ((VariableReference)x).Path.CompareTo(((VariableReference)y).Path);
            }

            return -1;
        }
    }
}