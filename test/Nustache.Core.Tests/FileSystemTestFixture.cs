using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace Nustache.Core.Tests
{
    public class FileSystemTestFixture
    {
        private List<string> _filesToDelete;

        protected string CreateEmptyFile()
        {
            string path = Path.GetTempFileName();
            _filesToDelete.Add(path);
            return path;
        }

        protected string CreateFile(string text)
        {
            string path = Path.GetTempFileName();
            _filesToDelete.Add(path);
            File.WriteAllText(path, text);
            return path;
        }

        [SetUp]
        public void BeforeEach()
        {
            _filesToDelete = new List<string>();
        }

        [TearDown]
        public void AfterEach()
        {
            _filesToDelete.ForEach(File.Delete);
        }
    }
}