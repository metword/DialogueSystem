using DialogueSystem;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace DialogueSystemRuntimeTests
{
    
    public class TestIdentifiedString 
    {
        [Test]
        public void TestAddBounds()
        {
            IdentifiedString idString = new IdentifiedString("Hello World!");

            idString.AddBounds("Hello", 0, 4);

            idString.AddBounds("World", 6, 12);

            Assert.Throws<ArgumentOutOfRangeException>(() => idString.AddBounds("key", 0, 13));

            Assert.Throws<ArgumentException>(() => idString.AddBounds("Hello", 1, 2));

            Assert.Throws<ArgumentException>(() => idString.AddBounds("New", 2, 1));

            idString.AddBounds("Woot!", 0, 0);
        }

        [Test]
        public void TestGetBounds()
        {
            IdentifiedString idString = new IdentifiedString("Hello World!");

            idString.AddBounds("Hello", 0, 4);

            idString.AddBounds("World", 6, 12);

            (int start, int end) = idString.GetBounds("Hello");

            Assert.AreEqual(0, start);
            Assert.AreEqual(4, end);

            (start, end) = idString.GetBounds("World");

            Assert.AreEqual(6, start);
            Assert.AreEqual(12, end);

            Assert.Throws<KeyNotFoundException>(() => idString.GetBounds("no"));
        }

        [Test]
        public void TestContainsBounds()
        {
            IdentifiedString idString = new IdentifiedString("Hello World!");

            idString.AddBounds("Hello", 0, 4);

            idString.AddBounds("World", 6, 12);

            Assert.IsTrue(idString.ContainsBounds("Hello"));

            Assert.IsTrue(idString.ContainsBounds("World"));

        }
    }

    
}
