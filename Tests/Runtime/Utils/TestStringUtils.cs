using DialogueSystem.Utils;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystemRuntimeTests
{
    /// <summary>
    /// Tests StringUtils
    /// </summary>
    public class TestStringUtils
    {
        [Test]
        public void TestSplitCommand()
        {
            Command dialogueCommand = new(CommandType.Dialogue, ":");
            Command escape = new(CommandType.Escape, "\\");
            List<Command> commands = new()
            {
                dialogueCommand, escape, new(CommandType.Comment, "//"), new (CommandType.NodeStart, "-"),
            };

            string text = "Hello: World! \\: escape // comment";

            (string before, string after, string next, Command command) = StringUtils.SplitCommand(text, escape, commands);

            Assert.AreEqual("Hello", before);
            Assert.AreEqual(" World! : escape ", after);
            Assert.AreEqual("// comment", next);
            Assert.AreEqual(dialogueCommand, command);

            // try without an after input
            text = "Hello: World!";

            (before, after, next, command) = StringUtils.SplitCommand(text, escape, commands);
            Assert.AreEqual("Hello", before);
            Assert.AreEqual(" World!", after);
            Assert.AreEqual("", next);
            Assert.AreEqual(dialogueCommand, command);

            // try without an command
            text = "Hello!!!";

            (before, after, next, command) = StringUtils.SplitCommand(text, escape, commands);
            Assert.AreEqual("Hello!!!", before);
            Assert.AreEqual("", after);
            Assert.AreEqual("", next);
            Assert.AreEqual(null, command);

            // try with just escapes
            text = "Hello\\:\\\\";

            (before, after, next, command) = StringUtils.SplitCommand(text, escape, commands);
            Assert.AreEqual("Hello:\\", before);
            Assert.AreEqual("", after);
            Assert.AreEqual("", next);
            Assert.AreEqual(null, command);

            text = "Hello\\:\\:\\:";

            (before, after, next, command) = StringUtils.SplitCommand(text, escape, commands);
            Assert.AreEqual("Hello:::", before);
            Assert.AreEqual("", after);
            Assert.AreEqual("", next);
            Assert.AreEqual(null, command);
        }

        [Test]
        public void TestSplitCommandEmptyInput()
        {
            Command dialogueCommand = new(CommandType.Dialogue, ":");
            Command escape = new(CommandType.Escape, "\\");
            List<Command> commands = new()
            {
                dialogueCommand, escape, new(CommandType.Comment, "//"), new (CommandType.NodeStart, "-"),
            };

            string text = "";

            (string before, string after, string next, Command command) = StringUtils.SplitCommand(text, escape, commands);

            Assert.AreEqual("", before);
            Assert.AreEqual("", after);
            Assert.AreEqual("", next);
            Assert.AreEqual(null, command);


        }

        [Test]
        public void TestSplitThrowsException()
        {
            Command dialogueCommand = new(CommandType.Dialogue, ":");
            Command escape = new(CommandType.Escape, "\\");
            List<Command> commands = new()
            {
                dialogueCommand, escape, new(CommandType.Comment, "//"), new (CommandType.NodeStart, "-"),
            };

            string text = "Hello: World \\ unescaped";

            Assert.Throws<StringUtilsException>(() =>
            {
                (string before, string after, string next, Command command) = StringUtils.SplitCommand(text, escape, commands);
            });

            Assert.Throws<ArgumentNullException>(() =>
            {
                // try with null
                (string before, string after, string next, Command command) = StringUtils.SplitCommand(null, escape, commands);
            });
        }

        [Test]
        public void TestSplitRespectsSortOrder()
        {
            Command c1 = new(CommandType.Dialogue, ":");
            Command c2 = new(CommandType.IdStart, "::");
            Command c3 = new(CommandType.IdEnd, ":::");

            Command escape = new(CommandType.Escape, "\\");
            List<Command> commands = new()
            {
                c3, c2, c1, escape,
            };

            string text = "Hello:::World";
            (string before, string after, string next, Command command) = StringUtils.SplitCommand(text, escape, commands);

            Assert.AreEqual(c3, command);
            Assert.AreEqual("Hello", before);
            Assert.AreEqual("World", after);
            Assert.AreEqual("", next);

        }

        [Test]
        public void TestIsOneWordWithoutSymbols()
        {
            string invalid1 = "word-";
            string valid1 = "word";
            string invalid2 = "words are";
            string valid2 = "yay";

            Assert.IsTrue(StringUtils.IsOneWordWithoutSymbols(valid1));
            Assert.IsTrue(StringUtils.IsOneWordWithoutSymbols(valid2));

            Assert.IsFalse(StringUtils.IsOneWordWithoutSymbols(invalid1));
            Assert.IsFalse(StringUtils.IsOneWordWithoutSymbols(invalid2));
        }

        [Test]
        public void TestIndexOfBefore()
        {
            string text = "abcabcabc";

            // occurrences
            Assert.AreEqual(0, StringUtils.IndexOfBefore(text, "abc", 3));
            Assert.AreEqual(0, StringUtils.IndexOfBefore(text, "abc", 4));
            Assert.AreEqual(0, StringUtils.IndexOfBefore(text, "abc", 5));
            Assert.AreEqual(3, StringUtils.IndexOfBefore(text, "ab", 5));
            Assert.AreEqual(3, StringUtils.IndexOfBefore(text, "a", 5));
            Assert.AreEqual(6, StringUtils.IndexOfBefore(text, "a", 7));
            Assert.AreEqual(6, StringUtils.IndexOfBefore(text, "abc", 100));


            // non occurrences
            Assert.AreEqual(-1, StringUtils.IndexOfBefore(text, "d", 7));
            Assert.AreEqual(-1, StringUtils.IndexOfBefore(text, "abc", 2));
            Assert.AreEqual(-1, StringUtils.IndexOfBefore(text, "abc", 0));
            Assert.AreEqual(-1, StringUtils.IndexOfBefore(text, "a", -1));
            Assert.AreEqual(-1, StringUtils.IndexOfBefore(text, "abc", -100));

        }

        [Test]
        public void TestIndexOfAfter()
        {
            string text = "abcabcabc";

            // occurrences
            Assert.AreEqual(0, StringUtils.IndexOfAfter(text, "abc", -1));
            Assert.AreEqual(3, StringUtils.IndexOfAfter(text, "abc", 0));
            Assert.AreEqual(3, StringUtils.IndexOfAfter(text, "abc", 1));
            Assert.AreEqual(3, StringUtils.IndexOfAfter(text, "abc", 2));
            Assert.AreEqual(6, StringUtils.IndexOfAfter(text, "abc", 3));
            Assert.AreEqual(6, StringUtils.IndexOfAfter(text, "abc", 5));
            Assert.AreEqual(8, StringUtils.IndexOfAfter(text, "c", 7));
            Assert.AreEqual(7, StringUtils.IndexOfAfter(text, "bc", 6));
            Assert.AreEqual(1, StringUtils.IndexOfAfter(text, "bc", -100));


            // non occurrences
            Assert.AreEqual(-1, StringUtils.IndexOfAfter(text, "d", 6));
            Assert.AreEqual(-1, StringUtils.IndexOfAfter(text, "abc", 6));
            Assert.AreEqual(-1, StringUtils.IndexOfAfter(text, "abc", 100));

        }

        [Test]
        public void TestUnboundedSubstring()
        {
            string text = "abcdefghi";

            Assert.AreEqual("abc", StringUtils.UnboundedSubstring(text, 0, 3));
            Assert.AreEqual("abc", StringUtils.UnboundedSubstring(text, -1, 3));
            Assert.AreEqual("abc", StringUtils.UnboundedSubstring(text, -100, 3));

            Assert.AreEqual("d", StringUtils.UnboundedSubstring(text, 3, 4));
            Assert.AreEqual("e", StringUtils.UnboundedSubstring(text, 4, 5));
            Assert.AreEqual("f", StringUtils.UnboundedSubstring(text, 5, 6));

            Assert.AreEqual("ghi", StringUtils.UnboundedSubstring(text, 6, 9));
            Assert.AreEqual("ghi", StringUtils.UnboundedSubstring(text, 6, 10));
            Assert.AreEqual("ghi", StringUtils.UnboundedSubstring(text, 6, 100));
        }
    }
}
