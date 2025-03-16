
#if UNITY_WEBGL && !UNITY_EDITOR
using System;
using UnityEngine;

namespace Glitch9.CoreLib.IO.Audio
{
    class WebGLMicrophone : MonoBehaviour
    {
        public Action<AudioClip> onAudioClipReceived;

        void Update()
        {
            Microphone.Update();
        }

        public void HandleAudioData(string base64AudioData)
        {
            byte[] audioBytes = Convert.FromBase64String(base64AudioData.Split(',')[1]); // Data URL prefix 제거

            AudioClip clip = CreateAudioClipFromWAV(audioBytes);
            onAudioClipReceived?.Invoke(clip);
        }

        private AudioClip CreateAudioClipFromWAV(byte[] wavBytes)
        {
            // WAV 파일 헤더에서 샘플레이트와 채널 수 읽기
            int channels = wavBytes[22] | wavBytes[23] << 8; // Little endian
            int sampleRate = wavBytes[24] | wavBytes[25] << 8 | wavBytes[26] << 16 | wavBytes[27] << 24; // Little endian

            int headerSize = 44; // 기본 WAV 헤더 크기
            if (wavBytes.Length <= headerSize)
            {
                Debug.LogError("Invalid WAV file");
                return null;
            }

            // WAV 데이터 추출
            int sampleCount = (wavBytes.Length - headerSize) / 2; // 16비트 샘플
            float[] audioData = new float[sampleCount];
            for (int i = 0; i < sampleCount; i++)
            {
                short sample = (short)((wavBytes[headerSize + i * 2 + 1] << 8) | wavBytes[headerSize + i * 2]);
                audioData[i] = sample / 32768.0f; // 16비트 PCM 정규화
            }

            // AudioClip 생성
            AudioClip clip = AudioClip.Create("RecordedClip", sampleCount, channels, sampleRate, false);
            clip.SetData(audioData, 0);
            return clip;
        }
    }
}
#endif