
using DialogueSystem;
using NUnit.Framework;
using System.IO;
using System.Reflection;

namespace DialogueSystemRuntimeTests
{
    /// <summary>
    /// Tests reading files from given paths
    /// </summary>
    public class TestFileReader
    {
        private FileReader reader;
        [SetUp]
        public void SetUp()
        {
            reader = new();
        }

        [Test]
        public void TestRead()
        {

            string filePath = Path.Combine(Paths.GetTestFilesDirectory(), "test-file-reader.txt");

            string text = reader.ReadFile(filePath);

            Assert.AreEqual("Hello World!", text);
        }
    }
}
