namespace Glitch9.IO.Network
{
    internal class InternalNetLogger
    {
        private static class LoggerTags
        {
            internal const string REQUEST_INFO = "Request Info";
            internal const string REQUEST_HEADERS = "Request Headers";
            internal const string REQUEST_DETAILS = "Request Details";
            internal const string REQUEST_BODY = "Request Body";
            internal const string REQUEST_ERROR = "Request Error";
            internal const string RESPONSE_INFO = "Response Info";
            internal const string RESPONSE_BODY = "Response Body";
            internal const string RESPONSE_ERROR = "Response Error";
            internal const string STREAMED_DATA = "Streamed Data";
        }

        internal void RequestInfo(string info) => _logger.Info(LoggerTags.REQUEST_INFO, info);
        internal void RequestBody(string body) => _logger.Info(LoggerTags.REQUEST_BODY, body);
        internal void RequestDetails(string details) => _logger.Info(LoggerTags.REQUEST_DETAILS, details);
        internal void RequestHeaders(string header) => _logger.Info(LoggerTags.REQUEST_HEADERS, header);
        internal void ResponseInfo(string info) => _logger.Info(LoggerTags.RESPONSE_INFO, info);
        internal void ResponseBody(string body) => _logger.Info(LoggerTags.RESPONSE_BODY, body);
        internal void StreamEvent(string data) => _logger.Info(LoggerTags.STREAMED_DATA, data);
        internal void RequestError(string error) => _logger.Error(LoggerTags.REQUEST_ERROR, error);
        internal void ResponseError(string error) => _logger.Error(LoggerTags.RESPONSE_ERROR, error);

        private readonly ILogger _logger;

        internal InternalNetLogger(ILogger logger)
        {
            ThrowIf.ArgumentIsNull(logger);
            _logger = logger;
        }
    }
}