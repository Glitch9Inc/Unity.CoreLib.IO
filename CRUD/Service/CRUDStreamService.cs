using Newtonsoft.Json;
using System.Collections.Generic;

namespace Glitch9.IO.RESTApi
{
    public interface IChunk
    {
        string GetChunkText();
    }

    public abstract class CRUDStreamService<TChunk, TClient> : CRUDService<TClient>
        where TChunk : class, IChunk, new()
        where TClient : CRUDClient<TClient>
    {
        private const string FALLBACK_SSE_DONE = "[DONE]"; // OpenAI SSE Done string
        public StreamHandler CurrentStreamHandler { get; set; }
        public string StreamingText { get; set; }
        public TChunk LastChunk { get; set; }

        private readonly string _sseDone;

        protected CRUDStreamService(TClient client, params RESTHeader[] betaHeaders) : base(client, betaHeaders)
        {
            _sseDone = string.IsNullOrEmpty(client.SseDone) ? FALLBACK_SSE_DONE : client.SseDone;
        }

        protected CRUDStreamService(TClient client, bool isBeta) : base(client, isBeta)
        {
            _sseDone = string.IsNullOrEmpty(client.SseDone) ? FALLBACK_SSE_DONE : client.SseDone;
        }


        protected void DefaultStreamEvent(string sseString)
        {
            if (CurrentStreamHandler == null || string.IsNullOrEmpty(sseString)) return;
            List<(SSEField field, string result)> data = Client.SSEParser.Parse(sseString);
            if (data == null) return;

            foreach ((SSEField field, string result) kvp in data)
            {
                if (kvp.field == SSEField.Data)
                {
                    if (!string.IsNullOrEmpty(kvp.result))
                    {
                        if (kvp.result.Contains(_sseDone))
                        {
                            CurrentStreamHandler.Done(this);
                            break;  // End of stream
                        }

                        LastChunk = JsonConvert.DeserializeObject<TChunk>(kvp.result, Client.JsonSettings);
                        if (LastChunk == null)
                        {
                            Client.Logger.Warning($"Failed to deserialize SSE data: {kvp.result}");
                            continue;
                        }

                        string text = LastChunk.GetChunkText();
                        if (string.IsNullOrEmpty(text))
                        {
                            //Suppress this warning for now
                            //Client.Logger.Warning($"Failed to get text from chunk: {kvp.result}");
                            continue;
                        }

                        CurrentStreamHandler.Stream(this, text);
                        StreamingText += text;
                    }
                }
            }
        }
    }
}