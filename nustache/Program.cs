using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;
using System.Xml;
using Nustache.Core;

namespace nustache
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Usage: nustache.exe templatePath dataPath outputPath");

                Environment.Exit(1);
            }

            var templatePath = args[0];
            var dataPath = args[1];
            var outputPath = args[2];

            var ext = Path.GetExtension(dataPath);

            object data = null;

            if (string.Equals(ext, ".js", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(ext, ".json", StringComparison.OrdinalIgnoreCase))
            {
                var serializer = new JavaScriptSerializer();
                string json = File.ReadAllText(dataPath);
                data = serializer.Deserialize<IDictionary<string, object>>(json);
            }
            else if (string.Equals(ext, ".xml", StringComparison.OrdinalIgnoreCase))
            {
                var doc = new XmlDocument();
                doc.Load(dataPath);
                data = doc.DocumentElement;
            }
            else
            {
                Console.WriteLine("Sorry, dataPath must end in .js, .json, or .xml!");

                Environment.Exit(1);
            }

            Render.FileToFile(templatePath, data, outputPath);
        }
    }
}