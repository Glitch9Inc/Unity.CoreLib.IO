using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using Glitch9.IO.Files;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Networking;

namespace Glitch9.IO.RESTApi
{
    public class RESTRequest
    {
        /// <summary>
        /// Creates an empty RESTRequest with the given endpoint.
        /// This is used when you are sending a request with empty body.
        /// </summary>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        public static RESTRequest Empty(string endpoint)
        {
            RESTRequest req = new() { Endpoint = endpoint };
            return req;
        }

        private const string DEFAULT_AUTH_HEADER_FIELD_NAME = "Authorization";
        private const int DEFAULT_RETRY_COUNT = 3;
        private const int DEFAULT_RETRY_DELAY_IN_SEC = 1;
        private const int DEFAULT_TIMEOUT_IN_SEC = 90;

        /// <summary>
        /// (Optional) The content type of the request. Default is Json
        /// </summary>
        [JsonIgnore] public ContentType ContentType { get; set; } = ContentType.Json;

        /// <summary>
        /// (Optional) The content type of the request. Default is Json
        /// </summary>
        [JsonIgnore] public ContentType ResponseContentType { get; set; } = ContentType.Json;

        /// <summary>
        /// (Required) Defines the target URL for the UnityWebRequest to communicate with
        /// </summary>
        [JsonIgnore] public string Endpoint { get; set; }

        /// <summary>
        /// If the request has a body or not
        /// </summary>
        [JsonIgnore] public bool HasBody { get; set; } = true;

        /// <summary>
        /// The number of retries of the request. Default is 3
        /// </summary>
        [JsonIgnore] public int MaxRetry { get; set; } = DEFAULT_RETRY_COUNT;

        /// <summary>
        /// Seconds of delay to make a retry. Default is 1
        /// </summary>
        [JsonIgnore] public float RetryDelayInSec { get; set; } = DEFAULT_RETRY_DELAY_IN_SEC;

        /// <summary>
        /// The timeout of the request in seconds. Default is 90
        /// </summary>
        [JsonIgnore] public int TimeoutInSec { get; set; } = DEFAULT_TIMEOUT_IN_SEC;

        /// <summary>
        /// The type of the response. Default is Text
        /// </summary>
        [JsonIgnore] public DownloadMode DownloadMode { get; set; } = DownloadMode.Text;

        /// <summary>
        /// The headers of the request
        /// </summary>
        [JsonIgnore] public List<RESTHeader> Headers { get; protected set; } = new();

        /// <summary>
        /// (Optional) The progress callback of the request
        /// </summary>
        [JsonIgnore] public Action<float> OnProgressChanged { get; set; }

        /// <summary>
        /// Holds a reference to a UnityWebRequest object, which manages the request to the remote server.
        /// </summary>
        [JsonIgnore] public UnityWebRequest WebRequest { get; set; }

        /// <summary>
        /// The form data of the request
        /// </summary>
        [JsonIgnore] public WWWForm Form { get; set; }

        /// <summary>
        /// The callback for the Text stream response
        /// </summary>
        [JsonIgnore] public Action<string> OnTextStreamReceived { get; set; }

        /// <summary>
        /// The callback for the binary stream response
        /// </summary>
        [JsonIgnore] public Action<byte[]> OnBinaryStreamReceived { get; set; }

        /// <summary>
        /// The directory path where the file will be downloaded.
        /// </summary>
        [JsonIgnore] public string FilePath { get; set; }

        /// <summary>
        /// This is only used locally.
        /// </summary>
        [JsonIgnore] public Guid Guid { get; } = Guid.NewGuid();


        protected RESTRequest() { }

        public void SetSecret(string secret)
        {
            AddHeader(DEFAULT_AUTH_HEADER_FIELD_NAME, $"Bearer {secret}");
        }

        public void AddAuthHeader(string headerValue, string headerName = DEFAULT_AUTH_HEADER_FIELD_NAME)
        {
            Headers.Add(new RESTHeader(headerName, headerValue));
        }

        public void AddHeader(string key, string value)
        {
            Headers.Add(new RESTHeader(key, value));
        }

        public void AddHeader(RESTHeader header)
        {
            Headers.Add(header);
        }

        public IEnumerable<RESTHeader> GetHeaders()
        {
            yield return ContentType.GetHeader();
            foreach (RESTHeader header in Headers) yield return header;
        }


        // equal check using Guid
        public static bool operator ==(RESTRequest left, RESTRequest right)
        {
            if (ReferenceEquals(left, null) && ReferenceEquals(right, null))
            {
                return true;
            }
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
            {
                return false;
            }
            return left.Guid == right.Guid;
        }

        public static bool operator !=(RESTRequest left, RESTRequest right)
        {
            return !(left == right);
        }

