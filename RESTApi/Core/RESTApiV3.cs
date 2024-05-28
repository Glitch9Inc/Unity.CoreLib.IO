using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using System;
using UnityEngine;
using UnityEngine.Networking;


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

        #endregion

        private const int NETWORK_CHECK_INTERVAL_IN_MILLIS = 500;
        private static Version _version;

        /// <summary>
        /// Gets the version of the RESTApi library.
        /// </summary>
        public static Version Version => _version ??= new Version("3.3.6");

        public static async UniTask CheckNetworkAsync()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                GNLog.Warning("Internet is not reachable. Waiting for network connection...");

                await UniTask.RunOnThreadPool(() =>
                {
                    while (Application.internetReachability == NetworkReachability.NotReachable)
                    {
                        UniTask.Delay(NETWORK_CHECK_INTERVAL_IN_MILLIS); // Wait for half a second Before checking again
                    }
                });

                GNLog.Info("Network connection re-established.");
            }
        }

        public static async UniTask<Result> SendAndProcessRequest<TErr>(UnityWebRequest request, float delayInSec, int retryCount)
        {
            try
            {
                // Send the request for the first time
                return await HandleUnityWebRequestResultAsync(request, delayInSec, retryCount);
            }
            catch (Exception ex) // Catching more general exception for broader coverage
            {
                string msg = ex.Message;
                if (!string.IsNullOrEmpty(msg))
                {
                    TErr errorObject = JsonConvert.DeserializeObject<TErr>(msg);
                    if (errorObject != null)
                    {
                        //TODO: Do something with the error object
                    }
                }
                
                GNLog.Error($"An error occurred during request: {ex.Message}");
                return new Error(ex); // Assuming TaskResult.CreateError can handle general exceptions
            }
        }

        public static async UniTask<Result> HandleUnityWebRequestResultAsync(UnityWebRequest request, float baseDelayInSec, int maxRetries)
        {
            if (request == null) return new Error("UnityWebRequest is null");

            float currentDelay = baseDelayInSec;
            if (baseDelayInSec < 2) currentDelay = 2; // Minimum delay of 2 seconds

            for (int attempt = 0; attempt < maxRetries; ++attempt)
            {
                if (request.isDone) break;
                
                try
                {
                    await request.SendWebRequest().ToUniTask();

                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        return Result.Success();
                    }
   
                    LogRequestError(request);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Request failed: {ex.Message}");
                    if (attempt == maxRetries - 1) throw; // Last attempt, rethrow exception
                }

                await UniTask.Delay(TimeSpan.FromSeconds(currentDelay));
                currentDelay *= 2; // Exponential backoff
            }

            return new Error(Issue.RequestFailed);
        }

        private static void LogRequestError(UnityWebRequest request)
        {
            switch (request.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                    Debug.LogWarning("Connection error detected.");
                    break;
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogWarning("Data processing error detected.");
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogWarning("HTTP error detected: " + request.responseCode);
                    break;
                default:
                    Debug.LogWarning("Unknown error occurred.");
                    break;
            }
        }

        public static bool TryGetError<TErr>(string resultAsString, JsonSerializerSettings jsonSettings, [CanBeNull] out TErr error)
            where TErr : Error, new()
        {
            if (!resultAsString.Search("error :"))
            {
                error = null;
                return false;
            }

            error = JsonConvert.DeserializeObject<TErr>(resultAsString, jsonSettings);
            return error != null;
        }
    }
}