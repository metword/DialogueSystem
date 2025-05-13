using System;
using System.Collections;
using DialogueSystem;
using NUnit.Framework;
using UnityEngine.TestTools;
using UnityEngine;

namespace DialogueSystemRuntimeTests
{
    public class TestDialogueSequence
    {
        private DialogueSequence sequence;

        [SetUp]
        public void SetUp()
        {
            sequence = new DialogueSequence();
        }

        [Test]
        public void TestSettingNode()
        {
            // no nodes in sequence
            Assert.Throws<NodeNotFoundException>(() =>
            {
                sequence.SetCurrentNode("Node");
            });

            sequence.AddNode("Node");

            Assert.Throws<ArgumentException>(() =>
            {
                sequence.AddNode("Node");
            });

            // methods requiring node to be set
            sequence.SetCurrentNode("Node");

            sequence.AddLine("Node", new DialogueLine("Hello", "World"));
        }


        [Test]
        public void TestGetCurrentLine()
        {
            DialogueLine newLine = new("Hello", "World");
            DialogueLine secondLine = new("Hello", "World");

            Assert.Null(sequence.GetCurrentVisibleLine());

            sequence.AddNode("Start");

            sequence.AddLine("Start", newLine);

            sequence.SetCurrentNode("Start");

            Assert.AreEqual(sequence.GetCurrentVisibleLine(), newLine);

            sequence.AddLine("Start", secondLine);

            Assert.AreEqual(sequence.GetCurrentVisibleLine(), newLine);
        }

        /// <summary>
        /// Tests stepping through a built sequence ensuring that control flows
        /// appropriately
        /// </summary>
        [Test]
        public void TestStepThrough()
        {
            MockDialogueReader reader = new();

            string startNode = "Start";
            string endNode = "End";
            string node1 = "1";
            string node2 = "2";

            // build an optional line
            OptionalLine option = new OptionalLine();
            int option1Id = option.AddOption(node1, "Go to node 1");
            int option2Id = option.AddOption(node2, "Go to node 2");

            // Add nodes
            sequence.AddNode(startNode);
            sequence.AddNode(node1);
            sequence.AddNode(node2);
            sequence.AddNode(endNode);

            // add lines
            sequence.AddLine(startNode, new DialogueLine("Tom", "Hi there, my name is tom!"));
            sequence.AddLine(startNode, new DialogueLine("Tom", "Hope you have a good day!"));
            sequence.AddLine(startNode, option);

            sequence.AddLine(node1, new DialogueLine("Bob", "Hello! I'm from node1!"));
            sequence.AddLine(node2, new DialogueLine("Billy", "Hello! I'm from node2!"));
            sequence.AddLine(node1, new GotoLine(endNode, sequence));

            sequence.AddLine(endNode, new DialogueLine("End", "Hello!!! at the end yo!!!"));

            sequence.AddDialogueReader(reader);
            sequence.SetCurrentNode(startNode);
            sequence.StartSequence();

            Assert.AreEqual("Tom", reader.GetLine().GetSpeaker(), "Did not correctly get first speaker");
            Assert.AreEqual("Hi there, my name is tom!", reader.GetLine().GetText(), "Did not correctly get first line");

            reader.NextLine();

            Assert.AreEqual("Tom", reader.GetLine().GetSpeaker(), "Did not correctly get second speaker");
            Assert.AreEqual("Hope you have a good day!", reader.GetLine().GetText(), "Did not correctly get second line");

            reader.NextLine();

            OptionalLine currentOption = reader.GetOption();
            Option option1 = currentOption.GetOption(option1Id);
            Option option2 = currentOption.GetOption(option2Id);

            Assert.AreEqual("Go to node 1", option1.Text);
            Assert.AreEqual("Go to node 2", option2.Text);

            reader.NextOption(option1Id);

            Assert.AreEqual("Bob", reader.GetLine().GetSpeaker());
            Assert.AreEqual("Hello! I'm from node1!", reader.GetLine().GetText());

            reader.NextLine();

            Assert.AreEqual("End", reader.GetLine().GetSpeaker());
            Assert.AreEqual("Hello!!! at the end yo!!!", reader.GetLine().GetText());

            reader.NextLine();

            // no more lines after this
            Assert.Null(reader.GetLine());

            // restart

            sequence.SetCurrentNode(startNode);
            sequence.StartSequence();
            Assert.AreEqual("Tom", reader.GetLine().GetSpeaker(), "Did not correctly get first speaker");
            Assert.AreEqual("Hi there, my name is tom!", reader.GetLine().GetText(), "Did not correctly get first line");

            reader.NextLine();

            Assert.AreEqual("Tom", reader.GetLine().GetSpeaker(), "Did not correctly get second speaker");
            Assert.AreEqual("Hope you have a good day!", reader.GetLine().GetText(), "Did not correctly get second line");

            reader.NextLine();

            Assert.AreEqual("Go to node 1", option1.Text);
            Assert.AreEqual("Go to node 2", option2.Text);

            reader.NextOption(option2Id);

            Assert.AreEqual("Billy", reader.GetLine().GetSpeaker());
            Assert.AreEqual("Hello! I'm from node2!", reader.GetLine().GetText());

            reader.NextLine();

            Assert.Null(reader.GetLine());
        }
    }
}
