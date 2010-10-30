using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;
using Nustache.Core;

namespace nustache
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Usage: nustache.exe templatePath jsonPath outputPath");

                Environment.Exit(1);
            }

            var serializer = new JavaScriptSerializer();
            string json = File.ReadAllText(args[1]);
            var data = serializer.Deserialize<IDictionary<string, object>>(json);

            Render.FileToFile(args[0], data, args[2]);
        }
    }
}