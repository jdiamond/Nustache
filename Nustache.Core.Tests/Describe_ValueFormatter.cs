using System.IO;
using NUnit.Framework;

namespace Nustache.Core.Tests
{
    [TestFixture]
    public class Describe_ValueFormatter : FileSystemTestFixture
    {
        [Test]
        public void It_can_format_object_with_value_formatter()
        {
            string decFormat = ".#"; //1 decimal
            var model = new
            {
                price = (decimal)55.4545458m,
            };

            ValueFormatter.Add<decimal>(val => val.ToString(decFormat));

            var output = Render.StringToString("{{price}}", model);
            var priceString = model.price.ToString(decFormat);
            Assert.AreEqual(priceString, output);
        }

    }
}