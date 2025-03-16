namespace Glitch9.IO.RESTApi
{
    public enum AutoParam
    {
        Unset,
        Path,
        Query,
        Header
    }

    public class CRUDClientSettings
    {
        /// <summary>
        /// Required.
        /// The name of the API.
        /// <para>Example: "OpenAI"</para>
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Required.
        /// This is the version of the API, in case it changes, it will be updated here.
        /// <para>Example: "v1"</para>
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Optional.
        /// This is the beta version of the API, in case it changes, it will be updated here.
        /// <para>Example: "v1beta"</para>
        /// </summary>
        public string BetaApiVersion { get; set; }

        /// <summary>
        /// Optional.
        /// The default base url for all the requests.
        /// <para>Example: "https://api.openai.com"</para>
        /// </summary>
        public string BaseURL { get; set; }

        /// <summary>
        /// Required.
        /// Indicates how the API key should be sent.
        /// </summary>
        public AutoParam AutoApiKey { get; set; }

        /// <summary>
        /// Required if <see cref="AutoApiKey"/> is set to <see cref="AutoParam.Path"/>,
        /// or disregarded otherwise.
        /// </summary>
        public string ApiKeyQueryKey { get; set; }

        /// <summary>
        /// Required if <see cref="AutoApiKey"/> is set.
        /// </summary>
        public ApiKeyGetter ApiKeyGetter { get; set; }

        /// <summary>
        /// Optional.
        /// If the API requires a specific parameter for versioning, it will be set here. 
        /// </summary>
        public AutoParam AutoVersionParam { get; set; }

        /// <summary>
        /// Optional.
        /// If the API requires a specific parameter for beta versioning, it will be set here.
        /// </summary>
        public AutoParam AutoBetaParam { get; set; }

        /// <summary>
        /// Required if <see cref="AutoVersionParam"/> is set to <see cref="AutoParam.Header"/>,
        /// or disregarded otherwise.
        /// </summary>
        public RESTHeader? BetaHeader { get; set; }

        /// <summary>
        /// Optional.
        /// Add additional headers to the request.
        /// </summary>
        public RESTHeader[] AdditionalHeaders { get; set; }

        /// <summary>
        /// Optional.
        /// Indicates an error in the SSE response.
        /// <para>Example: "error"</para>
        /// </summary>
        public string SSEError { get; set; }

        /// <summary>
        /// Optional.
        /// Indicates the end of the SSE response. (like the end of a stream)
        /// <para>Example: "DONE"</para>
        /// </summary>
        public string SSEDone { get; set; }
    }
}