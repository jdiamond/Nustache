using System.Web;
using NUnit.Framework;

namespace Nustache.Core.Tests
{
    [TestFixture]
    public class Describe_Encoders
    {
        [Test]
        public void It_encodes_HTML_characters()
        {
            Assert.AreEqual(
                "&lt;foo&gt;&amp;bar;&lt;/foo&gt;",
                Encoders.HtmlEncode("<foo>&bar;</foo>"));
        }

        [Test]
        public void It_encodes_international_characters()
        {
            Assert.AreEqual(
                "I&#241;t&#235;rn&#226;ti&#244;n&#224;liz&#230;ti&#248;n",
                Encoders.HtmlEncode("Iñtërnâtiônàlizætiøn"));
        }

        [Test]
        public void It_encodes_russian_characters()
        {
            Assert.AreEqual(
                "&#1055;&#1088;&#1080;&#1074;&#1077;&#1090;, &#1082;&#1072;&#1082; &#1076;&#1077;&#1083;&#1072;",
                Encoders.HtmlEncode("Привет, как дела"));
        }

        [Test]
        public void How_does_SystemWebHttpUtilityHtmlEncode_work()
        {
            Assert.AreEqual(
                "&lt;foo&gt;&amp;bar;&lt;/foo&gt;",
                HttpUtility.HtmlEncode("<foo>&bar;</foo>"));
        }

        [Test]
        public void How_does_SystemWebHttpUtilityHtmlEncode_work_with_international_characters()
        {
            Assert.AreEqual(
                "I&#241;t&#235;rn&#226;ti&#244;n&#224;liz&#230;ti&#248;n",
                HttpUtility.HtmlEncode("Iñtërnâtiônàlizætiøn"));
        }

        [Test]
        public void How_does_SystemWebHttpUtilityHtmlEncode_work_with_russian_characters()
        {
            Assert.AreEqual(
                "Привет, как дела",
                HttpUtility.HtmlEncode("Привет, как дела"));
        }
    }
}