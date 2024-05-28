namespace Glitch9.IO.RESTApi
{
    public class RESTLog
    {
        private static class RESTLogTags
        {
            internal const string REQUEST_INFO = "RequestInfo";
            internal const string REQUEST_HEADER = "RequestHeader";
            internal const string REQUEST_DETAILS = "RequestDetails";
            internal const string REQUEST_BODY = "RequestBody";
            internal const string REQUEST_ERROR = "RequestError";
            internal const string RESPONSE_BODY = "ResponseBody";
            internal const string RESPONSE_ERROR = "ResponseError";
            internal const string STREAMED_DATA = "StreamedData";
        }

        public static void RequestInfo(string info) => GNLog.Info(RESTLogTags.REQUEST_INFO, info);
        public static void RequestBody(string body) => GNLog.Info(RESTLogTags.REQUEST_BODY, body);
        public static void RequestDetails(string details) => GNLog.Info(RESTLogTags.REQUEST_DETAILS, details);
        public static void RequestHeader(string header) => GNLog.Info(RESTLogTags.REQUEST_HEADER, header);
        public static void ResponseBody(string body) => GNLog.Info(RESTLogTags.RESPONSE_BODY, body);
        public static void StreamedData(string data) => GNLog.Info(RESTLogTags.STREAMED_DATA, data);
        public static void RequestError(string error) => GNLog.Error(RESTLogTags.REQUEST_ERROR, error);
        public static void ResponseError(string error) => GNLog.Error(RESTLogTags.RESPONSE_ERROR, error);
    }
}