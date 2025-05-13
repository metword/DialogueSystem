using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace DialogueSystem.Sample
{
    /// <summary>
    /// Show off some of the features in the dialogue system
    /// </summary>
    public class SimpleDialogueSequence : MonoBehaviour
    {
        [SerializeField] private FormattedReader formattedReader;
        [SerializeField] private TextAsset textAsset;

        // Start is called before the first frame update
        void Start()
        {
            // Build Sequence and Reader
            DialogueParser parser = new ();
            string text = textAsset.text;

            DialogueSequence sequence = parser.Parse(text);

            sequence.AddDialogueReader(formattedReader);

            formattedReader.RegisterFormat("bold", FormatCollection.Bold);

            sequence.StartSequence();
        }

        private void Update()
        {
            // test inputs
            if (Input.GetButtonDown("Fire1"))
            {
                formattedReader.AdvanceLine();
            }
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                formattedReader.SelectOption(0);
            }
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                formattedReader.SelectOption(1);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                formattedReader.SelectOption(2);
            }
        }
    }
}