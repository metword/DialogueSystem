using System;
using System.Collections.Generic;
using System.Linq;

namespace DialogueSystem
{
    /// <summary>
    /// An Identified String holds ids identifying various chunks within a
    /// string and the indicies demarcating start and end of those chunks
    /// </summary>
    public class IdentifiedString
    {
        // identified string should be built by the FormatParser and used by
        // the TextMeshFormatter. TextMeshFormatter just needs to pass on,
        // indicies when given an ID to the IFormat interface

        private Dictionary<string, (int, int)> chunks;
        private string text;
        public IdentifiedString(string text)
        {
            chunks = new();
            this.text = text;
        }

        /// <summary>
        /// Adds an Id label on this identified string
        /// </summary>
        /// <param name="id">Id of this bound</param>
        /// <param name="startIndex">start index inclusive of this bound</param>
        /// <param name="endIndex">end index esclusive of this bound</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if bounds are
        /// not valid</exception>
        /// <exception cref="ArgumentException">Thrown if duplicate key is
        /// attempted to be added or if startIndex is greater than endIndex
        /// </exception>
        public void AddBounds(string id, int startIndex, int endIndex)
        {
            // do a verification to ensure indicies are within bounds
            if (startIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex), "Cannot be less than 0");
            }
            if (endIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(endIndex), "Cannot be less than 0");
            }
            if (startIndex > text.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex), "Cannot be greater than text length");
            }
            if (endIndex > text.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(endIndex), "Cannot be greater than text length");
            }
            if (startIndex > endIndex)
            {
                throw new ArgumentException("Start index must come before end index");
            }
            if (chunks.ContainsKey(id))
            {
                throw new ArgumentException(nameof(id), "Key already exists");
            }

            this.chunks.Add(id, (startIndex, endIndex));
        }

        /// <summary>
        /// Returns true if the bound is present in this string
        /// </summary>
        /// <param name="id">id of the bound to check</param>
        /// <returns>true if the bound is present, else false</returns>
        public bool ContainsBounds(string id)
        {
            return chunks.ContainsKey(id);
        }

        /// <summary>
        /// Gets all bounds ids currently at this identified string
        /// </summary>
        /// <returns>The list of unique string bound identifiers</returns>
        public List<string> GetAllIds()
        {
            return chunks.Keys.ToList();
        }

        public string GetText()
        {
            return text;
        }

        /// <summary>
        /// Returns the bounds of the identified string if it exists
        /// </summary>
        /// <param name="id">Id to get</param>
        /// <returns>The bounds</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the key does not
        /// exist </exception>
        public (int start, int end) GetBounds(string id)
        {
            if (chunks.TryGetValue(id, out (int, int) pair)) {
                return pair;
            }
            throw new KeyNotFoundException($"Key {id} not present");
        }
    }
}

