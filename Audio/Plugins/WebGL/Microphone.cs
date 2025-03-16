
#if UNITY_WEBGL && !UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Glitch9.CoreLib.IO.Audio
{
    public class Microphone
    {
        private static WebGLMicrophone _webglMicrophone;

        static Microphone()
        {
            Init();
            QueryAudioInput();

            GameObject webglMicrophoneGO = new GameObject("WebGLMicrophone");
            _webglMicrophone = webglMicrophoneGO.AddComponent<WebGLMicrophone>();
        }

        public static Action<AudioClip> onAudioClipReceived
        {
            get => _webglMicrophone.onAudioClipReceived;
            set => _webglMicrophone.onAudioClipReceived = value;
        }

        [DllImport("__Internal")]
        public static extern void Init();

        [DllImport("__Internal")]
        public static extern void QueryAudioInput();

        [DllImport("__Internal")]
        private static extern int GetNumberOfMicrophones();

        [DllImport("__Internal")]
        private static extern string GetMicrophoneDeviceName(int index);

        [DllImport("__Internal")]
        private static extern float GetMicrophoneVolume(int index);

        [DllImport("__Internal")]
        public static extern void Start(string deviceName, bool loop, int duration, int sampleRate);

        /// deviceName is always null in WebGL
        [DllImport("__Internal")]
        public static extern int GetPosition(string deviceName);


        private static List<Action> _sActions = new List<Action>();

        public static void Update()
        {
            for (int i = 0; i < _sActions.Count; ++i)
            {
                Action action = _sActions[i];
                action.Invoke();
            }
        }

        public static string[] devices
        {
            get
            {
                List<string> list = new List<string>();
                int size = GetNumberOfMicrophones();
                for (int index = 0; index < size; ++index)
                {
                    string deviceName = GetMicrophoneDeviceName(index);
                    list.Add(deviceName);
                }
                return list.ToArray();
            }
        }

        public static float[] volumes
        {
            get
            {
                List<float> list = new List<float>();
                int size = GetNumberOfMicrophones();
                for (int index = 0; index < size; ++index)
                {
                    float volume = GetMicrophoneVolume(index);
                    list.Add(volume);
                }
                return list.ToArray();
            }
        }

        public static bool IsRecording(string deviceName)
        {
            return false;
        }

        public static void GetDeviceCaps(string deviceName, out int minFreq, out int maxFreq)
        {
            minFreq = 0;
            maxFreq = 0;
        }

        public static void End(string deviceName)
        {
        }
    }
}

#endif