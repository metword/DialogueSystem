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
        [UnityTest]
        public IEnumerator TestFormattingWorks()
        {
            // parse the text
            string path = Path.Combine(Paths.GetTestFilesDirectory(), "formatting.ds");
            FileReader fileReader = new();
            string dialogue = fileReader.ReadFile(path);
            DialogueParser parser = new();
            DialogueSequence sequence = parser.Parse(dialogue);

            GameObject readerGO = new GameObject("Reader");
            FormattedReader reader = readerGO.AddComponent<FormattedReader>();
            TextMeshProUGUI speaker = new GameObject("SpeakerTMP").AddComponent<TextMeshProUGUI>();
            TextMeshProUGUI line = new GameObject("LineTMP").AddComponent<TextMeshProUGUI>();
            TextMeshProUGUI o1 = new GameObject("Option1TMP").AddComponent<TextMeshProUGUI>();
            TextMeshProUGUI o2 = new GameObject("Option2TMP").AddComponent<TextMeshProUGUI>();
            TextMeshProUGUI o3 = new GameObject("Option3TMP").AddComponent<TextMeshProUGUI>();

            GameObject canvasGO = new GameObject("TestCanvas");
            Canvas canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            string pathToFont = Path.Combine("Fonts", "TestFontAsset");
            TMP_FontAsset fontAsset = Resources.Load<TMP_FontAsset>(pathToFont);

            speaker.font = fontAsset;
            line.font = fontAsset;
            o1.font = fontAsset;
            o2.font = fontAsset;
            o3.font = fontAsset;

            speaker.transform.SetParent(canvas.transform);
            line.transform.SetParent(canvas.transform);
            o1.transform.SetParent(canvas.transform);
            o2.transform.SetParent(canvas.transform);
            o3.transform.SetParent(canvas.transform);

            yield return null;
            yield return null;
            yield return null;

            speaker.ForceMeshUpdate(true, true);

            reader.SetSpeakerTextMesh(speaker);
            reader.SetLineTextMesh(line);
            reader.AddOptionTextMesh(o1);
            reader.AddOptionTextMesh(o2);
            reader.AddOptionTextMesh(o3);

            sequence.AddDialogueReader(reader);

            reader.RegisterFormat("bold", FormatCollection.Bold);

            sequence.StartSequence();

            speaker.ForceMeshUpdate();
            line.ForceMeshUpdate();
            o1.ForceMeshUpdate();
            o2.ForceMeshUpdate();
            o3.ForceMeshUpdate();

            yield return null;

            Assert.AreEqual("Tom", speaker.text);
            Assert.AreEqual("Hope you're <b>BOLD</b>!", line.text);

            Object.Destroy(readerGO);
            Object.Destroy(speaker.gameObject);
            Object.Destroy(line.gameObject);
            Object.Destroy(o1.gameObject);
            Object.Destroy(o2.gameObject);
            Object.Destroy(o3.gameObject);

            yield return null;
        }

        [UnityTest]
        public IEnumerator TestReaderShowsLines()
        {
            string path = Path.Combine(Paths.GetTestFilesDirectory(), "sample-formatter.ds");
            FileReader fileReader = new();
            string dialogue = fileReader.ReadFile(path);

            DialogueSequence sequence = new DialogueParser().Parse(dialogue);

            FormattedReader reader = new GameObject("Reader").AddComponent<FormattedReader>();
            TextMeshProUGUI speaker = new GameObject("SpeakerTMP").AddComponent<TextMeshProUGUI>();
            TextMeshProUGUI line = new GameObject("LineTMP").AddComponent<TextMeshProUGUI>();
            TextMeshProUGUI o1 = new GameObject("Option1TMP").AddComponent<TextMeshProUGUI>();
            TextMeshProUGUI o2 = new GameObject("Option2TMP").AddComponent<TextMeshProUGUI>();
            TextMeshProUGUI o3 = new GameObject("Option3TMP").AddComponent<TextMeshProUGUI>();

            reader.SetSpeakerTextMesh(speaker);
            reader.SetLineTextMesh(line);
            reader.AddOptionTextMesh(o1);
            reader.AddOptionTextMesh(o2);
            reader.AddOptionTextMesh(o3);

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

            Object.Destroy(reader.gameObject);
            Object.Destroy(speaker.gameObject);
            Object.Destroy(line.gameObject);
            Object.Destroy(o1.gameObject);
            Object.Destroy(o2.gameObject);
            Object.Destroy(o3.gameObject);

        }
    }
}

