using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Glitch9.IO.RESTApi
{
    /// <summary>
    /// REST API client for performing CRUD operations.
    /// </summary>
    public abstract partial class CRUDClient<TSelf> : RESTClient
        where TSelf : CRUDClient<TSelf>
    {
        /// <summary>
        /// The name of the API.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The version of the API.
        /// </summary>
        public string Version { get; }

        /// <summary>
        /// The beta version of the API if available.
        /// </summary>
        public string BetaVersion { get; }

        /// <summary>
        /// Delegate for handling exceptions that occur during an API request.
        /// </summary>
        /// <param name="exception">The exception that occurred.</param>
        public delegate void ExceptionHandler(string endpoint, Exception exception);

        /// <summary>
        /// Event invoked when an error occurs during an API request.
        /// </summary>
        public ExceptionHandler OnException { get; set; }

        /// <summary>
        /// Special logger for logging CRUD operations.
        /// </summary>
        public CRUDLogger CRUDLogger { get; }

        public string BaseUrl { get; }
        public string SseError { get; }
        public string SseDone { get; }

        // Fields
        private readonly ApiKeyGetter _apiKeyGetter;
        private readonly AutoParam _autoApiKey;
        private readonly AutoParam _autoVersionParam;
        private readonly AutoParam _autoBetaParam;
        private readonly RESTHeader? _betaHeader;
        private readonly RESTHeader[] _additionalHeaders;
        private readonly string _apiKeyQueryKey;

        // Constructors
        protected CRUDClient(CRUDClientSettings clientSettings, JsonSerializerSettings jsonSettings, ISSEParser sseParser, ILogger logger) : base(jsonSettings, sseParser, logger)
        {
            if (clientSettings == null) throw new ArgumentNullException(nameof(clientSettings));
            if (string.IsNullOrEmpty(clientSettings.Name)) throw new ArgumentException("API name must be set.", nameof(clientSettings));
            if (string.IsNullOrEmpty(clientSettings.BaseURL)) throw new ArgumentException("Base URL must be set.", nameof(clientSettings));

            if (clientSettings.AutoApiKey != AutoParam.Unset)
            {
                _apiKeyGetter = clientSettings.ApiKeyGetter ?? throw new ArgumentNullException(nameof(clientSettings.ApiKeyGetter));
                string apiKey = _apiKeyGetter?.Invoke();
                if (string.IsNullOrEmpty(apiKey)) throw new ArgumentException("API key is not provided. Make sure GetApiKey() is implemented correctly.");

                if (clientSettings.AutoApiKey == AutoParam.Query)
                {
                    if (string.IsNullOrEmpty(clientSettings.ApiKeyQueryKey)) throw new ArgumentException("API key param must be set.", nameof(clientSettings));
                    _apiKeyQueryKey = clientSettings.ApiKeyQueryKey;
                }
            }

            if (clientSettings.AutoBetaParam != AutoParam.Unset)
            {
                if (clientSettings.AutoBetaParam == AutoParam.Header)
                {
                    if (clientSettings.BetaHeader == null) throw new ArgumentException("Beta header must be set.", nameof(clientSettings));
                    _betaHeader = clientSettings.BetaHeader.Value;
                }
            }

            Name = clientSettings.Name;
            Version = clientSettings.Version;
            BetaVersion = clientSettings.BetaApiVersion;

            BaseUrl = clientSettings.BaseURL;
            _autoApiKey = clientSettings.AutoApiKey;
            _autoVersionParam = clientSettings.AutoVersionParam;
            _autoBetaParam = clientSettings.AutoBetaParam;
            _additionalHeaders = clientSettings.AdditionalHeaders;
            SseError = clientSettings.SSEError;
            SseDone = clientSettings.SSEDone;

            CRUDLogger = new CRUDLogger(clientSettings.Name, logger);
        }



        // Override Methods

        /// <summary>
        /// Deserializes the error object from the given JSON,
        /// and extracts the error message.
        /// </summary>
        /// <param name="errorJson"></param>
        /// <returns>The error message.</returns>
        protected abstract string DeserializeErrorObject(string errorJson);

        /// <summary>
        /// An event that is invoked when a CRUD operation is successful.
        /// Override this method to perform additional operations when a create operation is successful such as <c>logging</c>.
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="crudMethod"></param>
        /// <param name="req"></param>
        /// <param name="res"></param>
        protected abstract UniTask OnOperationComplete<TRequest, TResult>(CRUDMethod crudMethod, TRequest req, TResult res)
            where TRequest : RESTRequest
            where TResult : IResult;

        /// <summary>
        /// Override this method to handle the status of a <see cref="CRUDMethod.Delete"/> operation.
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="res"></param>
        /// <returns></returns>
        protected abstract bool HandleDeletionStatus<TResponse>(TResponse res) where TResponse : RESTResponse;


        // CRUD Operations
        private async UniTask<TResponse> Create<TRequest, TResponse>(TRequest req, string endpoint)
            where TRequest : RESTRequest
            where TResponse : RESTResponse, new()
        {
            ThrowIf.ArgumentIsNull(req, "Create Request");
            ThrowIf.EndpointIsNull(endpoint);
            req.Endpoint = endpoint;

            Type reqType = req.GetType();
            if (LogRequestInfo) CRUDLogger.Create(reqType);

            IResult result = await POST<TRequest, TResponse>(req);
            ThrowIf.ResultIsNull(result);

            await OnOperationComplete(CRUDMethod.Create, req, result);
            TResponse res = ProcessResponse<TResponse>(CRUDMethod.Create, result);
            return res;
        }

        private async UniTask<bool> CreateAndGetEmptyResponse<TRequest>(TRequest req, string endpoint)
            where TRequest : RESTRequest
        {
            ThrowIf.ArgumentIsNull(req, "CreateAndGetEmptyResponse Request");
            ThrowIf.EndpointIsNull(endpoint);
            req.Endpoint = endpoint;

            Type reqType = req.GetType();
            if (LogRequestInfo) CRUDLogger.Create(reqType);

            IResult result = await POST(req);
            ThrowIf.ResultIsNull(result);

            await OnOperationComplete(CRUDMethod.Create, req, result);

            if (result is RESTResponse res)
            {
                return res.HasEmptyBody;
            }
            return false;
        }

        // Modify an object using GenerativeAI API with dynamic endpoint resolution.
        private async UniTask<TResponse> Update<TRequest, TResponse>(TRequest req, string endpoint)
            where TRequest : RESTRequest
            where TResponse : RESTResponse, new()
        {
            ThrowIf.ArgumentIsNull(req, "Update Request");
            ThrowIf.EndpointIsNull(endpoint);
            req.Endpoint = endpoint;

            if (LogRequestInfo) CRUDLogger.Update(req.GetType());
            IResult result = await POST<TRequest, TResponse>(req);
            ThrowIf.ResultIsNull(result);

            await OnOperationComplete(CRUDMethod.Update, req, result);
            return ProcessResponse<TResponse>(CRUDMethod.Update, result);
        }

        private async UniTask<TResponse> Get<TResponse>(RESTRequest req, string endpoint)
            where TResponse : RESTResponse, new()
        {
            ThrowIf.ArgumentIsNull(req, "Get Request");
            ThrowIf.EndpointIsNull(endpoint);
            req.Endpoint = endpoint;

            if (LogRequestInfo) CRUDLogger.Get(typeof(TResponse));
            IResult result = await GET<TResponse>(req);
            ThrowIf.ResultIsNull(result);

            await OnOperationComplete(CRUDMethod.Get, req, result);
            return ProcessResponse<TResponse>(CRUDMethod.Get, result);
        }

        private async UniTask<TResponse> Patch<TResponse>(RESTRequest req, string endpoint)
            where TResponse : RESTResponse, new()
        {
            ThrowIf.ArgumentIsNull(req, "Patch Request");
            ThrowIf.EndpointIsNull(endpoint);
            req.Endpoint = endpoint;

            if (LogRequestInfo) CRUDLogger.Patch(typeof(TResponse));
            IResult result = await PATCH(req);
            ThrowIf.ResultIsNull(result);

            await OnOperationComplete(CRUDMethod.Patch, req, result);
            return ProcessResponse<TResponse>(CRUDMethod.Patch, result);
        }

        // Cancel an object using GenerativeAI API with dynamic endpoint resolution.
        private async UniTask<TResponse> Cancel<TResponse>(RESTRequest req, string endpoint)
            where TResponse : RESTResponse, new()
        {
            ThrowIf.ArgumentIsNull(req, "Cancel Request");
            ThrowIf.EndpointIsNull(endpoint);
            req.Endpoint = endpoint;

            if (LogRequestInfo) CRUDLogger.Cancel(typeof(TResponse));
            IResult result = await POST(req);
            ThrowIf.ResultIsNull(result);

            await OnOperationComplete(CRUDMethod.Cancel, req, result);
            return ProcessResponse<TResponse>(CRUDMethod.Cancel, result);
        }

        // Delete an object using GenerativeAI API with dynamic endpoint resolution.
        // If successful, the response body is empty.
        private async UniTask<bool> Delete<TResponse>(RESTRequest req, string endpoint)
            where TResponse : RESTResponse, new()
        {
            ThrowIf.ArgumentIsNull(req, "Delete Request");
            ThrowIf.EndpointIsNull(endpoint);
            req.Endpoint = endpoint;

            if (LogRequestInfo) CRUDLogger.Delete(typeof(TResponse));
            IResult result = await DELETE(req);
            ThrowIf.ResultIsNull(result);

            await OnOperationComplete(CRUDMethod.Delete, req, result);
            if (result is TResponse res) return HandleDeletionStatus(res);
            return result?.IsSuccess ?? false;
        }


        // Retrieve a list of objects using a query with optional parameters.
        private async UniTask<TQueryResponse> List<TRequest, TQueryResponse, T>(TRequest req, string endpoint)
            where TRequest : RESTRequest
            where TQueryResponse : RESTQueryResponse<T>, new()
            where T : class, new()
        {
            ThrowIf.ArgumentIsNull(req, "List Request");
            ThrowIf.EndpointIsNull(endpoint);
            req.Endpoint = endpoint;

            if (LogRequestInfo) CRUDLogger.List(typeof(T));
            IResult result = await GET<TRequest, TQueryResponse>(req);
            ThrowIf.ResultIsNull(result);
            await OnOperationComplete(CRUDMethod.List, req, result);

            return ProcessResponse<TQueryResponse>(CRUDMethod.List, result);
        }

        private async UniTask<TResponse> Query<TRequest, TResponse>(TRequest req, string endpoint)
            where TRequest : RESTRequest
            where TResponse : RESTResponse, new()
        {
            ThrowIf.ArgumentIsNull(req, "Query Request");
            ThrowIf.EndpointIsNull(endpoint);
            req.Endpoint = endpoint;

            if (LogRequestInfo) CRUDLogger.Query(req.GetType());
            IResult result = await POST<TRequest, TResponse>(req);
            ThrowIf.ResultIsNull(result);

            await OnOperationComplete(CRUDMethod.Query, req, result);
            return ProcessResponse<TResponse>(CRUDMethod.Query, result);
        }



        // Handle results from API calls, converting them to the appropriate type.
        private TResponse ProcessResponse<TResponse>(CRUDMethod method, IResult result)
            where TResponse : RESTResponse, new()
        {
            if (result is TResponse model) return model; // Return the model if result can be directly converted.
            if (result is Result<TResponse> res) return res.Value; // Return the data if result is a generic result.
            if (result is Error err)
            {
                CRUDLogger.Error(err.ToString());
                return null;
            }
            if (result.IsSuccess || result.IsDone) return null; // Return null if result indicates success but no data.

            string requestedResponseName = typeof(TResponse).Name;
            string returnedResponseName = result.GetType().Name;
            string preMessage = $"'{LastRequest}({method})' requested a {requestedResponseName} and received a {returnedResponseName}: ";

            string failReason = result.FailReason;
            if (string.IsNullOrEmpty(failReason)) failReason = "No fail reason provided.";

            string failMessage = $"{preMessage}Operation failed: {failReason}";
            CRUDLogger.Error(failMessage);

            return null;
        }


        // Handle exceptions from GenerativeAI API calls, potentially parsing error messages.
        public void HandleException(Exception exception)
        {
            string exceptionMessage = exception.Message;

            if (!string.IsNullOrEmpty(SseError) && exceptionMessage.Contains(SseError))
            {
                //CRUDLogger.Warning("Error message received from OpenAI API.");

                // Extract the JSON part
                int jsonStartIndex = exceptionMessage.IndexOf("{", StringComparison.Ordinal);
                string jsonPart = exceptionMessage.Substring(jsonStartIndex);

                // Parse the JSON part using JObject
                JObject json;
                try
                {
                    json = JObject.Parse(jsonPart);
                }
                catch
                {
                    CRUDLogger.Error(exceptionMessage);
                    OnException?.Invoke(LastEndpoint, exception);
                    return;
                }

                JToken errorJson = json[SseError];
                if (errorJson == null)
                {
                    CRUDLogger.Error("Failed to parse OpenAI error message: " + jsonPart);
                    OnException?.Invoke(LastEndpoint, exception);
                    return;
                }

                try
                {
                    string errorMessage = DeserializeErrorObject(errorJson.ToString());
                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        CRUDLogger.Error(errorMessage);
                        exception = new Exception(errorMessage);
                    }
                }
                catch (JsonException ex)
                {
                    CRUDLogger.Error("Failed to deserialize OpenAI error message: " + ex.Message);
                    OnException?.Invoke(LastEndpoint, exception);
                    return;
                }
            }
            else
            {
                CRUDLogger.Error(exceptionMessage);
            }

            OnException?.Invoke(LastEndpoint, exception);
        }
    }
}