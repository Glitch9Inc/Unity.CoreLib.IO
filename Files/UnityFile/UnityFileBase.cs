using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using UnityEngine;

namespace Glitch9.IO.Files
{
    [Serializable]
    public abstract class UnityFileBase<T>
    {
        private const int FILE_CHECK_INITIAL_DELAY_IN_MILLIS = 1200;
        private const int FILE_CHECK_RETRY_INTERVAL_IN_MILLIS = 1000;
        private const int DELAY_AFTER_LOADING_IN_MILLIS = 500;
        private const int FILE_CHECK_COUNT = 3;

        // Event handlers
        public event Action onLoading;
        public event Action<T> onLoaded;
        //public event Action<string> onError;

        [SerializeField] private UnityFilePath path;
        [SerializeField] private string text;

        [JsonIgnore] private T _value;

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

        [JsonIgnore] public string LastError { get; private set; }



        protected UnityFileBase() { }
        protected UnityFileBase(UnityFilePath path) => Path = path;

        protected UnityFileBase(string filePath)
        {
            if (filePath.StartsWith("Assets/"))
                Path = new UnityFilePath(PathType.DataPath, filePath);
            else if (filePath.StartsWith("Resources/"))
                Path = new UnityFilePath(PathType.Resources, filePath);
            else if (filePath.StartsWith("http://") || filePath.StartsWith("https://"))
                Path = new UnityFilePath(PathType.WebUrl, filePath);
            else
                Path = new UnityFilePath(PathType.Local, filePath);
        }

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
                LastError = $"Error loading file. Path:{Path?.Path}";
                Debug.LogError(LastError);
                return _value;
            }

            return await GetAsync(customInitialDelay);
        }

        private async UniTask<T> GetAsync(float customInitialDelay)
        {
            IsLoading = true;
            onLoading?.Invoke();

            IsError = false;
            IsLoaded = false;

            if (Path == null)
            {
                LastError = "UnityFile was not loaded because the UnityFilePath was null.";
                Debug.LogError(LastError);
            }
            else if (!Path.IsValid)
            {
                LastError = "UnityFile was not loaded because the UnityFilePath was invalid (null or whitespace).";
                //Debug.LogError(LastError);
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
                    onLoaded?.Invoke(_value);
                }
                catch (Exception e)
                {
                    LastError = e.Message;
                    Debug.LogError(e);
                }
            }

            StopLoading().Forget();
            return _value;
        }

        private async UniTask StopLoading()
        {
            await UniTask.Delay(DELAY_AFTER_LOADING_IN_MILLIS);
            IsLoading = false;
        }

        protected abstract UniTask<T> LoadFileAsync();
        public abstract byte[] ToByteArray();
    }
}