        protected bool Equals(RESTRequest other)
        {
            return Equals(Guid, other.Guid);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RESTRequest)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Guid);
        }

        
        public abstract class BaseApiReqBuilder<TBuilder, TReq>
            where TBuilder : BaseApiReqBuilder<TBuilder, TReq>
            where TReq : RESTRequest
        {
            // ReSharper disable once InconsistentNaming
            protected TReq _req;

            protected BaseApiReqBuilder()
            {
                _req = Activator.CreateInstance<TReq>();
            }

            protected BaseApiReqBuilder(string endpoint)
            {
                _req = Activator.CreateInstance<TReq>();
                _req.Endpoint = endpoint;
            }

            /// <summary>
            /// Set the number of retries of the request
            /// </summary>
            /// <param name="maxRetry"></param>
            /// <returns></returns>
            public TBuilder SetMaxRetry(int maxRetry)
            {
                _req.MaxRetry = maxRetry;
                return (TBuilder)this;
            }

            /// <summary>
            /// Set the delay of the retry in seconds
            /// </summary>
            /// <param name="retryDelayInSec"></param>
            /// <returns></returns>
            public TBuilder SetRetryDelay(float retryDelayInSec)
            {
                _req.RetryDelayInSec = retryDelayInSec;
                return (TBuilder)this;
            }

            /// <summary>
            /// Set the timeout of the request in seconds. Default is 90 seconds
            /// </summary>
            /// <param name="timeoutInSec"></param>
            /// <returns></returns>
            public TBuilder SetTimeout(int timeoutInSec)
            {
                _req.TimeoutInSec = timeoutInSec;
                return (TBuilder)this;
            }

            /// <summary>
            /// Set <see cref="RESTApi.DownloadMode"/> of the request. Default is <see cref="DownloadMode.Text"/>
            /// </summary>
            /// <param name="downloadMode"></param>
            /// <returns></returns>
            public TBuilder SetDownloadMode(DownloadMode downloadMode)
            {
                _req.DownloadMode = downloadMode;
                return (TBuilder)this;
            }


            /// <summary>
            /// Add an Authorization header to the request. Default header name is "Authorization".       
            /// </summary> 
            /// <remarks>
            /// (Optional) Very rarely, in some APIs, especially in old APIs, the HTTP header field name of Authorization may be different. (e.g. case-insensitive)
            /// </remarks>
            public TBuilder AddAuthHeader(string headerValue, string headerName = DEFAULT_AUTH_HEADER_FIELD_NAME)
            {
                return AddHeader(new RESTHeader(headerName, headerValue));
            }

            /// <summary>
            /// Add a header to the request
            /// </summary>
            /// <param name="headerValue"></param>
            /// <param name="headerName"></param>
            /// <returns></returns>
            public TBuilder AddHeader(string headerValue, string headerName)
            {
                return AddHeader(new RESTHeader(headerName, headerValue));
            }

            /// <summary>
            /// Add a header to the request
            /// </summary>
            /// <param name="headers"></param>
            /// <returns></returns>
            public TBuilder AddHeader(params RESTHeader[] headers)
            {
                if (headers == null) return (TBuilder)this;

                foreach (RESTHeader header in headers)
                {
                    if (!header.IsValid) continue;

                    // check if the header with the same name already exists
                    if (_req.Headers.Exists(h => h.Name == header.Name))
                    {
                        GNLog.Error($"[APIReq] {typeof(TReq).Name}: Header with the same name already exists: {header.Name}");
                        continue;
                    }

                    _req.Headers.Add(header);
                }
                return (TBuilder)this;
            }

            /// <summary>
            /// (Optional) The progress callback of the request
            /// </summary>
            /// <param name="progressCallback"></param>
            /// <returns></returns>
            public TBuilder SetProgressCallback(Action<float> progressCallback)
            {
                _req.OnProgressChanged = progressCallback;
                return (TBuilder)this;
            }

            /// <summary>
            /// Set the callback for Text stream
            /// </summary>
            /// <param name="onTextStreamReceived"></param>
            /// <returns></returns>
            public TBuilder SetTextStream(Action<string> onTextStreamReceived)
            {
                _req.DownloadMode = DownloadMode.TextStream;
                _req.OnTextStreamReceived = onTextStreamReceived;
                return (TBuilder)this;
            }

            /// <summary>
            /// Set the callback for binary stream
            /// </summary>
            /// <param name="onBinaryStreamReceived"></param>
            /// <returns></returns>
            public TBuilder SetBinaryStream(Action<byte[]> onBinaryStreamReceived)
            {
                _req.DownloadMode = DownloadMode.TextStream;
                _req.OnBinaryStreamReceived = onBinaryStreamReceived;
                return (TBuilder)this;
            }

            public TBuilder SetFilePath(string filePath)
            {
                _req.FilePath = filePath;
                return (TBuilder)this;
            }

            protected bool AllRequiredPropertiesSet()
            {
                PropertyInfo[] properties = typeof(TReq).GetProperties();
                List<string> missingFields = new();

                foreach (PropertyInfo property in properties)
                {
                    Attribute attribute = Attribute.GetCustomAttribute(property, typeof(RequiredAttribute));
                    if (attribute != null)
                    {
                        object value = property.GetValue(_req);
                        if (value == null || (value is string str && string.IsNullOrWhiteSpace(str)))
                        {
                            missingFields.Add(property.Name);
                        }
                    }
                }

                if (missingFields.Count <= 0) return true;
                GNLog.Error($"[APIReq] {typeof(TReq).Name}: Missing required properties: {string.Join(", ", missingFields)}");
                return false;
            }

            public TBuilder SetReturnContentType(ContentType contentType)
            {
                _req.ResponseContentType = contentType;
                return (TBuilder)this;
            }

            public virtual TReq Build()
            {
                return Build(ContentType.Json);
            }

            public virtual TReq Build(ContentType contentType)
            {
                if (AllRequiredPropertiesSet())
                {
                    _req.ContentType = contentType;
                    return _req;
                }
                GNLog.Error($"[APIReq] {typeof(TReq).Name}: Missing required properties");
                return null;
            }
        }
    }
}