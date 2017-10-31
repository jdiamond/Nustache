using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Nustache.Core.Tests
{
    internal static class EnumerablePartExtensions
    {
        public static void IsEmpty(
            this IEnumerable<Part> actualParts)
        {
            Assert.That(actualParts.ToArray(), Is.Empty);
        }

        public static void IsEqualTo(
            this IEnumerable<Part> actualParts,
            params Part[] expectedParts)
        {
            Assert.That(actualParts.ToArray(), Is.EqualTo(expectedParts)
                                                 .Using(new PartComparer()));
        }
    }
}