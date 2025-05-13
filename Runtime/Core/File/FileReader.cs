using System.IO;
using UnityEngine;

namespace DialogueSystem
{
    /// <summary>
    /// Turns a given file into a string
    /// </summary>
    public class FileReader
    {
        /// <summary>
        /// Reads the file at the given path
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public string ReadFile(string filename)
        {
            byte[] bytes = File.ReadAllBytes(filename);
            string text = System.Text.Encoding.UTF8.GetString(bytes);
            return text;
        }
    }
}