using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace DialogueSystem
{
    /// <summary>
    /// The TextMesh Reader reads in input in the form of Dialogue lines and
    /// then formats those lines based on the formatting language and defined
    /// TextFormatters
    /// </summary>
    public class FormattedReader : MonoBehaviour, IDialogueReader
    {
        // the text mesh reader needs references to TextMeshPro objects to
        // display LINES and OPTIONS

        [SerializeField] private TextMeshProUGUI lineTextMesh;
        [SerializeField] private TextMeshProUGUI speakerTextMesh;
        [SerializeField] private List<TextMeshProUGUI> optionTextMeshes;

        private Dictionary<string, List<Format>> formats;

        // line memmbers
        private List<TextMeshFormatter> lineFormatters;
        private Action lineCallback;
        private float lineTime;
        private bool canAdvanceLine;

        // option members
        private List<TextMeshFormatter> optionFormatters;
        private Action<string> optionCallback;
        private OptionalLine currentOption;
        private float optionTime;
        private bool canAdvanceOption;


        private void Awake()
        {
            formats = new();
            lineFormatters = new();
            optionFormatters = new();
        }

        /// <summary>
        /// Registers the given format on this reader, it'll be passed onto 
        /// formatted text that is displayed with the ID
        /// </summary>
        /// <param name="id">Id of the format to search for within dialogue
        /// </param>
        /// <param name="format">Format rule itself</param>
        public void RegisterFormat(string id, Format format)
        {
            if (!formats.ContainsKey(id))
                formats.Add(id, new List<Format>());

            formats[id].Add(format);
        }
        public void ReadEnd()
        {
            // when reading the end we should just close all formatters
            CleanUp();
        }
        public void ReadLine(DialogueLine line, Action callback)
        {
            CleanUp();

            string text = line.GetText();
            string speaker = line.GetSpeaker();
            List<bool> bools = new();
            void allTrue() => canAdvanceLine = true;

            // create formatters for the line and the speaker
            // save those formatters and then update them every frame
            lineFormatters.Add(new(line.GetText(), lineTextMesh, formats, AddFinishCallback(bools, allTrue)));
            lineFormatters.Add(new(line.GetSpeaker(), speakerTextMesh, formats, AddFinishCallback(bools, allTrue)));

            lineTime = Time.time;
            lineCallback = callback;
        }
        public void ReadOption(OptionalLine option, Action<string> callback)
        {
            CleanUp();

            int visibleOptions = Math.Min(option.CountOptions(), optionTextMeshes.Count);
            List<bool> bools = new();
            void allTrue() => canAdvanceOption = true;

            // iterate over each ordered option and create the formatted line
            for (int i = 0; i < visibleOptions; i++)
            {
                Option selected = option.GetOption(i);
                TextMeshProUGUI textMesh = optionTextMeshes[i];
                TextMeshFormatter newFormatter = new(selected.Text, textMesh, formats, AddFinishCallback(bools, allTrue));

                optionFormatters.Add(newFormatter);
            }

            optionTime = Time.time;
            optionCallback = callback;
            currentOption = option;
        }

        /// <summary>
        /// Attempts to advance the currently displaying line. A line can be
        /// advanced if all the formatters have finished displaying
        /// </summary>
        /// <returns>true if the line was successfully advanced, else returns
        /// false</returns>
        public bool AdvanceLine()
        {
            Update();

            if (!canAdvanceLine)
                return false;

            // reset all settings
            canAdvanceLine = false;
            lineTime = -1;
            lineCallback.Invoke();

            return true;
        }

        /// <summary>
        /// Selects the given option if possible. Options can be selected if
        /// all the optional formatters have finished displaying.
        /// </summary>
        /// <param name="optionIndex">Option being selected</param>
        ///<returns>True if the option was selected and execution was moved on
        ///</returns>
        public bool SelectOption(int optionIndex)
        {
            Update();

            if (!canAdvanceOption)
                return false;

            // try get the opton
            if (!currentOption.TryGetOption(optionIndex, out Option selected))
            {
                return false;
            }

            // change path
            string node = selected.Node;
            canAdvanceOption = false;
            optionTime = -1;
            optionCallback.Invoke(node);

            return true;
        }

        public void SetSpeakerTextMesh(TextMeshProUGUI textMesh)
        {
            speakerTextMesh = textMesh;
        }
        public void SetLineTextMesh(TextMeshProUGUI textMesh)
        {
            lineTextMesh = textMesh;
        }

        public void AddOptionTextMesh(TextMeshProUGUI textMesh)
        {
            optionTextMeshes ??= new();
            optionTextMeshes.Add(textMesh);
        }

        /// <summary>
        /// Creates a callback using the list of booleans. When all the 
        /// callbacks have finished, the alltrue callback is invoked
        /// </summary>
        /// <param name="bools">List of booleans used to store state</param>
        /// <param name="allCallbacked">Callback invoked when all the callbacks
        /// within have finished</param>
        /// <returns>The created callback</returns>
        private Action AddFinishCallback(List<bool> bools, Action allCallbacked)
        {
            int index = bools.Count;

            bools.Add(false);

            void callback()
            {
                bools[index] = true;

                if (bools.All(x => x))
                {
                    allCallbacked.Invoke();
                }
            }

            return callback;
        }

        /// <summary>
        /// Just reset all state to beginning
        /// </summary>
        private void CleanUp()
        {
            lineFormatters.Clear();
            optionFormatters.Clear();
            canAdvanceLine = false;
            canAdvanceOption = false;
            lineTime = -1;
            optionTime = -1;

            lineTextMesh.text = string.Empty;
            speakerTextMesh.text = string.Empty;
            optionTextMeshes.ForEach(mesh => mesh.text = string.Empty);
        }

        //TODO: possibly add the option to specify default formats that envelop
        // the entire method? 

        public void Update()
        {
            // iterate over all line formats
            if (lineTime >= 0)
            {
                float deltaLineTime = Time.time - lineTime;
                foreach (TextMeshFormatter formatter in lineFormatters)
                {

                    formatter?.UpdateFormats(deltaLineTime);
                }
            }

            // iterate over all option formats
            if (optionTime >= 0)
            {
                float deltaOptionTime = Time.time - optionTime;
                foreach (TextMeshFormatter formatter in optionFormatters)
                {
                    formatter?.UpdateFormats(deltaOptionTime);
                }
            }
        }
    }
}
