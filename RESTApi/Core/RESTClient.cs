using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using UnityEngine.Networking;
// ReSharper disable InconsistentNaming

namespace Glitch9.IO.RESTApi
{
    public class RESTClient
    {
        private const string TAG = nameof(RESTClient);
        public JsonSerializerSettings JsonSettings;

        public RESTClient(JsonSerializerSettings jsonSettings = null)
        {
            JsonSettings = jsonSettings;

            if (JsonSettings == null)
            {
                JsonSettings = new() { NullValueHandling = NullValueHandling.Ignore };
                //GNLog.Super(TAG, "Using Default JsonSerializerSettings");
            }
            else
            {
                //GNLog.Super(TAG, "Using Custom JsonSerializerSettings");
            }
        }

        public virtual async UniTask<IRESTResult> POST<TReq>(TReq request)
            where TReq : RESTRequest
        {
            return await SendRequest<TReq, RESTResponse, RESTError>(request, UnityWebRequest.kHttpVerbPOST);
        }

        public virtual async UniTask<IRESTResult> POST<TReq, TRes>(TReq request)
            where TReq : RESTRequest
            where TRes : RESTResponse, new()
        {
            return await SendRequest<TReq, TRes, RESTError>(request, UnityWebRequest.kHttpVerbPOST);
        }

        public virtual async UniTask<IRESTResult> POST<TReq, TRes, TErr>(TReq request)
            where TReq : RESTRequest
            where TRes : RESTResponse, new()
            where TErr : RESTError, new()
        {
            return await SendRequest<TReq, TRes, TErr>(request, UnityWebRequest.kHttpVerbPOST);
        }

        public virtual async UniTask<IRESTResult> PUT<TReq>(TReq request)
            where TReq : RESTRequest
        {
            return await SendRequest<TReq, RESTResponse, RESTError>(request, UnityWebRequest.kHttpVerbPUT);
        }

        public virtual async UniTask<IRESTResult> PUT<TReq, TRes>(TReq request)
            where TReq : RESTRequest
            where TRes : RESTResponse, new()
        {
            return await SendRequest<TReq, TRes, RESTError>(request, UnityWebRequest.kHttpVerbPUT);
        }

        public virtual async UniTask<IRESTResult> PUT<TReq, TRes, TErr>(TReq request)
            where TReq : RESTRequest
            where TRes : RESTResponse, new()
            where TErr : RESTError, new()
        {
            return await SendRequest<TReq, TRes, TErr>(request, UnityWebRequest.kHttpVerbPUT);
        }

        public virtual async UniTask<IRESTResult> GET<TReq>(TReq request)
            where TReq : RESTRequest
        {
            return await SendRequest<TReq, RESTResponse, RESTError>(request, UnityWebRequest.kHttpVerbGET);
        }

        public virtual async UniTask<IRESTResult> GET<TReq, TRes>(TReq request)
            where TReq : RESTRequest
            where TRes : RESTResponse, new()
        {
            return await SendRequest<TReq, TRes, RESTError>(request, UnityWebRequest.kHttpVerbGET);
        }

        public virtual async UniTask<IRESTResult> GET<TReq, TRes, TErr>(TReq request)
            where TReq : RESTRequest
            where TRes : RESTResponse, new()
            where TErr : RESTError, new()
        {
            return await SendRequest<TReq, TRes, TErr>(request, UnityWebRequest.kHttpVerbGET);
        }

        public virtual async UniTask<IRESTResult> DELETE<TReq>(TReq request)
            where TReq : RESTRequest
        {
            return await SendRequest<TReq, RESTResponse, RESTError>(request, UnityWebRequest.kHttpVerbDELETE);
        }

        public virtual async UniTask<IRESTResult> DELETE<TReq, TRes>(TReq request)
            where TReq : RESTRequest
            where TRes : RESTResponse, new()
        {
            return await SendRequest<TReq, TRes, RESTError>(request, UnityWebRequest.kHttpVerbDELETE);
        }

        public virtual async UniTask<IRESTResult> DELETE<TReq, TRes, TErr>(TReq request)
            where TReq : RESTRequest
            where TRes : RESTResponse, new()
            where TErr : RESTError, new()
        {
            return await SendRequest<TReq, TRes, TErr>(request, UnityWebRequest.kHttpVerbDELETE);
        }

        public virtual async UniTask<IRESTResult> HEAD<TReq>(TReq request)
            where TReq : RESTRequest
        {
            return await SendRequest<TReq, RESTResponse, RESTError>(request, UnityWebRequest.kHttpVerbHEAD);
        }

        public virtual async UniTask<IRESTResult> HEAD<TReq, TRes>(TReq request)
            where TReq : RESTRequest
            where TRes : RESTResponse, new()
        {
            return await SendRequest<TReq, TRes, RESTError>(request, UnityWebRequest.kHttpVerbHEAD);
        }

