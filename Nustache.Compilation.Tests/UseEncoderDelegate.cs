using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using Nustache.Compilation;
using Nustache.Core;

namespace Nustache.Compilation.Tests
{
    [TestFixture]
    public class Encoder_Delegate_Usage
    {
        class TemplateData
        {
            public string Value { get; set; }
        }

        [Test]
        public void ReplacingHtmlEncodeWorksForCompiledTemplates()
        {
            // replace the default encoder with one that wraps the input in "--" 
            Encoders.HtmlEncoder encoder = (input) => "--" + input + "--";
            Encoders.HtmlEncode = encoder;

            var template = Template("{{Value}}");
            var compiled = template.Compile<TemplateData>(null);


            var inputText = "Some cool text";
            var data = new TemplateData()
            {
                Value = inputText
            };

            var expectedOutput = encoder(inputText);

            Assert.AreEqual(expectedOutput, compiled(data));

            // reset the used HTML encoder to default
            Encoders.HtmlEncode = Encoders.DefaultHtmlEncode;
        }

        private Template Template(string text)
        {
            var template = new Template();
            template.Load(new StringReader(text));
            return template;
        }
    }
}
