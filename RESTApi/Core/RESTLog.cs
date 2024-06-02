namespace Glitch9.IO.RESTApi
{
    public class RESTLog
    {
        private static class RESTLogTags
        {
            internal const string REQUEST_INFO = "Request Info";
            internal const string REQUEST_HEADERS = "Request Headers";
            internal const string REQUEST_DETAILS = "Request Details";
            internal const string REQUEST_BODY = "Request Body";
            internal const string REQUEST_ERROR = "Request Error";
            internal const string RESPONSE_BODY = "Response Body";
            internal const string RESPONSE_ERROR = "Response Error";
            internal const string STREAMED_DATA = "Streamed Data";
        }

        public static void RequestInfo(string info) => GNLog.Info(RESTLogTags.REQUEST_INFO, info);
        public static void RequestBody(string body) => GNLog.Info(RESTLogTags.REQUEST_BODY, body);
        public static void RequestDetails(string details) => GNLog.Info(RESTLogTags.REQUEST_DETAILS, details);
        public static void RequestHeaders(string header) => GNLog.Info(RESTLogTags.REQUEST_HEADERS, header);
        public static void ResponseBody(string body) => GNLog.Info(RESTLogTags.RESPONSE_BODY, body);
        public static void StreamedData(string data) => GNLog.Info(RESTLogTags.STREAMED_DATA, data);
        public static void RequestError(string error) => GNLog.Error(RESTLogTags.REQUEST_ERROR, error);
        public static void ResponseError(string error) => GNLog.Error(RESTLogTags.RESPONSE_ERROR, error);
    }
}