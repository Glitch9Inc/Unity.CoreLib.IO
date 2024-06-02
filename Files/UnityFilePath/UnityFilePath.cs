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
    public class UnityFilePath
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
        [JsonIgnore]
        public string Path
        {
            get => path;
            set => path = value;
        }

        /// <summary>
        /// Gets the content type of the file.
        /// </summary>
        [JsonIgnore]
        public ContentType Type
        {
            get => type;
            set => type = value;
        }

        /// <summary>
        /// Gets the file name.
        /// </summary>
        [JsonIgnore] public string FileName => _fileName ??= System.IO.Path.GetFileName(path);

        private string _fileName;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnityFilePath"/> class.
        /// </summary>
        [JsonConstructor]
        public UnityFilePath() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnityFilePath"/> class with the specified Unity path type and file path.
        /// </summary>
        /// <param name="unityPath">The Unity path type.</param>
        /// <param name="path">The file path.</param>
        /// <param name="fileType">The content type of the file.</param>
        public UnityFilePath(UnityPath unityPath, string path, ContentType fileType = ContentType.Json)
        {
            this.unityPath = unityPath;
            this.path = path;
            this.type = fileType;
            //type = fileType ?? ContentTypeUtils.ParseFileExtension(path);
        }

        public bool FileExists()
        {
            string localPath = this.GetLocalPath();
            return System.IO.File.Exists(localPath);
        }
    }
}