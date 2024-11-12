using System;
using Cysharp.Threading.Tasks;
using Glitch9.IO.Network;
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
            internal const bool DEFAULT_LOG_REQUEST_HEADERS = true;
            internal const bool DEFAULT_LOG_REQUEST_BODY = true;
            internal const bool DEFAULT_LOG_REQUEST_INFO = true;
            internal const bool DEFAULT_LOG_REQUEST_DETAILS = true;
            internal const bool DEFAULT_LOG_RESPONSE_BODY = true;
            internal const bool DEFAULT_LOG_STREAMED_DATA = true;
            internal static readonly TimeSpan DEFAULT_TIMEOUT = TimeSpan.FromSeconds(30);
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
        /// Gets or sets the logger.
        /// You can set a custom logger to handle logging.
        /// </summary>
        public ILogger Logger { get; set; }

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
        /// Gets or sets the timeout for requests.
        /// </summary>
        public TimeSpan Timeout { get; set; } = Config.DEFAULT_TIMEOUT;

        /// <summary>
        /// Gets or sets the SSE parser for handling Server-Sent Events.
        /// </summary>
        public ISSEParser SSEParser { get; set; }

        /// <summary>
        /// Name of the last request made.
        /// </summary>
        public string LastRequest { get; set; } = "";

        /// <summary>
        /// Gets or sets the last endpoint used.
        /// </summary>
        public string LastEndpoint { get; set; } = "";

        // internal
        internal InternalNetLogger InternalLogger { get; set; }


        /// <summary>
        /// Constructor to initialize RESTClient with optional JSON settings.
        /// </summary>
        /// <param name="jsonSettings">Custom JSON serializer settings.</param>
        /// <param name="sseParser">Custom SSE parser.</param>
        /// <param name="logger">Custom logger.</param>
        public RESTClient(JsonSerializerSettings jsonSettings = null, ISSEParser sseParser = null, ILogger logger = null)
        {
            JsonSettings = jsonSettings ?? JsonUtils.DefaultSettings;
            SSEParser = sseParser;
            Logger = logger ?? new NetLogger("RESTApi");
            InternalLogger = new InternalNetLogger(Logger);
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
            return await RESTApiV3.SendRequest<TReq, RESTResponse>(request, UnityWebRequest.kHttpVerbPOST, this)
                .AttachExternalCancellation(request.CancellationToken);
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
            where TRes : RESTResponse, new()
        {
            return await RESTApiV3.SendRequest<TReq, TRes>(request, UnityWebRequest.kHttpVerbPOST, this)
                .AttachExternalCancellation(request.CancellationToken);
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
            return await RESTApiV3.SendRequest<TReq, RESTResponse>(request, UnityWebRequest.kHttpVerbPUT, this)
                .AttachExternalCancellation(request.CancellationToken);
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
            where TRes : RESTResponse, new()
        {
            return await RESTApiV3.SendRequest<TReq, TRes>(request, UnityWebRequest.kHttpVerbPUT, this)
                .AttachExternalCancellation(request.CancellationToken);
        }

        /// <summary>
        /// Sends a GET request with a generic request type and default response and error types.
        /// </summary>
        /// <typeparam name="TRes">Response type.</typeparam>
        /// <param name="request">Request object.</param>
        /// <returns>Response result.</returns>
        public virtual async UniTask<IResult> GET<TRes>(RESTRequest request)
            where TRes : RESTResponse, new()
        {
            return await RESTApiV3.SendRequest<RESTRequest, TRes>(request, UnityWebRequest.kHttpVerbGET, this)
                .AttachExternalCancellation(request.CancellationToken);
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
            where TRes : RESTResponse, new()
        {
            return await RESTApiV3.SendRequest<TReq, TRes>(request, UnityWebRequest.kHttpVerbGET, this)
                .AttachExternalCancellation(request.CancellationToken);
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
            return await RESTApiV3.SendRequest<TReq, RESTResponse>(request, UnityWebRequest.kHttpVerbDELETE, this)
                .AttachExternalCancellation(request.CancellationToken);
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
            where TRes : RESTResponse, new()
        {
            return await RESTApiV3.SendRequest<TReq, TRes>(request, UnityWebRequest.kHttpVerbDELETE, this)
                .AttachExternalCancellation(request.CancellationToken);
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
            return await RESTApiV3.SendRequest<TReq, RESTResponse>(request, UnityWebRequest.kHttpVerbHEAD, this)
                .AttachExternalCancellation(request.CancellationToken);
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
            where TRes : RESTResponse, new()
        {
            return await RESTApiV3.SendRequest<TReq, TRes>(request, UnityWebRequest.kHttpVerbHEAD, this)
                .AttachExternalCancellation(request.CancellationToken);
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
            return await RESTApiV3.SendRequest<TReq, RESTResponse>(request, Config.METHOD_PATCH, this)
                .AttachExternalCancellation(request.CancellationToken);
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
            where TRes : RESTResponse, new()
        {
            return await RESTApiV3.SendRequest<TReq, TRes>(request, Config.METHOD_PATCH, this)
                .AttachExternalCancellation(request.CancellationToken);
        }


        /// <summary>
        /// Smart method to execute a CRUD operation.
        /// </summary>
        /// <typeparam name="TReq"></typeparam>
        /// <typeparam name="TRes"></typeparam>
        /// <param name="method"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public virtual async UniTask<IResult> ExecuteAsync<TReq, TRes>(CRUDMethod method, TReq request)
            where TReq : RESTRequest
            where TRes : RESTResponse, new()
        {
            switch (method)
            {
                case CRUDMethod.Create:
                case CRUDMethod.Query:
                case CRUDMethod.Update:
                    return await POST<TReq, TRes>(request);
                case CRUDMethod.Get:
                case CRUDMethod.Retrieve:
                case CRUDMethod.List:
                    return await GET<TReq, TRes>(request);
                case CRUDMethod.Delete:
                    return await DELETE<TReq, TRes>(request);
                case CRUDMethod.Patch:
                    return await PATCH<TReq, TRes>(request);
                default:
                    throw new ArgumentOutOfRangeException(nameof(method), method, null);
            }
        }
    }
}
