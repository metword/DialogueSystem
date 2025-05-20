using DialogueSystem;
using NUnit.Framework;
using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;

namespace DialogueSystemRuntimeTests
{
    public class TestFormattedReader
    {
        private FormattedReader reader;
        private TextMeshProUGUI speaker;
        private TextMeshProUGUI line;
        private TextMeshProUGUI o1;
        private TextMeshProUGUI o2;
        private TextMeshProUGUI o3;
        private Canvas canvas;

        [SetUp]
        public void SetUp()
        {
            // create our game objects
            reader = new GameObject("Reader").AddComponent<FormattedReader>();
            speaker = new GameObject("SpeakerTMP").AddComponent<TextMeshProUGUI>();
            line = new GameObject("LineTMP").AddComponent<TextMeshProUGUI>();
            o1 = new GameObject("Option1TMP").AddComponent<TextMeshProUGUI>();
            o2 = new GameObject("Option2TMP").AddComponent<TextMeshProUGUI>();
            o3 = new GameObject("Option3TMP").AddComponent<TextMeshProUGUI>();
            canvas = new GameObject("TestCanvas").AddComponent<Canvas>();

            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            // give everything a font
            string pathToFont = Path.Combine("Fonts", "TestFontAsset");
            TMP_FontAsset fontAsset = Resources.Load<TMP_FontAsset>(pathToFont);
            speaker.font = fontAsset;
            line.font = fontAsset;
            o1.font = fontAsset;
            o2.font = fontAsset;
            o3.font = fontAsset;

            // add everything to the canvas
            speaker.transform.SetParent(canvas.transform);
            line.transform.SetParent(canvas.transform);
            o1.transform.SetParent(canvas.transform);
            o2.transform.SetParent(canvas.transform);
            o3.transform.SetParent(canvas.transform);

            // build the reader
            reader.SetSpeakerTextMesh(speaker);
            reader.SetLineTextMesh(line);
            reader.AddOptionTextMesh(o1);
            reader.AddOptionTextMesh(o2);
            reader.AddOptionTextMesh(o3);
        }

        [TearDown]
        public void TearDown()
        {
            Object.Destroy(reader.gameObject);
            Object.Destroy(speaker.gameObject);
            Object.Destroy(line.gameObject);
            Object.Destroy(o1.gameObject);
            Object.Destroy(o2.gameObject);
            Object.Destroy(o3.gameObject);
            Object.Destroy(canvas.gameObject);
        }

        [UnityTest]
        public IEnumerator TestFormattingWorks()
        {
            // parse the text
            string path = Path.Combine(Paths.GetTestFilesDirectory(), "formatting.ds");
            FileReader fileReader = new();
            string dialogue = fileReader.ReadFile(path);
            DialogueParser parser = new();
            DialogueSequence sequence = parser.Parse(dialogue);

            sequence.AddDialogueReader(reader);

            reader.RegisterFormat("bold", FormatCollection.Bold);

            sequence.StartSequence();

            yield return null;

            Assert.AreEqual("Tom", speaker.text);
            Assert.AreEqual("Hope you're <b>BOLD</b>!", line.text);

            yield return null;
        }

        [UnityTest]
        public IEnumerator TestReaderShowsLines()
        {
            string path = Path.Combine(Paths.GetTestFilesDirectory(), "sample-formatter.ds");
            FileReader fileReader = new();
            string dialogue = fileReader.ReadFile(path);

            DialogueSequence sequence = new DialogueParser().Parse(dialogue);

            sequence.AddDialogueReader(reader);

            sequence.StartSequence();

            yield return null;

            Assert.AreEqual("Tom", speaker.text);
            Assert.AreEqual("Hello there how are you?", line.text);
            reader.AdvanceLine();

            Assert.AreEqual("Good", o1.text);
            Assert.AreEqual("Bad", o2.text);
            Assert.IsEmpty(o3.text);

            reader.SelectOption(0);


            Assert.AreEqual("Tom", speaker.text);
            Assert.AreEqual("Glad you're doing well!", line.text);

            reader.AdvanceLine();

            Assert.AreEqual("Tom", speaker.text);
            Assert.AreEqual("Hope you have a great rest of your day person!", line.text);

            reader.AdvanceLine();

            Assert.AreEqual("Yeah thanks!", o1.text);
            Assert.AreEqual(string.Empty, o2.text);
            Assert.AreEqual(string.Empty, o3.text);

            reader.SelectOption(0);

            Assert.AreEqual("Woot", speaker.text);
            Assert.AreEqual("woot woot!", line.text);

            reader.AdvanceLine();

            Assert.AreEqual("Hello there!", o1.text);
            Assert.AreEqual("Woot woot!", o2.text);
            Assert.AreEqual("Wowie!", o3.text);

            reader.SelectOption(2);

            Assert.AreEqual("Tom", speaker.text);
            Assert.AreEqual("Bye!", line.text);

            reader.AdvanceLine();

            Assert.AreEqual("You", speaker.text);
            Assert.AreEqual("Bye bye!", line.text);

            reader.AdvanceLine();

            Assert.AreEqual("Tom", speaker.text);
            Assert.AreEqual("Hello!", line.text);

            reader.AdvanceLine();

            Assert.IsEmpty(speaker.text);
            Assert.IsEmpty(line.text);
            Assert.IsEmpty(o1.text);
            Assert.IsEmpty(o2.text);
            Assert.IsEmpty(o3.text);
        }

