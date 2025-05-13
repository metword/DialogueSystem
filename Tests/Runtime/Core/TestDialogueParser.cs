using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DialogueSystem;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace DialogueSystemRuntimeTests
{
    /// <summary>
    /// Tests the Dialogue parser class
    /// </summary>
    public class TestDialogueParser
    {
        DialogueParser dialogueParser;

        [SetUp]
        public void SetUp()
        {
            dialogueParser = new();
        }

        [Test]
        public void TestInvalidInput()
        {
            Assert.Throws<DialogueParseException>(() =>
            {
                dialogueParser.Parse(null);
            }, "Did not throw or threw wrong exception");
        }

        [Test]
        public void TestParse()
        {
            string filePath = Path.Combine(Paths.GetTestFilesDirectory(), "sample-dialogue.ds");

            DialogueSequence sequence = dialogueParser.Parse(new FileReader().ReadFile(filePath));

            MockDialogueReader reader = new();

            sequence.AddDialogueReader(reader);

            Action onDisable = () =>
            {
                sequence.GetLineWithID<DialogueLine>("DialogueID").Toggle(false);
                sequence.GetLineWithID<OptionalLine>("OptionID").ToggleOption("OptionID", false); 
            };

            bool triggered = false;
            Action<string, string> dele = (s1, s2) =>
            {
                triggered = true;
            };

            sequence.SetFunction("Function", dele);

            sequence.SetFunction("DisableOption", onDisable);
            sequence.StartSequence();

            Assert.IsNull(reader.GetOption());
            Assert.AreEqual("Hello there how are you?", reader.GetLine().GetText(), "Did not get the correct first line");

            reader.NextLine();

            OptionalLine oLine = reader.GetOption();
            Dictionary<int, Option> options = oLine.GetOptions();

            Assert.AreEqual(2, options.Count, "Did not get correct number of options");

            bool option1 = false;
            string goodNodeName = null;
            bool option2 = false;
            foreach (Option option in options.Values)
            {
                if (option.Text == "Good")
                {
                    option1 = true;
                    goodNodeName = option.Node;
                }
                else if (option.Text == "Bad")
                {
                    option2 = true;
                }
            }

            Assert.NotNull(goodNodeName, "Option does not have node");

            Assert.IsTrue(option1, "Good option was not present");
            Assert.IsTrue(option2, "Bad option was not present");

            reader.NextOption(goodNodeName);

            Assert.AreEqual("Glad you're doing well!", reader.GetLine().GetText());

            reader.NextLine();

            Assert.AreEqual("Hope you have a great rest of your day $name(person)!", reader.GetLine().GetText());

            reader.NextLine();

            OptionalLine line = reader.GetOption();

            string optionDestination = line.GetOptions().Values.ToList()[0].Node;

            reader.NextOption(optionDestination);

            Assert.True(triggered, "Function was not triggered");

            Assert.AreEqual("woot woot!", reader.GetLine().GetText());

            reader.NextLine();

            Assert.AreEqual(reader.GetOption().GetEnabledOptions().Count, 2, "Should have 2 options since we toggled one");

            reader.NextOption(reader.GetOption().GetEnabledOptions()[0].Node);

            Assert.AreEqual(reader.GetLine().GetText(), "Bye!", "Text does not match up");
            Assert.AreEqual(reader.GetLine().GetSpeaker(), "Tom", "Speaker does not match up");

            reader.NextLine();

            Assert.AreEqual(reader.GetLine().GetText(), "Bye bye...", "Text does not match up");
            Assert.AreEqual(reader.GetLine().GetSpeaker(), "You", "Speaker does not match up");

            reader.NextLine();

            Line finalLine = reader.GetLine();

            Assert.Null(finalLine, "No more lines so parse should give a null line.");
        }


        [Test]
        public void TestLotsOfIndentation()
        {
            string filePath = Path.Combine(Paths.GetTestFilesDirectory(), "sample-lots-indent.ds");

            DialogueSequence sequence = dialogueParser.Parse(new FileReader().ReadFile(filePath));

            MockDialogueReader reader = new();

            sequence.AddDialogueReader(reader);

            sequence.StartSequence();

            reader.NextRandomOption();

            reader.NextRandomOption();

            reader.NextRandomOption();

            Assert.AreEqual("End Layer 3", reader.GetLine().GetText());

            reader.NextLine();

            Assert.AreEqual("End Layer 2", reader.GetLine().GetText());

            reader.NextLine();

            Assert.AreEqual("End Layer 1", reader.GetLine().GetText());

            reader.NextLine();

            Assert.IsNull(reader.GetLine());
        }

        /// <summary>
        /// Ensures that a mix of tabs and spaces hasn't been allowed to use
        /// </summary>
        [Test]
        public void TestMixTabAndSpaces()
        {
            string filePath = Path.Combine(Paths.GetTestFilesDirectory(), "mix-tabs-and-spaces.ds");

            Assert.Throws<DialogueParseException>(() =>
            {
                DialogueSequence sequence = dialogueParser.Parse(new FileReader().ReadFile(filePath));
            });
        }
    }
}