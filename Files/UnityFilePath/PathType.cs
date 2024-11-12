using UnityEngine;

namespace Glitch9.IO.Files
{
    /// <summary>
    /// Specifies different types of paths used in Unity projects.
    /// </summary>
    public enum PathType
    {
        /// <summary>
        /// The path is not set.
        /// </summary>
        Unknown,

        /// <summary>
        /// The path is an absolute path on the local file system.
        /// </summary>
        Local,

        /// <summary>
        /// The path is relative to the project's <see cref="Application.dataPath"/>.
        /// </summary>
        DataPath,

        /// <summary>
        /// The path is relative to the project's <see cref="Application.dataPath"/> + <c>"/Resources"</c>.
        /// </summary>
        Resources,

        /// <summary>
        /// The path is relative to the project's <see cref="Application.streamingAssetsPath"/>.
        /// </summary>
        StreamingAssetsPath,

        /// <summary>
        /// The path is relative to the project's <see cref="Application.persistentDataPath"/>.
        /// This path is used to store data that should persist between app launches.
        /// </summary>
        PersistentDataPath,

        /// <summary>
        /// The path is a URL to a resource on the internet.
        /// </summary>
        WebUrl,

        /// <summary>
        /// The path is relative to the project's <see cref="Application.temporaryCachePath"/>.
        /// </summary>
        TemporaryCachePath,

        /// <summary>
        /// The path is relative to the project's <see cref="Application.consoleLogPath"/>.
        /// </summary>
        ConsoleLogPath
    }
}