        [UnityTest]
        public IEnumerator TestGlobalFormatting()
        {
            // parse the text
            string path = Path.Combine(Paths.GetTestFilesDirectory(), "global-formatting.ds");
            FileReader fileReader = new();
            string dialogue = fileReader.ReadFile(path);
            DialogueParser parser = new();
            DialogueSequence sequence = parser.Parse(dialogue);

            reader.RegisterGlobalFormat(FormatLocation.Line, FormatCollection.Bold);

            sequence.AddDialogueReader(reader);

            sequence.StartSequence();

            yield return null;

            Assert.AreEqual("<b>Hello world!</b>", line.text);
            Assert.AreEqual("Tom", speaker.text);

            reader.AdvanceLine();

            Assert.AreEqual("<b>Goodbye earth!</b>", line.text);
            Assert.AreEqual("Tom", speaker.text);

            reader.RegisterGlobalFormat(FormatLocation.Speaker, FormatCollection.Bold);
            reader.AdvanceLine();

            Assert.AreEqual("<b>Speaker aaaa!</b>", line.text);
            Assert.AreEqual("<b>Tom</b>", speaker.text);

            reader.AdvanceLine();

            Assert.AreEqual("<b>The end!!!</b>", line.text);
            Assert.AreEqual("<b>Tom</b>", speaker.text);
        }

        [UnityTest]
        public IEnumerator TestCallbacks()
        {
            // parse the text
            string path = Path.Combine(Paths.GetTestFilesDirectory(), "sample-formatter.ds");
            FileReader fileReader = new();
            string dialogue = fileReader.ReadFile(path);
            DialogueParser parser = new();
            DialogueSequence sequence = parser.Parse(dialogue);

            bool lineCallback = false;
            bool optionCallback = false;
            bool end = false;

            sequence.AddDialogueReader(reader);
             
            reader.OnDialougeLine += (line) => lineCallback = true;
            reader.OnOptionalLine += (option) => optionCallback = true;
            reader.OnReadEnd += () => end = true;

            Assert.IsFalse(lineCallback, "Callback was called before starting sequence");

            sequence.StartSequence(); // hello there

            yield return null;

            Assert.IsTrue(lineCallback, "Callback was not set to true by starting sequence");
            Assert.IsFalse(optionCallback, "Option should not be true whens starting");

            lineCallback = false;
            optionCallback = false;

            reader.AdvanceLine(); // -> good bad

            Assert.IsFalse(lineCallback);
            Assert.IsTrue(optionCallback);
            lineCallback = false;
            optionCallback = false;

            reader.SelectOption(0); // glad your doing well

            Assert.IsTrue(lineCallback);
            Assert.IsFalse(optionCallback);
            lineCallback = false;
            optionCallback = false;

            reader.AdvanceLine(); // hope great rest of day

            Assert.IsTrue(lineCallback);
            Assert.IsFalse(optionCallback);
            lineCallback = false;
            optionCallback = false;

            reader.AdvanceLine(); // -> yeah thansk!
            Assert.IsFalse(lineCallback);
            Assert.IsTrue(optionCallback);
            lineCallback = false;
            optionCallback = false;

            reader.SelectOption(0); // woot woot!
            Assert.IsTrue(lineCallback);
            Assert.IsFalse(optionCallback);
            lineCallback = false;
            optionCallback = false;

            reader.AdvanceLine(); // -> hello woot wowie

            reader.SelectOption(2); // bye!
            Assert.IsFalse(end);

            reader.AdvanceLine(); // bye bye!

            reader.AdvanceLine(); // hello!
            Assert.IsFalse(end);

            reader.AdvanceLine(); // end
            Assert.IsTrue(end);
            end = false;

            reader.AdvanceLine();
            Assert.IsFalse(end);

            reader.SelectOption(0);
            Assert.IsFalse(end);

        }

