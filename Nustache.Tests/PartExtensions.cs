using NUnit.Framework;
using Nustache.Core;

namespace Nustache.Tests
{
    internal static class PartExtensions
    {
        public static void IsEqualTo(
            this Part actualPart,
            Part expectedPart)
        {
            Assert.That(actualPart, Is.EqualTo(expectedPart).Using(new PartComparer()));
        }
    }
}