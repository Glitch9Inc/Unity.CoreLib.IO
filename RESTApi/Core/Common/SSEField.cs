namespace Glitch9.IO.RESTApi
{
    /// <summary>
    /// Server-Sent Event field keys.
    /// </summary>
    public enum SSEField
    {
        Unset = 0,
        [ApiEnum("id")]
        Id,
        [ApiEnum("event")]
        Event,
        [ApiEnum("data")]
        Data,
        [ApiEnum("retry")]
        Retry,
        [ApiEnum("error")]
        Error
    }
}