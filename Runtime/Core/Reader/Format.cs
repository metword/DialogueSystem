using TMPro;

namespace DialogueSystem
{
    /// <summary>
    /// Apply the format to the given text.
    /// </summary>
    /// <param name="time">Time from beginning of animation</param>
    /// <param name="startFormat">Start index (inclusive) to be formatted</param>
    /// <param name="endFormat">End index (exclusive)</param>
    /// <param name="textMesh">Text mesh object being formatted</param>
    /// <returns>True when the format has finished animating</returns>
    public delegate bool Format(float time, int startFormat, int endFormat, TextMeshProUGUI textMesh);

}
