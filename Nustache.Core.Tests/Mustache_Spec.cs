using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using YamlDotNet.RepresentationModel.Serialization;

namespace Nustache.Core.Tests
{
    [TestFixture]
    public class Mustache_Spec
    {
        [Test]
        [TestCaseSource("Comments")]
        [TestCaseSource("Delimiters")]
        [TestCaseSource("Interpolation")]
        [TestCaseSource("Inverted")]
        [TestCaseSource("Partials")]
        [TestCaseSource("Sections")]
        public void AllTests(string name, Dictionary<object, object> data, string template, Dictionary<object, string> partials, string expected)
        {
            FixData(data);

            var actual = Render.StringToString(
                template,
                data,
                partial => {
                    var t = new Template();
                    t.Load(new StringReader(partials[partial]));
                    return t;
                });

            Assert.AreEqual(expected, actual);
        }

        private void FixData(Dictionary<object, object> data)
        {
            // YamlDotNet returns all values as strings. Parse the one value that matters to the tests.
            if (data.ContainsKey("power"))
            {
                data["power"] = double.Parse((string)data["power"]);
            }
        }

        public IEnumerable<ITestCaseData> Comments() { return GetTestCases("comments"); }
        public IEnumerable<ITestCaseData> Delimiters() { return GetTestCases("delimiters"); }
        public IEnumerable<ITestCaseData> Interpolation() { return GetTestCases("interpolation"); }
        public IEnumerable<ITestCaseData> Inverted() { return GetTestCases("inverted"); }
        public IEnumerable<ITestCaseData> Partials() { return GetTestCases("partials"); }
        public IEnumerable<ITestCaseData> Sections() { return GetTestCases("sections"); }

        public IEnumerable<ITestCaseData> GetTestCases(string file)
        {
            var text = File.ReadAllText(string.Format("../../../spec/specs/{0}.yml", file));
            var doc = new Deserializer().Deserialize<SpecDoc>(new StringReader(text));

            return doc.tests
                .Select(test => new TestCaseData(test.name, test.data, test.template, test.partials, test.expected)
                .SetName(file + ": " + test.name));
        }
    }

    public class SpecDoc
    {
        public string overview { get; set; }
        public SpecTest[] tests  { get; set; }
    }

    public class SpecTest
    {
        public string name { get; set; }
        public string desc { get; set; }
        public Dictionary<object, object> data { get; set; }
        public string template { get; set; }
        public Dictionary<object, string> partials { get; set; }
        public string expected { get; set; }
    }
}
