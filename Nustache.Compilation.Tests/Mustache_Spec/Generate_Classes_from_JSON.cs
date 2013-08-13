using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using System.IO;

namespace Nustache.Core.Tests.Mustache_Spec
{
    public static class Generate_Classes_from_JSON
    {
        public static void Run()
        {
            var specs = new [] { "comments", "delimiters", "interpolation", "inverted", "partials", "sections" };

            foreach (var spec in specs)
            {
                var specDefinition = new MustacheSpec(spec);

                if (specDefinition.StronglyTypedExamples.Children().Any())
                {
                    var generator = new Xamasoft.JsonClassGenerator.JsonClassGenerator();
                    generator.SingleFile = true;
                    generator.TargetFolder = "../../Mustache_Spec/examples/";
                    generator.Namespace = FullTestClassNamespace(spec);
                    generator.MainClass = spec;
                    generator.Example = specDefinition.StronglyTypedExamples.ToString();
                    generator.ExplicitDeserialization = true;
                    generator.GenerateClasses();
                }
            }
        }

        public static string FullTestClassNamespace(string spec)
        {
            return "Nustache.Core.Tests.Mustache_Spec.Examples." + spec;
        }

        public static string FullTestClassName(string spec, string name)
        {
            return FullTestClassNamespace(spec) + "." + name;
        }
    }
}
