using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using System;
using System.Net.Http;
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
        public static Version Version => _version ??= new Version("3.3.2");

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

                GNLog.Log("Network connection re-established.");
            }
        }

        public static async UniTask<TaskResult> SendAndProcessRequest(UnityWebRequest request, float delayInSec, int retryCount)
        {
            try
            {
                // Send the request for the first time
                return await HandleUnityWebRequestResultAsync(request, delayInSec, retryCount);
            }
            catch (Exception ex) // Catching more general exception for broader coverage
            {
                GNLog.Error($"An error occurred during request: {ex.Message}");
                return TaskResult.Error(ex); // Assuming TaskResult.Error can handle general exceptions
            }
        }

        public static async UniTask<TaskResult> HandleUnityWebRequestResultAsync(UnityWebRequest request, float baseDelayInSec, int retryCount)
        {
            TaskResult result = TaskResult.Success; // Assuming a default successful state
            if (baseDelayInSec < 2) baseDelayInSec = 2; // Minimum delay of 2 seconds

            for (int attempt = 0; attempt < retryCount; ++attempt)
            {
                try
                {
                    await request.SendWebRequest().ToUniTask();

                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        return TaskResult.Success;
                    }

                    if (HasErrorObject(request.downloadHandler.text, out result))
                    {
                        return result;
                    }
                    else if (request.result == UnityWebRequest.Result.ProtocolError && request.responseCode == 429)
                    {
                        Debug.LogWarning("Too many requests. Retrying after delay.");
                    }
                    else if (request.result == UnityWebRequest.Result.ConnectionError)
                    {
                        Debug.LogWarning("Connection error detected. Retrying after delay.");
                    }
                    else if (request.result == UnityWebRequest.Result.DataProcessingError)
                    {
                        Debug.LogWarning("Data processing error detected. Retrying after delay.");
                    }
                    else
                    {
                        // Not retrying here.
                        Debug.LogError($"Request failed: {request.error}");
                        return TaskResult.Error(new HttpRequestException(request.error));
                    }
                }
                catch (Exception ex)
                {
                    if (HasErrorObject(request.downloadHandler.text, out result))
                    {
                        return result;
                    }

                    if (attempt == retryCount - 1)
                    {
                        return TaskResult.Error(ex);
                    }
                }

                if (attempt < retryCount - 1)
                {
                    float delay = baseDelayInSec * (int)Math.Pow(2, attempt); // 지수 백오프 계산
                    Debug.Log($"Waiting {delay} seconds before retrying...");
                    await UniTask.Delay(TimeSpan.FromSeconds(delay));
                }
            }

            return result;
        }

        private static bool HasErrorObject(string handlerText, out TaskResult taskResult)
        {
            taskResult = null;
            if (string.IsNullOrWhiteSpace(handlerText) || !handlerText.Contains("error"))
            {
                return false;
            }
            taskResult = TaskResult.Error(Issue.ErrorObjectReturned, handlerText);
            Debug.LogError(handlerText);
            return true;
        }

        public static bool TryGetError<TErr>(string resultAsString, JsonSerializerSettings jsonSettings, [CanBeNull] out TErr error)
            where TErr : RESTError, new()
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