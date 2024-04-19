using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Glitch9.IO.RESTApi
{
    public class BinaryStreamHandlerBuffer : DownloadHandlerScript
    {
        /// <summary>
        ///     The event to subscribe to for receiving data chunks
        /// </summary>
        private readonly Action<byte[]> _onDataChunkReceived;
        private readonly Action<float> _onProgressChanged;
        private readonly bool _enableLog;

        public BinaryStreamHandlerBuffer(Action<byte[]> onDataChunkReceived, Action<float> onProgressChanged = null, bool enableLog = false) : base()
        {
            _onDataChunkReceived = onDataChunkReceived;
            _onProgressChanged = onProgressChanged;
            _enableLog = enableLog;
        }

        /// <summary>
        ///     Implement if needed to report progress
        /// </summary>
        /// <returns></returns>
        protected override float GetProgress()
        {
            float progress = base.GetProgress();
            _onProgressChanged?.Invoke(progress);
            return progress;
        }

        /// <summary>
        ///     This method is called whenever data is received
        /// </summary>
        /// <param name="data"></param>
        /// <param name="dataLength"></param>
        /// <returns></returns>
        protected override bool ReceiveData(byte[] data, int dataLength)
        {
            if (data == null || dataLength == 0) return false;
            _onDataChunkReceived(data); // 그대로 전달
            return true;
        }

        /// <summary>
        ///     Called when all data has been received
        /// </summary>
        protected override void CompleteContent()
        {
            if (_enableLog) Debug.Log("<color=blue>Stream complete!</color>");
        }
    }
}