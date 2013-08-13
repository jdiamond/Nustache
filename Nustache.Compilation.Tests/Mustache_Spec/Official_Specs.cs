using System.IO;
using NUnit.Framework;
using Nustache.Compilation;

namespace Nustache.Core.Tests.Mustache_Spec
{
    [TestFixture]
    public class Official_Specs
    {
        private static MustacheSpec.MustacheTest[] SectionTests() { return GetSpecs("sections"); }

        [Test, TestCaseSource("SectionTests")]
        public void Sections(MustacheSpec.MustacheTest test) { RunMustacheSpecs(test); }

        private static MustacheSpec.MustacheTest[] CommentTests() { return GetSpecs("comments"); }

        [Test, TestCaseSource("CommentTests")]
        public void Comments(MustacheSpec.MustacheTest test) { RunMustacheSpecs(test); }

        // TODO: support changing delimiters
        //private static MustacheSpec.MustacheTest[] DelimiterTests() { return GetSpecs("delimiters"); }

        //[Test, TestCaseSource("DelimiterTests")]
        //public void Delimiters(MustacheSpec.MustacheTest test) { RunMustacheSpecs(test); }

        private static MustacheSpec.MustacheTest[] InterpolationTests() { return GetSpecs("interpolation"); }

        [Test, TestCaseSource("InterpolationTests")]
        public void Interpolation(MustacheSpec.MustacheTest test) { RunMustacheSpecs(test); }

        private static MustacheSpec.MustacheTest[] InvertedTests() { return GetSpecs("inverted"); }

        [Test, TestCaseSource("InvertedTests")]
        public void Inverted(MustacheSpec.MustacheTest test) { RunMustacheSpecs(test); }

        private static MustacheSpec.MustacheTest[] PartialsTests() { return GetSpecs("partials"); }

        [Test, TestCaseSource("PartialsTests")]
        public void Partials(MustacheSpec.MustacheTest test) { RunMustacheSpecs(test); }

        // uncomment this to regenerate the specs from the mustache spec submodule
        //[Test]
        public void Generate()
        {
            Generate_Classes_from_JSON.Run();
        }

        private static void RunMustacheSpecs(MustacheSpec.MustacheTest test)
        {
            TemplateLocator testDataTemplateLocator = name =>
            {
                if (test.Partials != null && test.Partials[name] != null)
                {
                    var template = new Template();
                    template.Load(new StringReader(test.Partials[name].ToString()));
                    return template;
                };

                return null;
            };

            var rendered = Render.StringToString(test.Template, test.Example, testDataTemplateLocator);
            Assert.AreEqual(test.Expected, rendered, "JSON object rendering failed for " + test.Description);

            rendered = Render.StringToString(test.Template, test.StronglyTypedExample, testDataTemplateLocator);
            Assert.AreEqual(test.Expected, rendered, "Strongly typed rendering failed for " + test.Description);

            var templ = new Template();
            templ.Load(new StringReader(test.Template));

            if (!test.Name.ToLower().Contains("context miss") &&
                !test.Name.ToLower().Contains("broken chain") &&
                !(test.SpecName == "inverted" && (test.Name == "List" || test.Name == "Context")))
            {
                var compiledTemplate = templ.Compile(
                    test.StronglyTypedExample != null ? test.StronglyTypedExample.GetType() : typeof(object),
                    testDataTemplateLocator);
                rendered = compiledTemplate(test.StronglyTypedExample);
                Assert.AreEqual(test.Expected, rendered, "Compiled Template rendering failed for " + test.Description);
            }
            else
            {
                bool gotException = false;
                try
                {
                    var compiledTemplate = templ.Compile(
                        test.StronglyTypedExample != null ? test.StronglyTypedExample.GetType() : typeof(object),
                        testDataTemplateLocator);
                }
                catch (Compilation.CompilationException)
                {
                    gotException = true;
                }

                Assert.IsTrue(gotException, "Expected test to throw a compilation exception for an invalid template");
            }
        }

        private static MustacheSpec.MustacheTest[] GetSpecs(string name)
        {
            var spec = new MustacheSpec(name);

            return spec.MustacheTests.ToArray();
        }

    }
}
