using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine.Networking;
// ReSharper disable InconsistentNaming

namespace Glitch9.IO.RESTApi
{
    /// <summary>
    /// A REST client class for handling various types of REST API requests.
    /// </summary>
    public class RESTClient
    {
        public static class Config
        {
            internal const string METHOD_PATCH = "PATCH";
            internal const int NETWORK_CHECK_INTERVAL_IN_MILLIS = 500;  // 0.5 seconds
            internal const int NETWORK_CHECK_TIMEOUT_IN_MILLIS = 10000; // 10 seconds
            internal const bool DEFAULT_LOG_REQUEST_HEADERS = false;
            internal const bool DEFAULT_LOG_REQUEST_BODY = false;
            internal const bool DEFAULT_LOG_REQUEST_INFO = false;
            internal const bool DEFAULT_LOG_REQUEST_DETAILS = false;
            internal const bool DEFAULT_LOG_RESPONSE_BODY = false;
            internal const bool DEFAULT_LOG_STREAMED_DATA = false;
        }

        /// <summary>
        /// Gets or sets the JSON serializer settings.
        /// You can add custom converters or other settings here.
        /// </summary>
        /// <remarks>
        /// [Example] restClient.JsonSettings.Converters.Add(new MyCustomConverter());
        /// </remarks>
        public JsonSerializerSettings JsonSettings { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether request headers should be logged.
        /// </summary>
        public virtual bool LogRequestHeaders { get; set; } = Config.DEFAULT_LOG_REQUEST_HEADERS;

        /// <summary>
        /// Gets or sets a value indicating whether the request body should be logged.
        /// </summary>
        public virtual bool LogRequestBody { get; set; } = Config.DEFAULT_LOG_REQUEST_BODY;

        /// <summary>
        /// Gets or sets a value indicating whether basic request information should be logged.
        /// </summary>
        public virtual bool LogRequestInfo { get; set; } = Config.DEFAULT_LOG_REQUEST_INFO;

        /// <summary>
        /// Gets or sets a value indicating whether detailed request information should be logged.
        /// </summary>
        public virtual bool LogRequestDetails { get; set; } = Config.DEFAULT_LOG_REQUEST_DETAILS;

        /// <summary>
        /// Gets or sets a value indicating whether the response body should be logged.
        /// </summary>
        public virtual bool LogResponseBody { get; set; } = Config.DEFAULT_LOG_RESPONSE_BODY;

        /// <summary>
        /// Gets or sets a value indicating whether streamed data should be logged.
        /// </summary>
        public virtual bool LogStreamEvents { get; set; } = Config.DEFAULT_LOG_STREAMED_DATA;

        /// <summary>
        /// Gets or sets the SSE parser for handling Server-Sent Events.
        /// </summary>
        public SSEParser SSEParser { get; set; } = new SSEParser();


        /// <summary>
        /// Constructor to initialize RESTClient with optional JSON settings.
        /// </summary>
        /// <param name="jsonSettings">Custom JSON serializer settings.</param>
        public RESTClient(JsonSerializerSettings jsonSettings = null)
        {
            JsonSettings = jsonSettings ?? JsonUtils.DefaultSettings;
        }

        /// <summary>
        /// Sends a POST request with a generic request type and default response and error types.
        /// </summary>
        /// <typeparam name="TReq">Request type.</typeparam>
        /// <param name="request">Request object.</param>
        /// <returns>Response result.</returns>
        public virtual async UniTask<IResult> POST<TReq>(TReq request)
            where TReq : RESTRequest
        {
            return await RESTApiV3.SendRequest<TReq, RESTObject>(request, UnityWebRequest.kHttpVerbPOST, this);
        }

        /// <summary>
        /// Sends a POST request with a generic request and response type.
        /// </summary>
        /// <typeparam name="TReq">Request type.</typeparam>
        /// <typeparam name="TRes">Response type.</typeparam>
        /// <param name="request">Request object.</param>
        /// <returns>Response result.</returns>
        public virtual async UniTask<IResult> POST<TReq, TRes>(TReq request)
            where TReq : RESTRequest
            where TRes : RESTObject, new()
        {
            return await RESTApiV3.SendRequest<TReq, TRes>(request, UnityWebRequest.kHttpVerbPOST, this);
        }

        /// <summary>
        /// Sends a PUT request with a generic request type and default response and error types.
        /// </summary>
        /// <typeparam name="TReq">Request type.</typeparam>
        /// <param name="request">Request object.</param>
        /// <returns>Response result.</returns>
        public virtual async UniTask<IResult> PUT<TReq>(TReq request)
            where TReq : RESTRequest
        {
            return await RESTApiV3.SendRequest<TReq, RESTObject>(request, UnityWebRequest.kHttpVerbPUT, this);
        }

        /// <summary>
        /// Sends a PUT request with a generic request and response type.
        /// </summary>
        /// <typeparam name="TReq">Request type.</typeparam>
        /// <typeparam name="TRes">Response type.</typeparam>
        /// <param name="request">Request object.</param>
        /// <returns>Response result.</returns>
        public virtual async UniTask<IResult> PUT<TReq, TRes>(TReq request)
            where TReq : RESTRequest
            where TRes : RESTObject, new()
        {
            return await RESTApiV3.SendRequest<TReq, TRes>(request, UnityWebRequest.kHttpVerbPUT, this);
        }

        /// <summary>
        /// Sends a GET request with a generic request type and default response and error types.
        /// </summary>
        /// <typeparam name="TReq">Request type.</typeparam>
        /// <param name="request">Request object.</param>
        /// <returns>Response result.</returns>
        public virtual async UniTask<IResult> GET<TReq>(TReq request)
            where TReq : RESTRequest
        {
            return await RESTApiV3.SendRequest<TReq, RESTObject>(request, UnityWebRequest.kHttpVerbGET, this);
        }

        /// <summary>
        /// Sends a GET request with a generic request and response type.
        /// </summary>
        /// <typeparam name="TReq">Request type.</typeparam>
        /// <typeparam name="TRes">Response type.</typeparam>
        /// <param name="request">Request object.</param>
        /// <returns>Response result.</returns>
        public virtual async UniTask<IResult> GET<TReq, TRes>(TReq request)
            where TReq : RESTRequest
            where TRes : RESTObject, new()
        {
            return await RESTApiV3.SendRequest<TReq, TRes>(request, UnityWebRequest.kHttpVerbGET, this);
        }

        /// <summary>
        /// Sends a DELETE request with a generic request type and default response and error types.
        /// </summary>
        /// <typeparam name="TReq">Request type.</typeparam>
        /// <param name="request">Request object.</param>
        /// <returns>Response result.</returns>
        public virtual async UniTask<IResult> DELETE<TReq>(TReq request)
            where TReq : RESTRequest
        {
            return await RESTApiV3.SendRequest<TReq, RESTObject>(request, UnityWebRequest.kHttpVerbDELETE, this);
        }

        /// <summary>
        /// Sends a DELETE request with a generic request and response type.
        /// </summary>
        /// <typeparam name="TReq">Request type.</typeparam>
        /// <typeparam name="TRes">Response type.</typeparam>
        /// <param name="request">Request object.</param>
        /// <returns>Response result.</returns>
        public virtual async UniTask<IResult> DELETE<TReq, TRes>(TReq request)
            where TReq : RESTRequest
            where TRes : RESTObject, new()
        {
            return await RESTApiV3.SendRequest<TReq, TRes>(request, UnityWebRequest.kHttpVerbDELETE, this);
        }

        /// <summary>
        /// Sends a HEAD request with a generic request type and default response and error types.
        /// </summary>
        /// <typeparam name="TReq">Request type.</typeparam>
        /// <param name="request">Request object.</param>
        /// <returns>Response result.</returns>
        public virtual async UniTask<IResult> HEAD<TReq>(TReq request)
            where TReq : RESTRequest
        {
            return await RESTApiV3.SendRequest<TReq, RESTObject>(request, UnityWebRequest.kHttpVerbHEAD, this);
        }

        /// <summary>
        /// Sends a HEAD request with a generic request and response type.
        /// </summary>
        /// <typeparam name="TReq">Request type.</typeparam>
        /// <typeparam name="TRes">Response type.</typeparam>
        /// <param name="request">Request object.</param>
        /// <returns>Response result.</returns>
        public virtual async UniTask<IResult> HEAD<TReq, TRes>(TReq request)
            where TReq : RESTRequest
            where TRes : RESTObject, new()
        {
            return await RESTApiV3.SendRequest<TReq, TRes>(request, UnityWebRequest.kHttpVerbHEAD, this);
        }

        /// <summary>
        /// Sends a PATCH request with a generic request type and default response and error types.
        /// </summary>
        /// <typeparam name="TReq">Request type.</typeparam>
        /// <param name="request">Request object.</param>
        /// <returns>Response result.</returns>
        public virtual async UniTask<IResult> PATCH<TReq>(TReq request)
            where TReq : RESTRequest
        {
            return await RESTApiV3.SendRequest<TReq, RESTObject>(request, Config.METHOD_PATCH, this);
        }

        /// <summary>
        /// Sends a PATCH request with generic request, response, and error types.
        /// </summary>
        /// <typeparam name="TReq">Request type.</typeparam>
        /// <typeparam name="TRes">Response type.</typeparam>
        /// <param name="request">Request object.</param>
        /// <returns>Response result.</returns>
        public virtual async UniTask<IResult> PATCH<TReq, TRes>(TReq request)
            where TReq : RESTRequest
            where TRes : RESTObject, new()
        {
            return await RESTApiV3.SendRequest<TReq, TRes>(request, Config.METHOD_PATCH, this);
        }
    }
}