        [UnityTest]
        public IEnumerator TestKeepTextOnNext()
        {
            string path = Path.Combine(Paths.GetTestFilesDirectory(), "sample-formatter.ds");
            FileReader fileReader = new();
            string dialogue = fileReader.ReadFile(path);
            DialogueParser parser = new();
            DialogueSequence sequence = parser.Parse(dialogue);

            sequence.AddDialogueReader(reader);

            reader.SetKeepTextOnNext(true);

            sequence.StartSequence();

            yield return null;

            Assert.AreEqual("Tom", speaker.text);
            Assert.AreEqual("Hello there how are you?", line.text);
            reader.AdvanceLine();

            Assert.AreEqual("Tom", speaker.text);
            Assert.AreEqual("Hello there how are you?", line.text);
            Assert.AreEqual("Good", o1.text);
            Assert.AreEqual("Bad", o2.text);
            Assert.IsEmpty(o3.text);

            reader.SelectOption(0);

            Assert.AreEqual("Tom", speaker.text);
            Assert.AreEqual("Glad you're doing well!", line.text);
            Assert.AreEqual("Good", o1.text);
            Assert.AreEqual("Bad", o2.text);
            Assert.IsEmpty(o3.text);

            reader.AdvanceLine();

            Assert.AreEqual("Tom", speaker.text);
            Assert.AreEqual("Hope you have a great rest of your day person!", line.text);
            Assert.AreEqual("Good", o1.text);
            Assert.AreEqual("Bad", o2.text);
            Assert.IsEmpty(o3.text);

            reader.AdvanceLine();

            Assert.AreEqual("Tom", speaker.text);
            Assert.AreEqual("Hope you have a great rest of your day person!", line.text);
            Assert.AreEqual("Yeah thanks!", o1.text);
            Assert.AreEqual(string.Empty, o2.text);
            Assert.AreEqual(string.Empty, o3.text);

            reader.SelectOption(0);

            Assert.AreEqual("Woot", speaker.text);
            Assert.AreEqual("woot woot!", line.text);
            Assert.AreEqual("Yeah thanks!", o1.text);
            Assert.AreEqual(string.Empty, o2.text);
            Assert.AreEqual(string.Empty, o3.text);

            reader.AdvanceLine();

            Assert.AreEqual("Woot", speaker.text);
            Assert.AreEqual("woot woot!", line.text);
            Assert.AreEqual("Hello there!", o1.text);
            Assert.AreEqual("Woot woot!", o2.text);
            Assert.AreEqual("Wowie!", o3.text);

            reader.SelectOption(2);

            Assert.AreEqual("Tom", speaker.text);
            Assert.AreEqual("Bye!", line.text);
            Assert.AreEqual("Hello there!", o1.text);
            Assert.AreEqual("Woot woot!", o2.text);
            Assert.AreEqual("Wowie!", o3.text);

            reader.AdvanceLine();

            Assert.AreEqual("You", speaker.text);
            Assert.AreEqual("Bye bye!", line.text);
            Assert.AreEqual("Hello there!", o1.text);
            Assert.AreEqual("Woot woot!", o2.text);
            Assert.AreEqual("Wowie!", o3.text);

            reader.AdvanceLine();

            Assert.AreEqual("Tom", speaker.text);
            Assert.AreEqual("Hello!", line.text);
            Assert.AreEqual("Hello there!", o1.text);
            Assert.AreEqual("Woot woot!", o2.text);
            Assert.AreEqual("Wowie!", o3.text);

            // advance a few extra times at the end to ensure the text does not change
            reader.AdvanceLine();

            Assert.AreEqual("Tom", speaker.text);
            Assert.AreEqual("Hello!", line.text);
            Assert.AreEqual("Hello there!", o1.text);
            Assert.AreEqual("Woot woot!", o2.text);
            Assert.AreEqual("Wowie!", o3.text);

            reader.AdvanceLine();

            Assert.AreEqual("Tom", speaker.text);
            Assert.AreEqual("Hello!", line.text);
            Assert.AreEqual("Hello there!", o1.text);
            Assert.AreEqual("Woot woot!", o2.text);
            Assert.AreEqual("Wowie!", o3.text);
        }
        [UnityTest]
        public IEnumerator TestMultilineDialogue()
        {
            DialogueParser parser = new();
            DialogueSequence sequence = parser.Parse("-Start\nTom: hello there : how are you? : hope you're well!");

            sequence.AddDialogueReader(reader);

            sequence.StartSequence();
             
            yield return null;

            Assert.AreEqual("Tom", speaker.text);
            Assert.AreEqual("hello there", line.text);

            reader.AdvanceLine();

            Assert.AreEqual("", speaker.text);
            Assert.AreEqual("how are you?", line.text);

            reader.AdvanceLine();

            Assert.AreEqual("", speaker.text);
            Assert.AreEqual("hope you're well!", line.text);

            sequence.SetCurrentNode("Start", 0);
            sequence.StartSequence();
            yield return null;

            reader.SetKeepTextOnNext(true);

            Assert.AreEqual("Tom", speaker.text);
            Assert.AreEqual("hello there", line.text);

            reader.AdvanceLine();

            Assert.AreEqual("Tom", speaker.text);
            Assert.AreEqual("how are you?", line.text);

            reader.AdvanceLine();

            Assert.AreEqual("Tom", speaker.text);
            Assert.AreEqual("hope you're well!", line.text);
        }
    }
}

