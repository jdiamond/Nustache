using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Web.Script.Serialization;
using System.Text.RegularExpressions;

namespace Nustache.Core.Tests.Mustache_Spec
{
    /// <summary>
    /// Strongly typed wrapper for a suite of tests from the 
    /// official mustache spec (https://github.com/mustache/spec)
    /// </summary>
    public class MustacheSpec
    {
        private readonly string rawSpecs;
        private readonly JObject jsonSpecs;

        public MustacheSpec(string name)
        {
            rawSpecs = File.ReadAllText(String.Format("../../../spec/specs/{0}.json", name));
            jsonSpecs = JObject.Parse(rawSpecs);
            this.Tests = jsonSpecs["tests"].ToArray();
            this.StronglyTypedExamples = GetSpecs(this.Tests);

            this.MustacheTests = this.Tests.Select(test =>
                new MustacheTest(name, test, this.StronglyTypedExamples))
                .ToList();
        }

        public JToken[] Tests { get; set; }

        public static JObject GetSpecs(JToken[] specDefinition)
        {
            JObject specJson = new JObject();
            foreach (var test in specDefinition.Where(x => x["data"].Any()))
            {
                var name = Sanitize(test["name"].ToString());
                specJson[name] =
                    JObject.Parse(StripReservedWords(test["data"].ToString()));
                    
            }
            return specJson;
        }

        private static string StripReservedWords(string s)
        {
            s = new Regex(@"\{\s*[#\/\^]?string\s*\}\}").Replace(s, eval => eval.Value.Replace("string", "string1"));
            s = new Regex(@"\{\s*[#\/\^]?bool\s*\}\}").Replace(s, eval => eval.Value.Replace("bool", "bool1"));

            return s.Replace("\"string\"", "\"string1\"")
                    .Replace("\"bool\"", "\"bool1\"");
        }

        private static string Sanitize(string name)
        {
            foreach (var remove in new[] { " ", "-", "(", ")" })
            {
                name = name.Replace(remove, "");
            }

            return name;
        }

        public JObject StronglyTypedExamples { get; set; }
        public List<MustacheTest> MustacheTests { get; set; }

        /// <summary>
        /// Strongly typed wrapper for an individual test from the 
        /// official mustache spec (https://github.com/mustache/spec)
        /// </summary>
        public class MustacheTest
        {
            private JToken test;

            public MustacheTest(string specName, JToken test, JObject stronglyTypedExamples)
            {
                this.SpecName = specName;
                this.SanitizedName = Sanitize(test["name"].ToString());
                this.test = test;
                this.Template = StripReservedWords(test["template"].ToString());

                if (stronglyTypedExamples.Children().Any())
                {
                    if (stronglyTypedExamples[SanitizedName] != null)
                        this.Example =
                            new JavaScriptSerializer().DeserializeObject(stronglyTypedExamples[SanitizedName].ToString());
                    else
                        this.Example = new object();

                    var stronglyTypedClass = Assembly.GetExecutingAssembly().GetType(
                            Generate_Classes_from_JSON.FullTestClassName(specName, this.SanitizedName));
                    if (stronglyTypedClass != null)
                    {
                        this.StronglyTypedExample = Activator.CreateInstance(
                            stronglyTypedClass,
                            stronglyTypedExamples[this.SanitizedName]);
                    }
                }
                else
                {
                    this.StronglyTypedExample = new object();
                    this.Example = new object();
                }
            }

            public string Name { get { return test["name"].ToString(); } }
            public string SanitizedName { get; set; }
            public JToken Partials { get { return test["partials"]; } }
            public string Template { get; private set; }
            public string Description { get { return test["desc"].ToString(); } }
            public string Expected { get { return test["expected"].ToString(); } }
            public object Example { get; private set; }
            public object StronglyTypedExample { get; private set; }

            public override string ToString()
            {
                return Name;
            }

            public string SpecName { get; set; }
        }
    }
}
