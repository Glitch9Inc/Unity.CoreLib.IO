using System;

namespace Glitch9.IO.RESTApi
{
    /// <summary>
    /// CRUD is abbreviation for Create, Read, Update, Delete.
    /// It is used to define the operations that can be performed on a REST resource.
    /// </summary>
    [Flags]
    public enum CRUDMethod
    {
        Unset = 0,

        /// <summary>
        /// Creates REST resource using <seealso cref="HttpMethod.POST"/>.
        /// </summary>
        Create = 1 << 0,

        /// <summary>
        /// Deletes REST resource using <seealso cref="HttpMethod.DELETE"/>.
        /// </summary>
        Delete = 1 << 1,

        /// <summary>
        /// Gets REST resource using <seealso cref="HttpMethod.GET"/>.
        /// </summary>
        Get = 1 << 2,

        /// <summary>
        /// Gets REST resource using <seealso cref="HttpMethod.GET"/>.
        /// It's an alias for <seealso cref="Get"/>.
        /// </summary>
        Retrieve = 1 << 3,

        /// <summary>
        /// Updates REST resource using <seealso cref="HttpMethod.PATCH"/>.
        /// </summary>
        Patch = 1 << 4,

        /// <summary>
        /// Updates REST resource using <seealso cref="HttpMethod.POST"/>.
        /// Unlike <seealso cref="Patch"/>, this requires a body.
        /// <para>Also known as 'Modify'</para>
        /// </summary>
        Update = 1 << 5,

        /// <summary>
        /// Queries a list of REST resources using <seealso cref="HttpMethod.GET"/>.
        /// </summary>
        List = 1 << 6,

        /// <summary>
        /// Cancels a REST resource (which usually is a long-running operation) using <seealso cref="HttpMethod.POST"/>.
        /// </summary>
        Cancel = 1 << 7,

        /// <summary>
        /// Queries a REST resource using <seealso cref="HttpMethod.POST"/>.
        /// Unlike <seealso cref="List"/>, this requires a body.
        /// </summary>
        Query = 1 << 8,

        /// <summary>
        /// Uses WebSocket to communicate with the server.
        /// </summary>
        WebSocket = 1 << 9,
    }
}