namespace Glitch9.IO.RESTApi
{
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
        Retry
    }
}