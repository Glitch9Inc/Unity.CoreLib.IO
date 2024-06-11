using Cysharp.Threading.Tasks;
using Glitch9.IO.Files;
using System;
using Glitch9.IO.Network;
using UnityEngine.Networking;
using static Glitch9.IO.RESTApi.RESTClient;

namespace Glitch9.IO.RESTApi
{
    public static class RESTApiV3
    {
        #region File History

        /// 버전 3.1 업데이트: 2023-12-17 @Munchkin
        /// - UniTask를 사용하도록 변경
        /// - 에러 핸들링 로직 변경
        /// 
        /// 버전 3.2 업데이트: 2024-02-19 @Munchkin
        /// - GetAuthHeaderFieldName() 추가
        /// - Timeout 로직 변경
        /// - 최소 Delay값 확인 추가
        /// 
        /// 버전 3.3 업데이트: 2024-02-28 @Munchkin
        /// - TErr 추가 (에러오브젝트 기능)
        ///
        /// 버전 4.0 업데이트: 2024-05-28 @Munchkin
        /// - 인터넷 연결 체크 함수 이동 (NetworkUtils.cs)
        /// - 더이상 Exception을 catch하지않고 throw만 함
        ///
        /// 버전 4.1 업데이트: 2024-06-05 @Munchkin
        /// - Logger 추가 (RESTLogger.cs)
        /// 

        #endregion

        public const int MIN_INTERNAL_OPERATION_MILLIS = 1000;

        /// <summary>
        /// Gets the version of the RESTApi library.
        /// </summary>
        public static Version Version => _version ??= new Version("4.1.0");
        private static Version _version;

        /// <summary>
        /// Sends a request and processes the response.
        /// </summary>
        /// <typeparam name="TReq">Request type.</typeparam>
        /// <typeparam name="TRes">Response type.</typeparam>
        /// <param name="request">Request object.</param>
        /// <param name="method">HTTP method to use for the request.</param>
        /// <param name="client">RESTClient instance.</param>
        /// <returns>Response result.</returns>
        internal static async UniTask<IResult> SendRequest<TReq, TRes>(TReq request, string method, RESTClient client)
            where TReq : RESTRequest
            where TRes : RESTObject, new()
        {
            // Step 1. Validating request ==========================================================================================================================
            if (request == null) throw new IssueException(Issue.InvalidRequest, "Request is null.");
            if (string.IsNullOrEmpty(request.Endpoint)) throw new IssueException(Issue.InvalidEndpoint, "Endpoint is null or empty.");

            if (client.LogRequestInfo) client.InternalLogger.RequestInfo($"Sending {method} request to {request.Endpoint}.");
            await NetworkUtils.CheckNetworkAsync(Config.NETWORK_CHECK_INTERVAL_IN_MILLIS, Config.NETWORK_CHECK_TIMEOUT_IN_MILLIS);
            using UnityWebRequest webReq = UnityWebRequestFactory.Create(request, method, client);

            if (webReq == null) throw new IssueException(Issue.InvalidRequest, "UnityWebRequest is null.");

            // Step 2. Sending request =============================================================================================================================
            await HandleUnityWebRequestResultAsync(webReq, request.RetryDelayInSec, request.MaxRetry, client);

            // Step 3. Detect content type from the response ========================================================================================================
            string contentTypeAsString = webReq.GetResponseHeader("Content-Type");
            if (string.IsNullOrEmpty(contentTypeAsString)) throw new IssueException(Issue.EmptyResponse, "Content-Type is null or empty.");

            // Step 4. Handling 'Stream' response ===================================================================================================================
            bool isStream = request.StreamMode is StreamMode.TextStream or StreamMode.BinaryStream;
            if (isStream)
            {
                // If it's a stream, everything is handled within the SendAndProcessRequest method
                if (client.LogStreamEvents) client.InternalLogger.RequestInfo("Stream has ended.");
                return RESTObject.Done(); // Let the caller know that the stream has ended
            }

            if (webReq.downloadHandler == null) throw new IssueException(Issue.EmptyResponse, "DownloadHandler is null.");
            if (string.IsNullOrEmpty(webReq.downloadHandler.text)) throw new IssueException(Issue.EmptyResponse, "DownloadHandler text is null or empty.");

            // Step 5. Handling response ============================================================================================================================
            if (client.LogRequestInfo) client.InternalLogger.RequestInfo($"Received response from {request.Endpoint}");

            DataTransferMode returnTransferMode = DataTransferMode.Text;
            if (request.DownloadPath != null) returnTransferMode = request.DownloadPath.Type.ToDataTransferMode();

            if (returnTransferMode == DataTransferMode.Text)
            {
                if (client.LogRequestDetails) client.InternalLogger.RequestDetails("Download Mode: Text");
                string textResult = webReq.downloadHandler.text;

                if (string.IsNullOrEmpty(textResult)) throw new IssueException(Issue.EmptyResponse, "Text result is null or empty.");
                if (client.LogResponseBody) client.InternalLogger.ResponseBody(textResult);

                return await TextResponseConverter.ConvertAsync<TRes>(textResult, request.DownloadPath, client);
            }

            if (returnTransferMode == DataTransferMode.Binary)
            {
                if (client.LogRequestDetails) client.InternalLogger.RequestDetails("Download Mode: Binary");
                byte[] binaryResult = webReq.downloadHandler.data;

                if (binaryResult.IsNullOrEmpty()) throw new IssueException(Issue.EmptyResponse, "Binary result is null or empty.");

                return await BinaryResponseConverter.ConvertAsync<TRes>(binaryResult, request.DownloadPath, client);
            }

            throw new IssueException(Issue.UnknownError);
        }

        public static async UniTask HandleUnityWebRequestResultAsync(UnityWebRequest request, float baseDelayInSec, int maxRetries, RESTClient client)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            float currentDelay = baseDelayInSec;
            if (baseDelayInSec < 2) currentDelay = 2; // Minimum delay of 2 seconds

            for (int attempt = 0; attempt < maxRetries; ++attempt)
            {
                if (request.isDone) break;

                await request.SendWebRequest().ToUniTask();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    return;
                }

                LogRequestError(request, client);
                if (attempt == maxRetries - 1) break;

                await UniTask.Delay(TimeSpan.FromSeconds(currentDelay));
                currentDelay *= 2; // Exponential backoff
            }

            throw new TimeoutException("UnityWebRequest did not complete successfully within the specified number of retries.");
        }

        private static void LogRequestError(UnityWebRequest request, RESTClient client)
        {
            switch (request.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                    client.Logger.Warning("Connection error detected.");
                    break;
                case UnityWebRequest.Result.DataProcessingError:
                    client.Logger.Warning("Data processing error detected.");
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    client.Logger.Warning("HTTP error detected: " + request.responseCode);
                    break;
                default:
                    client.Logger.Warning("Unknown error occurred.");
                    break;
            }
        }
    }
}