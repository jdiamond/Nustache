using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
                partial =>
                {
                    if (partials.ContainsKey(partial))
                    {
                        var t = new Template();
                        t.Load(new StringReader(partials[partial]));
                        return t;
                    }
                    else
                    {
                        return null;
                    }
                });

            Assert.AreEqual(expected, actual);
        }

        private void FixData(Dictionary<object, object> data)
        {
            FixNumbers(data);
            FixFalseValues(data);
        }

        private void FixNumbers(Dictionary<object, object> data)
        {
            Visit(data,
                value => Regex.IsMatch(value, @"^\d+(\.\d+)?$"),
                value => double.Parse(value));
        }

        private void FixFalseValues(Dictionary<object, object> data)
        {
            Visit(data,
                value => value == "false",
                value => false);
        }

        private void Visit(object value, Func<string, bool> pred, Func<string, object> func)
        {
            if (value is List<object>)
            {
                var list = (List<object>)value;

                for (var i = 0; i < list.Count; i++)
                {
                    var val = list[i];

                    if (val is string && pred((string)val))
                    {
                        list[i] = func((string)val);
                    }
                    else
                    {
                        Visit(val, pred, func);
                    }
                }
            }
            else if (value is Dictionary<object, object>)
            {
                var dict = (Dictionary<object, object>)value;

                foreach (var key in dict.Keys.ToArray()) // Copy the array so we can modify it while looping.
                {
                    var val = dict[key];

                    if (val is string && pred((string)val))
                    {
                        dict[key] = func((string)val);
                    }
                    else
                    {
                        Visit(val, pred, func);
                    }
                }
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
            var deserializer = new Deserializer();
            var doc = deserializer.Deserialize<SpecDoc>(new StringReader(text));

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
