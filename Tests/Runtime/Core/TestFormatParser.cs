using DialogueSystem;
using NUnit.Framework;

namespace DialogueSystemRuntimeTests
{
    /// <summary>
    /// Ensure that the format parser is working as intended
    /// </summary>
    public class TestFormatParser
    {
        private FormatParser formatParser;

        [SetUp]
        public void SetUp()
        {
            formatParser = new();
        }

        [Test]
        public void TestSingleIdParse()
        {
            // check that the id string is accurate

            //                                            012345World11
            IdentifiedString test1 = formatParser.Format("Hello $id(World)!");

            Assert.IsTrue(test1.ContainsBounds("id"));
            Assert.AreEqual((6, 11), test1.GetBounds("id"));
            Assert.AreEqual("Hello World!", test1.GetText());

            IdentifiedString test2 = formatParser.Format("Hello $id (World)!");

            Assert.IsTrue(test2.ContainsBounds("id"));
            Assert.AreEqual((6, 11), test2.GetBounds("id"));
            Assert.AreEqual("Hello World!", test2.GetText());

            //                                            012345  6     789112
            IdentifiedString test3 = formatParser.Format("Hello \\$$id (World\\))!");

            Assert.IsTrue(test3.ContainsBounds("id"));
            Assert.AreEqual((7, 13), test3.GetBounds("id"));
            Assert.AreEqual("Hello $World)!", test3.GetText());


        }

        [Test]
        public void TestMultipleIdParse()
        {
            FormatParser formatParser = new();

            IdentifiedString test1 = formatParser.Format("Hello $shake (World) $bold (yay)!");
            Assert.IsTrue(test1.ContainsBounds("shake"));
            Assert.IsTrue(test1.ContainsBounds("bold"));

            //Hello World yay!
            Assert.AreEqual((6, 11), test1.GetBounds("shake"));
            Assert.AreEqual((12, 15), test1.GetBounds("bold"));

            IdentifiedString test2 = formatParser.Format("Hello $shake (World) $bold (yay)! $finale (Gimme your money \\$\\$) the end...\\$");
            Assert.IsTrue(test2.ContainsBounds("shake"));
            Assert.IsTrue(test2.ContainsBounds("bold"));
            Assert.IsTrue(test2.ContainsBounds("finale"));

            //Hello World yay! Gimme your money $$ ...
            Assert.AreEqual((6, 11), test2.GetBounds("shake"));
            Assert.AreEqual((12, 15), test2.GetBounds("bold"));
            Assert.AreEqual((17, 36), test2.GetBounds("finale"));
        }

        [Test]
        public void TestInvalidInput()
        {
            FormatParser formatParser = new();


            Assert.Throws<FormatParseException>(() =>
            {
                IdentifiedString test1 = formatParser.Format("Hello $id(There");
            });

            Assert.Throws<FormatParseException>(() =>
            {
                IdentifiedString test2 = formatParser.Format("Hello id (World)");
            });

            Assert.Throws<FormatParseException>(() =>
            {
                IdentifiedString test2 = formatParser.Format("Hello $id (World))");
            });

            Assert.Throws<FormatParseException>(() =>
            {
                IdentifiedString test2 = formatParser.Format("Hello $id id(World)");
            });

            Assert.Throws<FormatParseException>(() =>
            {
                IdentifiedString test2 = formatParser.Format("Hello $id-id(World)");
            });
        }
    }

}
