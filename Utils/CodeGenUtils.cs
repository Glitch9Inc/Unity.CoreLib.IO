#if UNITY_EDITOR
using Cysharp.Threading.Tasks;
using System;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Glitch9.IO.CodeGen
{
    public static class CodeGenUtils
    {
        /// <summary>
        /// Writes a C# script string to a file
        /// </summary>
        /// <param name="csharpString"></param>
        /// <param name="savePath"></param>
        /// <param name="namespace"></param>
        /// <returns>
        /// Returns class name
        /// </returns>
        public static async UniTask<string> CSharpScriptStringToFile(string csharpString, string savePath, string @namespace = null)
        {
            if (string.IsNullOrWhiteSpace(csharpString)) return null;

            // detect the class name
            string className = ExtractClassName(csharpString);

            if (string.IsNullOrWhiteSpace(className))
            {
                Debug.LogWarning("Could not detect class name.");
                return null;
            }

            if (string.IsNullOrWhiteSpace(savePath)) return null;

            if (!string.IsNullOrWhiteSpace(@namespace))
            {
                csharpString = InsertNamespace(csharpString, @namespace);
            }

            string directory = $"{Application.dataPath}/{savePath}";
            string path = $"{directory}/{className}.cs";
            // create dir if it doesn't exist
            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }

            await System.IO.File.WriteAllTextAsync(path, csharpString);
            AssetDatabase.Refresh();

            return className;
        }

        /// <summary>
        /// Extracts the class name from a C# script string
        /// </summary>
        /// <param name="csharpString"></param>
        /// <returns>
        /// Extracted class name
        /// </returns>
        public static string ExtractClassName(string csharpString)
        {
            string[] lines = csharpString.Split('\n');
            string className = string.Empty;

            foreach (string line in lines)
            {
                if (line.Contains("class"))
                {
                    // Split the line into parts
                    string[] parts = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    int classIndex = Array.IndexOf(parts, "class"); // "class" index
                    if (classIndex != -1 && classIndex < parts.Length - 1) // "class" is not the last word in the line
                    {
                        className = parts[classIndex + 1]; // "class" is followed by the class name
                        break;
                    }
                }
            }

            return className;
        }

        /// <summary>
        /// Inserts a namespace into a C# script string
        /// </summary>
        /// <param name="csharpString"></param>
        /// <returns>
        /// C# script string with namespace inserted
        /// </returns>
        public static string InsertNamespace(string csharpString, string @namespace)
        {
            // Check if the namespace is already present
            if (csharpString.Contains($"namespace {@namespace}"))
            {
                Debug.LogWarning($"Namespace {@namespace} already exists.");
                return csharpString;
            }

            // Define the pattern to match the start of a class, interface, struct, or enum
            string pattern = @"\b(public|private|internal|abstract|sealed|class|interface|struct|enum)\s+";

            // Match the pattern
            Match match = Regex.Match(csharpString, pattern, RegexOptions.Multiline);

            if (match.Success)
            {
                // Split the string into two parts: before and after the match
                string beforeNamespace = csharpString.Substring(0, match.Index);
                string afterNamespace = csharpString.Substring(match.Index);
                string indentedAfterNamespace = Regex.Replace(afterNamespace, @"^", "    ", RegexOptions.Multiline);

                // Insert the namespace
                string result = beforeNamespace +
                                $"namespace {@namespace}\n{{\n" +
                                indentedAfterNamespace +
                                "\n}";

                return result;
            }

            // If the pattern is not found, return the original string
            return csharpString;
        }
    }
}
#endif