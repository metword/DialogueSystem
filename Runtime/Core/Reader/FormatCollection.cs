using DialogueSystem.Utils;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace DialogueSystem
{
    /// <summary>
    /// List of useful formatting delegates that can be used when needed
    /// </summary>
    public static class FormatCollection
    {
        public static bool Bold(float time, int startFormat, int endFormat, TextMeshProUGUI textMesh)
        {
            TMP_TextInfo info = textMesh.GetTextInfo(textMesh.text);
            TMP_CharacterInfo[] charInfo = info.characterInfo;

            // indicies of the characters (inclusive) which will be bolded
            int startInsertIndex = charInfo[startFormat].index;


            int endInsertIndex = charInfo[endFormat - 1].index;

            // only insert if the bold does not exist in the space before up to
            // a previous </b> or <b>

            string text = textMesh.text;
            int indexOpenAfter = StringUtils.IndexOfAfter(text, "<b>", endInsertIndex);
            if (indexOpenAfter == -1) indexOpenAfter = text.Length;
            string afterSubstring = text.UnboundedSubstring(endInsertIndex + 1, indexOpenAfter);

            int indexCloseBefore = StringUtils.IndexOfBefore(text, "</b>", startInsertIndex);
            string beforeSubstring = text.UnboundedSubstring(indexCloseBefore, startInsertIndex);

            if (!afterSubstring.Contains("</b>"))
            {
                text = text.Insert(endInsertIndex + 1, "</b>");
            }

            if (!beforeSubstring.Contains("<b>"))
            {
                text = text.Insert(startInsertIndex, "<b>");
            }

            textMesh.text = text;

            return true;
        }
    }
}