        public virtual async UniTask<IRESTResult> HEAD<TReq, TRes, TErr>(TReq request)
            where TReq : RESTRequest
            where TRes : RESTResponse, new()
            where TErr : RESTError, new()
        {
            return await SendRequest<TReq, TRes, TErr>(request, UnityWebRequest.kHttpVerbHEAD);
        }

        public virtual async UniTask<IRESTResult> PATCH<TReq>(TReq request)
            where TReq : RESTRequest
        {
            return await SendRequest<TReq, RESTResponse, RESTError>(request, "PATCH");
        }

        public virtual async UniTask<IRESTResult> PATCH<TReq, TRes>(TReq request)
            where TReq : RESTRequest
            where TRes : RESTResponse, new()
        {
            return await SendRequest<TReq, TRes, RESTError>(request, "PATCH");
        }

        public virtual async UniTask<IRESTResult> PATCH<TReq, TRes, TErr>(TReq request)
            where TReq : RESTRequest where
            TRes : RESTResponse, new() where
            TErr : RESTError, new()
        {
            return await SendRequest<TReq, TRes, TErr>(request, "PATCH");
        }

        private async UniTask<IRESTResult> SendRequest<TReq, TRes, TErr>(TReq request, string method)
            where TReq : RESTRequest
            where TRes : RESTResponse, new()
            where TErr : RESTError, new()
        {
            try
            {
                if (request == null) throw new GNException(Issue.InvalidRequest, "Request is null.");
                if (string.IsNullOrEmpty(request.Endpoint)) throw new GNException(Issue.InvalidEndpoint, "Endpoint is null or empty.");

                await RESTApiV3.CheckNetworkAsync();
                using UnityWebRequest webReq = request.CreateUnityWebRequest(method, JsonSettings);

                if (webReq == null) throw new GNException(Issue.InvalidRequest, "UnityWebRequest is null.");

                TaskResult taskResult = await RESTApiV3.SendAndProcessRequest(webReq, request.RetryDelayInSec, request.RetryCount);
                if (!taskResult.IsSuccess)
                {
                    // try to parse the error
                    TErr errorObject = JsonConvert.DeserializeObject<TErr>(taskResult.ErrorText, JsonSettings);
                    if (errorObject == null) throw new GNException(Issue.ReceiveFailed, taskResult.ErrorText);
                    return errorObject;
                }

                bool isStream = request.DownloadMode is DownloadMode.TextStream or DownloadMode.BinaryStream;
                if (isStream)
                {
                    // If it's a stream, everything is handled within the SendAndProcessRequest method
                    if (request.IsLogEnabled) GNLog.Log("[RESTClient] Stream has ended.");
                    return new Done();
                }

                if (webReq.downloadHandler == null) throw new GNException(Issue.EmptyResponse, "DownloadHandler is null.");
                if (string.IsNullOrEmpty(webReq.downloadHandler.text)) throw new GNException(Issue.EmptyResponse, "DownloadHandler text is null or empty.");

                if (request.DownloadMode == DownloadMode.Text)
                {
                    byte[] binaryResult = webReq.downloadHandler.data;
                    string textResult = webReq.downloadHandler.text;

                    if (string.IsNullOrEmpty(textResult)) throw new GNException(Issue.EmptyResponse, "Text result is null or empty.");
                    if (request.IsLogEnabled) GNLog.Log($"[RESTClient] Response: {textResult}");

                    if (RESTApiV3.TryGetError(textResult, JsonSettings, out TErr error)) return error;
                    return await TextResponseHandler.HandleAsync<TRes>(binaryResult, textResult, request.FilePath, request.ContentType, JsonSettings);
                }
                else if (request.DownloadMode == DownloadMode.Binary)
                {
                    if (request.IsLogEnabled) GNLog.Log("[RESTClient] Response Mode: Binary");
                    byte[] binaryResult = webReq.downloadHandler.data;
                    string textResult = webReq.downloadHandler.text;

                    if (binaryResult == null || binaryResult.Length == 0)
                    {
                        GNLog.Error("[RESTClient] Binary result is null or empty");
                        if (string.IsNullOrEmpty(textResult)) throw new GNException(Issue.EmptyResponse, "Text result is also null or empty");
                        if (RESTApiV3.TryGetError(textResult, JsonSettings, out TErr error)) return error;
                    }

                    return await BinaryResponseHandler.HandleAsync<TRes>(binaryResult, textResult, request.FilePath, request.ResponseContentType);
                }
                else
                {
                    throw new GNException(Issue.UnknownError);
                }
            }
            catch (Exception e)
            {
                GNLog.Exception(e);
                return new TErr() { Issue = e.Convert(), Message = e.Message, StackTrace = e.StackTrace };
            }
        }
    }
}
