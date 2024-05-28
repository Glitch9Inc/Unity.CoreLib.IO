using System;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;

namespace Glitch9.IO.Files
{
    /// <summary>
    /// A wrapper for file paths that can be serialized to JSON.
    /// </summary>
    [Serializable]
    public class FilePath
    {
        [SerializeField] private string id;
        [FormerlySerializedAs("type")][SerializeField] private UnityPath unityPath;
        [FormerlySerializedAs("uri")][SerializeField] private string path;
        [SerializeField] private ContentType type;

        /// <summary>
        /// Gets the unique identifier for the file path.
        /// </summary>
        [JsonIgnore] public string Id => id;

        /// <summary>
        /// Gets the Unity path type.
        /// </summary>
        [JsonIgnore] public UnityPath UnityPath => unityPath;

        /// <summary>
        /// Gets the file path.
        /// </summary>
        [JsonIgnore] public string Path => path;

        /// <summary>
        /// Gets the content type of the file.
        /// </summary>
        [JsonIgnore] public ContentType Type => type;

        /// <summary>
        /// Gets the file name.
        /// </summary>
        [JsonIgnore] public string FileName => _fileName ??= System.IO.Path.GetFileName(path);

        private string _fileName;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilePath"/> class.
        /// </summary>
        [JsonConstructor]
        public FilePath() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilePath"/> class with the specified Unity path type and file path.
        /// </summary>
        /// <param name="unityPath">The Unity path type.</param>
        /// <param name="path">The file path.</param>
        public FilePath(UnityPath unityPath, string path)
        {
            this.unityPath = unityPath;
            this.path = path;
        }
    }
}