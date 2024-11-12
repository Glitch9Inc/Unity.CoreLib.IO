using System;
using System.Text;
using UnityEngine.Networking;

namespace Glitch9.IO.RESTApi
{
    public class TextStreamHandlerBuffer : DownloadHandlerScript
    {
        /// <summary>
        /// The event to subscribe to for receiving data chunks
        /// </summary>
        private readonly Action<string> _onDataChunkReceived;
        private readonly Action<float> _onProgressChanged;
        private readonly RESTClient _client;

        /// <summary>
        /// Temporary buffer for storing incomplete characters between data chunks
        /// </summary>
        private byte[] _leftoverBytes;

        public TextStreamHandlerBuffer(RESTClient client, Action<string> onDataChunkReceived, Action<float> onProgressChanged = null) : base()
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

            byte[] fullDataChunk = streamedData;
            if (_leftoverBytes != null && _leftoverBytes.Length > 0)
            {
                // Combine leftover bytes from the previous chunk with the current chunk
                fullDataChunk = new byte[_leftoverBytes.Length + dataLength];
                Buffer.BlockCopy(_leftoverBytes, 0, fullDataChunk, 0, _leftoverBytes.Length);
                Buffer.BlockCopy(streamedData, 0, fullDataChunk, _leftoverBytes.Length, dataLength);
                _leftoverBytes = null;
            }

            // Try to convert the bytes to string
            string encodedText = Encoding.UTF8.GetString(fullDataChunk);
            if (_client.LogStreamEvents) _client.InternalLogger.StreamEvent($"Received data: {encodedText}");

            // Here, you should invoke the onDataChunkReceived event or callback with the Text
            _onDataChunkReceived?.Invoke(encodedText);

            // Check if the last byte potentially begins a multibyte character
            int lastFullCharIndex = GetLastFullCharIndex(fullDataChunk);
            if (lastFullCharIndex < fullDataChunk.Length - 1)
            {
                // Store incomplete character bytes to be processed with the next chunk
                int leftoverLength = fullDataChunk.Length - lastFullCharIndex - 1;
                _leftoverBytes = new byte[leftoverLength];
                Buffer.BlockCopy(fullDataChunk, lastFullCharIndex + 1, _leftoverBytes, 0, leftoverLength);
            }

            return true;
        }

        /// <summary>
        /// Called when all data has been received
        /// </summary>
        protected override void CompleteContent()
        {
            if (_client.LogStreamEvents) _client.InternalLogger.StreamEvent("<color=blue>Stream complete!</color>");
        }

        /// <summary>
        /// Utility method to find the index of the last full character in a byte array
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private int GetLastFullCharIndex(byte[] bytes)
        {
            int lastIndex = bytes.Length - 1;

            // Work backwards from the end of the byte array
            for (int i = bytes.Length - 1; i >= 0; i--)
            {
                // Lead byte for UTF-8 multibyte character sequence will have two high bits set
                if ((bytes[i] & 0xC0) != 0x80)
                {
                    lastIndex = i;
                    break;
                }
            }

            return lastIndex;
        }
    }
}