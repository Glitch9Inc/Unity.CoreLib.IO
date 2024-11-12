using System;
using UnityEngine.Networking;

namespace Glitch9.IO.RESTApi
{
    public class BinaryStreamHandlerBuffer : DownloadHandlerScript
    {
        /// <summary>
        /// The event to subscribe to for receiving data chunks
        /// </summary>
        private readonly Action<byte[]> _onDataChunkReceived;
        private readonly Action<float> _onProgressChanged;
        private readonly RESTClient _client;

        public BinaryStreamHandlerBuffer(RESTClient client, Action<byte[]> onDataChunkReceived, Action<float> onProgressChanged = null) : base()
        {
            _onDataChunkReceived = onDataChunkReceived;
            _onProgressChanged = onProgressChanged;
            _client = client;
        }

        /// <summary>
        /// Implement if needed to report progress
        /// </summary>
        /// <returns></returns>
        protected override float GetProgress()
        {
            float progress = base.GetProgress();
            _onProgressChanged?.Invoke(progress);
            return progress;
        }

        /// <summary>
        /// This method is called whenever data is received
        /// </summary>
        /// <param name="streamedData"></param>
        /// <param name="dataLength"></param>
        /// <returns></returns>
        protected override bool ReceiveData(byte[] streamedData, int dataLength)
        {
            if (streamedData == null || dataLength == 0) return false;
            _onDataChunkReceived(streamedData); // 그대로 전달
            return true;
        }

        /// <summary>
        /// Called when all data has been received
        /// </summary>
        protected override void CompleteContent()
        {
            if (_client.LogStreamEvents) _client.InternalLogger.StreamEvent("<color=blue>Stream complete!</color>");
        }
    }
}