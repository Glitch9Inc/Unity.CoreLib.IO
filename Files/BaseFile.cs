using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Glitch9.IO.Files
{
    [Serializable]
    public abstract class BaseFile<T>
    {
        // Event handlers
        public event Action OnLoading;
        public event Action<T> OnLoaded;
        public event Action<string> OnError;
        
        [SerializeField] private FilePath path;
        [SerializeField] private string text;
        
        private T _value;
        private OneShotLog _oneShotLog;
        
        public FilePath Path
        {
            get => path;
            set => path = value; // Consider thread safety if needed.
        }

        public string Text
        {
            get => text;
            set => text = value; // Consider thread safety if needed.
        }

        public T Value
        {
            get => _value;
            set => _value = value; // Consider thread safety if needed.
        }

        public ContentType Type => Path?.Type ?? ContentType.OctetStream;

        public string FileName => Path?.FileName;
        
        public bool IsLoading { get; private set; } = false;
        
        public bool IsLoaded { get; private set; } = false;
        
        public bool IsError { get; private set; } = false;

        public string LastError => _oneShotLog?.ToString();

        public async UniTask<T> GetValue(bool forceRefresh = false)
        {
            if (IsLoaded && !forceRefresh)
                return _value;

            if (IsLoading)
                return _value;

            if (IsError)
            {
                _oneShotLog ??= new();
                _oneShotLog.Error($"Error loading file. Path:{Path?.Path}");
                OnError?.Invoke(LastError);
                return _value;
            }

            return await GetAsync();
        }

        private async UniTask<T> GetAsync()
        {
            IsLoading = true;
            OnLoading?.Invoke();

            IsError = false;
            IsLoaded = false;

            try
            {
                _value = await LoadFileAsync();
                IsLoaded = true;
                OnLoaded?.Invoke(_value);
            }
            catch (Exception e)
            {
                IsError = true;
                _oneShotLog ??= new();
                _oneShotLog.Error($"Failed to load file from {Path?.Path}. Error: {e}");
                OnError?.Invoke(LastError);
            }
            finally
            {
                IsLoading = false;
            }

            return _value;
        }

        protected abstract UniTask<T> LoadFileAsync();
        protected abstract byte[] ToBytes();
    }
}