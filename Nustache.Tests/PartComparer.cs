using System.Collections;
using Nustache.Core;

namespace Nustache.Tests
{
    class PartComparer : IComparer
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

            if (x.GetType() == typeof(TemplateInclude))
            {
                return ((TemplateInclude)x).Name.CompareTo(((TemplateInclude)y).Name);
            }

            if (x.GetType() == typeof(VariableReference))
            {
                return ((VariableReference)x).Name.CompareTo(((VariableReference)y).Name);
            }

            return -1;
        }
    }
}