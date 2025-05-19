using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace DialogueSystem
{
    /// <summary>
    /// The TextMeshFormatter applies vertice formatting to a chunk of
    /// identified text. Using a simple identifying language outlined more in
    /// detail in the manual
    /// </summary>
    public class TextMeshFormatter
    {
        private FormatParser formatParser;
        private IdentifiedString identifiedString;
        private TextMeshProUGUI textMesh;
        private Dictionary<string, List<Format>> formats;
        private Action onComplete;

        /// <summary>
        /// Construct a new TextMeshFormatter to format textMesh objects
        /// </summary>
        /// <param name="text">Actual text held in the text mesh</param>
        /// <param name="textMesh">Text mesh object to display on</param>
        /// <param name="formats">Formats to be used with the Formatter</param>
        /// <param name="onComplete">Callback to be invoked when all formats
        /// have finished animating</param>c  
        public TextMeshFormatter(string text, TextMeshProUGUI textMesh, Dictionary<string, List<Format>> formats, Action onComplete = null)
        {
            this.formatParser = new FormatParser();
            // first we parse the text using the formatting language into
            // identified substrings demarcated with index starts and ends

            this.identifiedString = formatParser.Format(text);

            this.textMesh = textMesh;

            this.textMesh.text = identifiedString.GetText();

            this.formats = formats;

            this.onComplete = onComplete;
        }

        /// <summary>
        /// Add a format to the Textformatter using the specified params
        /// </summary>
        /// <param name="id">Id for the formatter</param>
        /// <param name="startIndex">Beginning of the format (inclusive)</param>
        /// <param name="endIndex">End of the format (exclusive)</param>
        /// <param name="format">Format being added</param>
        public void AddFormat(string id, int startIndex, int endIndex, Format format)
        {
            identifiedString.AddBounds(id, startIndex, endIndex);

            if (!formats.ContainsKey(id))
            {
                formats.Add(id, new List<Format>());
            }

            formats[id].Add(format);
        }

        /// <summary>
        /// Adds a format to the formatter that extends the whole bounds of the
        /// string
        /// </summary>
        /// <param name="id">Id for the formatter</param>
        /// <param name="format">Format being added</param>
        public void AddFormat(string id, Format format)
        {
            identifiedString.AddBounds(id, 0, identifiedString.GetText().Length);

            if (!formats.ContainsKey(id))
            {
                formats.Add(id, new List<Format>());
            }

            formats[id].Add(format);
        }

        /// <summary>
        /// Updates all formats on the formatted TextMesh
        /// </summary>
        /// <param name="time">Time of the animation occurring</param>
        public void UpdateFormats(float time)
        {
            // do the formats
            // activate callback if all formats are finished
            bool done = true;

            foreach (string id in identifiedString.GetAllIds())
            {
                if (formats.TryGetValue(id, out List<Format> formatList))
                {
                    (int start, int end) = identifiedString.GetBounds(id);

                    foreach (Format format in formatList)
                    {
                        bool formatDone = format(time, start, end, textMesh);
                        done = done && formatDone;
                    }
                }
            }

            // if all the formats that were hit are true, done will be true
            if (done)
            {
                onComplete?.Invoke();
            }
        } 
    }
}

