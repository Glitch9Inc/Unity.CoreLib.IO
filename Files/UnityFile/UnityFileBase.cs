using System;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace Glitch9.IO.Files
{
    [Serializable]
    public abstract class UnityFileBase<T>
    {
        private const int FILE_CHECK_INITIAL_DELAY_IN_MILLIS = 3000;
        private const int FILE_CHECK_RETRY_INTERVAL_IN_MILLIS = 1000;
        private const int FILE_CHECK_COUNT = 3;

        // Event handlers
        public event Action OnLoading;
        public event Action<T> OnLoaded;
        public event Action<string> OnError;
        
        [SerializeField] private UnityFilePath path;
        [SerializeField] private string text;
        
        [JsonIgnore] private T _value;
        [JsonIgnore] private OneShotLog _oneShotLog;
        
        [JsonIgnore]  
        public UnityFilePath Path
        {
            get => path ?? new();
            set => path = value; // Consider thread safety if needed.
        }

        [JsonIgnore]
        public string Text
        {
            get => text;
            set => text = value; // Consider thread safety if needed.
        }

        [JsonIgnore]
        public T Value
        {
            get => _value;
            set => _value = value; // Consider thread safety if needed.
        }

        [JsonIgnore]
        public ContentType Type
        {
            get => Path?.Type ?? ContentType.OctetStream;
            set => Path.Type = value; // Consider thread safety if needed.
        }

        [JsonIgnore] public string FileName => Path?.FileName;

        [JsonIgnore] public bool IsLoading { get; private set; } = false;

        [JsonIgnore] public bool IsLoaded { get; private set; } = false;

        [JsonIgnore] public bool IsError { get; private set; } = false;

        [JsonIgnore] public string LastError => _oneShotLog?.ToString();

        
        public async UniTask<T> GetValue(bool forceRefresh = false)
        {
            return await GetValue(-1, forceRefresh);
        }
        
        public async UniTask<T> GetValue(float customInitialDelay, bool forceRefresh = false)
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

            return await GetAsync(customInitialDelay);
        }

        private async UniTask<T> GetAsync(float customInitialDelay)
        {
            IsLoading = true;
            OnLoading?.Invoke();

            IsError = false;
            IsLoaded = false;

            if (Path == null)
            {
                HandleError("UnityFile was not loaded because the UnityFilePath was null.");
            }
            else
            {
                if (customInitialDelay > 0)
                    await UniTask.Delay((int)customInitialDelay * 1000);
                else
                    await UniTask.Delay(FILE_CHECK_INITIAL_DELAY_IN_MILLIS);

                for (int i = 0; i < FILE_CHECK_COUNT; i++)
                {
                    if (Path.FileExists()) break;
                    await UniTask.Delay(FILE_CHECK_RETRY_INTERVAL_IN_MILLIS);
                }

                try
                {
                    _value = await LoadFileAsync();
                    IsLoaded = true;
                    OnLoaded?.Invoke(_value);
                }
                catch (Exception e)
                {
                    HandleError(e.Message);
                }
            }

            IsLoading = false;
            return _value;
        }

        private void HandleError(string message)
        {
            IsError = true;
            _oneShotLog ??= new();
            _oneShotLog.Error($"Failed to load file from {Path?.Path}. Error: {message}");
            OnError?.Invoke(LastError);
        }
      
        protected abstract UniTask<T> LoadFileAsync();
        protected abstract byte[] ToBytes();
    }
}