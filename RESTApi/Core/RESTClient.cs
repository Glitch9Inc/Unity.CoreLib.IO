using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine.Networking;
// ReSharper disable InconsistentNaming

namespace Glitch9.IO.RESTApi
{
    /// <summary>
    /// A REST client class for handling various types of REST API requests.
    /// </summary>
    public class RESTClient
    {
        private const string METHOD_PATCH = "PATCH";
        
        /// <summary>
        /// Gets or sets the JSON serializer settings.
        /// </summary>
        public JsonSerializerSettings JsonSettings { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether request headers should be logged.
        /// </summary>
        public bool LogRequestHeaders { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the request body should be logged.
        /// </summary>
        public bool LogRequestBody { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether basic request information should be logged.
        /// </summary>
        public bool LogRequestInfo { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether detailed request information should be logged.
        /// </summary>
        public bool LogRequestDetails { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the response body should be logged.
        /// </summary>
        public bool LogResponseBody { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether streamed data should be logged.
        /// </summary>
        public bool LogStreamedData { get; set; }


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
            return await SendRequest<TReq, RESTResult, Error>(request, UnityWebRequest.kHttpVerbPOST);
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
            where TRes : RESTResult, new()
        {
            return await SendRequest<TReq, TRes, Error>(request, UnityWebRequest.kHttpVerbPOST);
        }

        /// <summary>
        /// Sends a POST request with generic request, response, and error types.
        /// </summary>
        /// <typeparam name="TReq">Request type.</typeparam>
        /// <typeparam name="TRes">Response type.</typeparam>
        /// <typeparam name="TErr">Error type.</typeparam>
        /// <param name="request">Request object.</param>
        /// <returns>Response result.</returns>
        public virtual async UniTask<IResult> POST<TReq, TRes, TErr>(TReq request)
            where TReq : RESTRequest
            where TRes : RESTResult, new()
            where TErr : Error, new()
        {
            return await SendRequest<TReq, TRes, TErr>(request, UnityWebRequest.kHttpVerbPOST);
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
            return await SendRequest<TReq, RESTResult, Error>(request, UnityWebRequest.kHttpVerbPUT);
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
            where TRes : RESTResult, new()
        {
            return await SendRequest<TReq, TRes, Error>(request, UnityWebRequest.kHttpVerbPUT);
        }

        /// <summary>
        /// Sends a PUT request with generic request, response, and error types.
        /// </summary>
        /// <typeparam name="TReq">Request type.</typeparam>
        /// <typeparam name="TRes">Response type.</typeparam>
        /// <typeparam name="TErr">Error type.</typeparam>
        /// <param name="request">Request object.</param>
        /// <returns>Response result.</returns>
        public virtual async UniTask<IResult> PUT<TReq, TRes, TErr>(TReq request)
            where TReq : RESTRequest
            where TRes : RESTResult, new()
            where TErr : Error, new()
        {
            return await SendRequest<TReq, TRes, TErr>(request, UnityWebRequest.kHttpVerbPUT);
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
            return await SendRequest<TReq, RESTResult, Error>(request, UnityWebRequest.kHttpVerbGET);
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
            where TRes : RESTResult, new()
        {
            return await SendRequest<TReq, TRes, Error>(request, UnityWebRequest.kHttpVerbGET);
        }

        /// <summary>
        /// Sends a GET request with generic request, response, and error types.
        /// </summary>
        /// <typeparam name="TReq">Request type.</typeparam>
        /// <typeparam name="TRes">Response type.</typeparam>
        /// <typeparam name="TErr">Error type.</typeparam>
        /// <param name="request">Request object.</param>
        /// <returns>Response result.</returns>
        public virtual async UniTask<IResult> GET<TReq, TRes, TErr>(TReq request)
            where TReq : RESTRequest
            where TRes : RESTResult, new()
            where TErr : Error, new()
        {
            return await SendRequest<TReq, TRes, TErr>(request, UnityWebRequest.kHttpVerbGET);
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
            return await SendRequest<TReq, RESTResult, Error>(request, UnityWebRequest.kHttpVerbDELETE);
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
            where TRes : RESTResult, new()
        {
            return await SendRequest<TReq, TRes, Error>(request, UnityWebRequest.kHttpVerbDELETE);
        }

        /// <summary>
        /// Sends a DELETE request with generic request, response, and error types.
        /// </summary>
        /// <typeparam name="TReq">Request type.</typeparam>
        /// <typeparam name="TRes">Response type.</typeparam>
        /// <typeparam name="TErr">Error type.</typeparam>
        /// <param name="request">Request object.</param>
        /// <returns>Response result.</returns>
        public virtual async UniTask<IResult> DELETE<TReq, TRes, TErr>(TReq request)
            where TReq : RESTRequest
            where TRes : RESTResult, new()
            where TErr : Error, new()
        {
            return await SendRequest<TReq, TRes, TErr>(request, UnityWebRequest.kHttpVerbDELETE);
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
            return await SendRequest<TReq, RESTResult, Error>(request, UnityWebRequest.kHttpVerbHEAD);
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
            where TRes : RESTResult, new()
        {
            return await SendRequest<TReq, TRes, Error>(request, UnityWebRequest.kHttpVerbHEAD);
        }

        /// <summary>
        /// Sends a HEAD request with generic request, response, and error types.
        /// </summary>
        /// <typeparam name="TReq">Request type.</typeparam>
        /// <typeparam name="TRes">Response type.</typeparam>
        /// <typeparam name="TErr">Error type.</typeparam>
        /// <param name="request">Request object.</param>
        /// <returns>Response result.</returns>
        public virtual async UniTask<IResult> HEAD<TReq, TRes, TErr>(TReq request)
            where TReq : RESTRequest
            where TRes : RESTResult, new()
            where TErr : Error, new()
        {
            return await SendRequest<TReq, TRes, TErr>(request, UnityWebRequest.kHttpVerbHEAD);
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
            return await SendRequest<TReq, RESTResult, Error>(request, METHOD_PATCH);
        }

        /// <summary>
        /// Sends a PATCH request with a generic request and response type.
        /// </summary>
        /// <typeparam name="TReq">Request type.</typeparam>
        /// <typeparam name="TRes">Response type.</typeparam>
        /// <param name="request">Request object.</param>
        /// <returns>Response result.</returns>
        public virtual async UniTask<IResult> PATCH<TReq, TRes>(TReq request)
            where TReq : RESTRequest
            where TRes : RESTResult, new()
        {
            return await SendRequest<TReq, TRes, Error>(request, METHOD_PATCH);
        }

        /// <summary>
        /// Sends a PATCH request with generic request, response, and error types.
        /// </summary>
        /// <typeparam name="TReq">Request type.</typeparam>
        /// <typeparam name="TRes">Response type.</typeparam>
        /// <typeparam name="TErr">Error type.</typeparam>
        /// <param name="request">Request object.</param>
        /// <returns>Response result.</returns>
        public virtual async UniTask<IResult> PATCH<TReq, TRes, TErr>(TReq request)
            where TReq : RESTRequest where
            TRes : RESTResult, new() where
            TErr : Error, new()
        {
            return await SendRequest<TReq, TRes, TErr>(request, METHOD_PATCH);
        }

        /// <summary>
        /// Sends a request and processes the response.
        /// </summary>
        /// <typeparam name="TReq">Request type.</typeparam>
        /// <typeparam name="TRes">Response type.</typeparam>
        /// <typeparam name="TErr">Error type.</typeparam>
        /// <param name="request">Request object.</param>
        /// <param name="method">HTTP method to use for the request.</param>
        /// <returns>Response result.</returns>
        private async UniTask<IResult> SendRequest<TReq, TRes, TErr>(TReq request, string method)
            where TReq : RESTRequest
            where TRes : RESTResult, new()
            where TErr : Error, new()
        {
            try
            {
                // Step 1. Validating request ==========================================================================================================================
                if (request == null) throw new IssueException(Issue.InvalidRequest, "Request is null.");
                if (string.IsNullOrEmpty(request.Endpoint)) throw new IssueException(Issue.InvalidEndpoint, "Endpoint is null or empty.");

                if (LogRequestInfo) RESTLog.RequestInfo($"Sending {method} request to {request.Endpoint}.");
                await RESTApiV3.CheckNetworkAsync();
                using UnityWebRequest webReq = request.CreateUnityWebRequest(method, LogRequestBody, LogRequestHeaders, JsonSettings);

                if (webReq == null) throw new IssueException(Issue.InvalidRequest, "UnityWebRequest is null.");

                // Step 2. Sending request =============================================================================================================================
                Result taskResult = await RESTApiV3.SendAndProcessRequest<TErr>(webReq, request.RetryDelayInSec, request.MaxRetry);

                // Step 3. Handling response errors ====================================================================================================================
                if (taskResult == null) throw new IssueException(Issue.ReceiveFailed, "TaskResult is null.");
                if (taskResult is Error taskError) throw new IssueException(Issue.ReceiveFailed, taskError.ToString());

                // Step 4. Handling 'Stream' response ===================================================================================================================
                bool isStream = request.DownloadMode is DownloadMode.TextStream or DownloadMode.BinaryStream;
                if (isStream)
                {
                    // If it's a stream, everything is handled within the SendAndProcessRequest method
                    if (LogStreamedData) RESTLog.RequestInfo("Stream has ended.");
                    return RESTResult.Done(); // Let the caller know that the stream has ended
                }

                if (webReq.downloadHandler == null) throw new IssueException(Issue.EmptyResponse, "DownloadHandler is null.");
                if (string.IsNullOrEmpty(webReq.downloadHandler.text)) throw new IssueException(Issue.EmptyResponse, "DownloadHandler text is null or empty.");

                // Step 5. Handling response ============================================================================================================================
                if (LogRequestInfo) RESTLog.RequestInfo($"Received response from {request.Endpoint}");

                if (request.DownloadMode == DownloadMode.Text)
                {
                    if (LogRequestDetails) RESTLog.RequestDetails("Download Mode: Text");
                    byte[] binaryResult = webReq.downloadHandler.data;
                    string textResult = webReq.downloadHandler.text;

                    if (string.IsNullOrEmpty(textResult)) throw new IssueException(Issue.EmptyResponse, "Text result is null or empty.");
                    if (LogResponseBody) RESTLog.ResponseBody(textResult);

                    if (RESTApiV3.TryGetError(textResult, JsonSettings, out TErr error)) return error;
                    return await TextResponseConverter.ConvertAsync<TRes>(binaryResult, textResult, request.FilePath, request.ContentType, JsonSettings);
                }

                if (request.DownloadMode == DownloadMode.Binary)
                {
                    if (LogRequestDetails) RESTLog.RequestDetails("Download Mode: Binary");
                    byte[] binaryResult = webReq.downloadHandler.data;
                    string textResult = webReq.downloadHandler.text;

                    if (binaryResult == null || binaryResult.Length == 0)
                    {
                        RESTLog.ResponseError("Binary result is null or empty");
                        if (string.IsNullOrEmpty(textResult)) throw new IssueException(Issue.EmptyResponse, "Text result is also null or empty");
                        if (RESTApiV3.TryGetError(textResult, JsonSettings, out TErr error)) return error;
                    }

                    return await BinaryResponseConverter.ConvertAsync<TRes>(binaryResult, textResult, request.FilePath, request.ResponseContentType);
                }

                throw new IssueException(Issue.UnknownError);
            }
            catch (Exception e)
            {
                List<string> messages = new() { e.Convert().GetMessage() };
                return new TErr() { ErrorMessages = messages, StackTrace = e.StackTrace };
            }
        }
    }
}
