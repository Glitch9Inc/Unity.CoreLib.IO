namespace Glitch9.IO.Files
{
    /// <summary>
    /// Specifies different types of paths used in Unity projects.
    /// </summary>
    public enum UnityPath
    {
        /// <summary>
        /// The path is not set.
        /// </summary>
        Unset,

        /// <summary>
        /// The path is relative to the project's Assets folder.
        /// </summary>
        Assets,

        /// <summary>
        /// The path is relative to the project's Resources folder.
        /// </summary>
        Resources,

        /// <summary>
        /// The path is relative to the project's StreamingAssets folder.
        /// </summary>
        StreamingAsset,

        /// <summary>
        /// The path is relative to the project's PersistentDataPath.
        /// This path is used to store data that should persist between app launches.
        /// </summary>
        PersistentData,

        /// <summary>
        /// The path is a URL to a resource on the internet.
        /// </summary>
        URL,
    }
}