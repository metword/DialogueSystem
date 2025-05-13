using System;
using System.Collections.Generic;

namespace DialogueSystem
{
    public abstract class Line
    {
        // start off all lines enabled
        protected bool enabled = true;
        protected List<string> ids = new();

        /// <summary>
        /// Toggles on / off this line. When a line is toggled off, it should
        /// not be interpreted by the dialogue system
        /// </summary>
        /// <param name="enabled"></param>
        public void Toggle(bool enabled)
        {
            this.enabled = enabled;
        }

        /// <summary>
        /// Is this line currently enabled
        /// </summary>
        /// <returns></returns>
        public bool IsEnabled()
        {
            return enabled;
        }

        /// <summary>
        /// Adds an Id onto this line. A line can have multiple ids (optional
        /// lines)
        /// </summary>
        /// <param name="id">Id to add</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void AddId(string id)
        {
            if (id == null)
                throw new ArgumentNullException("Id cannot be null");
            this.ids.Add(id);
        }

        public List<string> GetIds()
        {
            return ids;
        }
    }
}
