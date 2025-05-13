using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace DialogueSystemRuntimeTests
{
    public class Paths
    {
        public static string GetTestFilesDirectory()
        {
            string scriptFilePath = new System.Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
            string scriptDirectory = Path.GetDirectoryName(scriptFilePath);
            string libraryDirectory = Directory.GetParent(scriptDirectory).FullName;
            string projectRootDirectory = Directory.GetParent(libraryDirectory).FullName;
            string testFilesDirectory = Path.Combine(projectRootDirectory, "Assets/DialogueSystem/Tests/Runtime/TestFiles");
            return testFilesDirectory;
        }
    }
}
