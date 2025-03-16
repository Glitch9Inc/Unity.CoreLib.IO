using UnityEngine;
using UnityEngine.Networking;
using System;
using Cysharp.Threading.Tasks;

namespace Glitch9.IO.Files
{
    public class AudioProcessor
    {
        /// <summary>
        /// Process an audio file and return the base64 encoded audio data (PCM16).
        /// </summary>
        public async UniTask<string> ProcessPCM16Audio(string audioFilePath)
        {
            AudioClip audioClip = await LoadAudioClipFromFile(audioFilePath);

            if (audioClip == null)
            {
                Debug.LogError("Failed to load audio file.");
                return null;
            }

            return AudioClipToPCM16Base64(audioClip);
        }

        /// <summary>
        /// Process an audio file and return the base64 encoded audio data (G.711 μ-law).
        /// </summary>
        public async UniTask<string> ProcessG711uLawAudio(string audioFilePath)
        {
            AudioClip audioClip = await LoadAudioClipFromFile(audioFilePath);

            if (audioClip == null)
            {
                Debug.LogError("Failed to load audio file.");
                return null;
            }

            return AudioClipToG711uLawBase64(audioClip);
        }

        /// <summary>
        /// Process an audio file and return the base64 encoded audio data (G.711 A-law).
        /// </summary>
        public async UniTask<string> ProcessG711aLawAudio(string audioFilePath)
        {
            AudioClip audioClip = await LoadAudioClipFromFile(audioFilePath);

            if (audioClip == null)
            {
                Debug.LogError("Failed to load audio file.");
                return null;
            }

            return AudioClipToG711aLawBase64(audioClip);
        }

        // Loads an AudioClip from a file
        private async UniTask<AudioClip> LoadAudioClipFromFile(string path)
        {
            string url = "file://" + path;

            using UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.WAV); // or AudioType.OGGVORBIS
            await www.SendWebRequest().ToUniTask();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(www.error);
                return null;
            }

            return DownloadHandlerAudioClip.GetContent(www);
        }

        // Converts an AudioClip into base64-encoded PCM16 data
        public string AudioClipToPCM16Base64(AudioClip audioClip)
        {
            float[] audioData = new float[audioClip.samples];
            audioClip.GetData(audioData, 0);
            byte[] pcm16Data = FloatTo16BitPCM(audioData);
            return Convert.ToBase64String(pcm16Data);
        }

        // Converts an AudioClip into base64-encoded G.711 μ-law data
        public string AudioClipToG711uLawBase64(AudioClip audioClip)
        {
            float[] audioData = new float[audioClip.samples];
            audioClip.GetData(audioData, 0);
            byte[] g711uLawData = FloatToG711uLaw(audioData);
            return Convert.ToBase64String(g711uLawData);
        }

        // Converts an AudioClip into base64-encoded G.711 A-law data
        public string AudioClipToG711aLawBase64(AudioClip audioClip)
        {
            float[] audioData = new float[audioClip.samples];
            audioClip.GetData(audioData, 0);
            byte[] g711aLawData = FloatToG711aLaw(audioData);
            return Convert.ToBase64String(g711aLawData);
        }

        public string FloatArrayToPCM16Base64(float[] floatArray)
        {
            byte[] pcm16Data = FloatTo16BitPCM(floatArray);
            return Convert.ToBase64String(pcm16Data);
        }

        public string FloatArrayToG711uLawBase64(float[] floatArray)
        {
            byte[] g711uLawData = FloatToG711uLaw(floatArray);
            return Convert.ToBase64String(g711uLawData);
        }

        public string FloatArrayToG711aLawBase64(float[] floatArray)
        {
            byte[] g711aLawData = FloatToG711aLaw(floatArray);
            return Convert.ToBase64String(g711aLawData);
        }

        // Converts a float array of audio data to PCM16 byte array
        private byte[] FloatTo16BitPCM(float[] floatArray)
        {
            var buffer = new byte[floatArray.Length * 2];
            int offset = 0;

            for (int i = 0; i < floatArray.Length; i++, offset += 2)
            {
                float sample = Mathf.Clamp(floatArray[i], -1f, 1f);
                short pcmValue = (short)(sample < 0 ? sample * 0x8000 : sample * 0x7FFF);
                buffer[offset] = (byte)(pcmValue & 0xFF);
                buffer[offset + 1] = (byte)((pcmValue >> 8) & 0xFF);
            }

            return buffer;
        }

        // Converts a float array of audio data to G.711 μ-law byte array
        private byte[] FloatToG711uLaw(float[] floatArray)
        {
            byte[] g711uLawData = new byte[floatArray.Length];
            for (int i = 0; i < floatArray.Length; i++)
            {
                short pcmValue = (short)(Mathf.Clamp(floatArray[i], -1f, 1f) * 32768);
                g711uLawData[i] = EncodeG711uLaw(pcmValue);
            }
            return g711uLawData;
        }

        // Converts a float array of audio data to G.711 A-law byte array
        private byte[] FloatToG711aLaw(float[] floatArray)
        {
            byte[] g711aLawData = new byte[floatArray.Length];
            for (int i = 0; i < floatArray.Length; i++)
            {
                short pcmValue = (short)(Mathf.Clamp(floatArray[i], -1f, 1f) * 32768);
                g711aLawData[i] = EncodeG711aLaw(pcmValue);
            }
            return g711aLawData;
        }

        // Encode a single PCM16 sample to G.711 μ-law
        private byte EncodeG711uLaw(short pcm16)
        {
            // Implementing G.711 μ-law encoding algorithm
            const int BIAS = 0x84;
            const int MAX = 0x7FFF;

            int sign = (pcm16 >> 8) & 0x80; // Extract sign
            if (sign != 0)
                pcm16 = (short)-pcm16; // Get magnitude

            pcm16 += BIAS;
            if (pcm16 > MAX) pcm16 = MAX;

            int exponent = 7;
            int mantissa;
            for (int expMask = 0x4000; (pcm16 & expMask) == 0 && exponent > 0; exponent--, expMask >>= 1) { }

            mantissa = (pcm16 >> (exponent + 3)) & 0x0F;
            byte g711uLawByte = (byte)(sign | (exponent << 4) | mantissa);
            return (byte)~g711uLawByte;
        }

        // Encode a single PCM16 sample to G.711 A-law
        private byte EncodeG711aLaw(short pcm16)
        {
            // Implementing G.711 A-law encoding algorithm
            const int MAX = 0x7FFF;

            int sign = (pcm16 >> 8) & 0x80; // Extract sign
            if (sign != 0)
                pcm16 = (short)-pcm16; // Get magnitude

            if (pcm16 > MAX) pcm16 = MAX;

            int exponent = 7;
            int mantissa;
            for (int expMask = 0x1000; (pcm16 & expMask) == 0 && exponent > 0; exponent--, expMask >>= 1) { }

            mantissa = (pcm16 >> ((exponent == 0) ? 4 : (exponent + 3))) & 0x0F;
            byte g711aLawByte = (byte)((exponent << 4) | mantissa);
            return (byte)(sign | g711aLawByte ^ 0x55); // A-law is biased differently
        }

        // Base64로 인코딩된 PCM16 데이터를 AudioClip으로 변환
        public AudioClip PCM16Base64ToAudioClip(string base64EncodedString, int sampleRate, int channels)
        {
            byte[] pcm16Data = Convert.FromBase64String(base64EncodedString);
            float[] audioData = PCM16ToFloatArray(pcm16Data);

            return CreateAudioClipFromFloatArray(audioData, sampleRate, channels);
        }

        // Base64로 인코딩된 G711uLaw 데이터를 AudioClip으로 변환
        public AudioClip G711uLawBase64ToAudioClip(string base64EncodedString, int sampleRate, int channels)
        {
            byte[] g711uLawData = Convert.FromBase64String(base64EncodedString);
            float[] audioData = G711uLawToFloatArray(g711uLawData);

            return CreateAudioClipFromFloatArray(audioData, sampleRate, channels);
        }

        // Base64로 인코딩된 G711aLaw 데이터를 AudioClip으로 변환
        public AudioClip G711aLawBase64ToAudioClip(string base64EncodedString, int sampleRate, int channels)
        {
            byte[] g711aLawData = Convert.FromBase64String(base64EncodedString);
            float[] audioData = G711aLawToFloatArray(g711aLawData);

            return CreateAudioClipFromFloatArray(audioData, sampleRate, channels);
        }

        public float[] PCM16ToFloatArray(string base64EncodedString)
        {
            byte[] pcm16Data = Convert.FromBase64String(base64EncodedString);
            return PCM16ToFloatArray(pcm16Data);
        }

        public float[] G711uLawToFloatArray(string base64EncodedString)
        {
            byte[] g711uLawData = Convert.FromBase64String(base64EncodedString);
            return G711uLawToFloatArray(g711uLawData);
        }

        public float[] G711aLawToFloatArray(string base64EncodedString)
        {
            byte[] g711aLawData = Convert.FromBase64String(base64EncodedString);
            return G711aLawToFloatArray(g711aLawData);
        }

        // PCM16 byte array를 float array로 변환
        private float[] PCM16ToFloatArray(byte[] pcm16Bytes)
        {
            int sampleCount = pcm16Bytes.Length / 2;
            float[] floatArray = new float[sampleCount];

            for (int i = 0; i < sampleCount; i++)
            {
                short sample = BitConverter.ToInt16(pcm16Bytes, i * 2);
                floatArray[i] = sample / 32768f; // PCM16을 float(-1 ~ 1)로 변환
            }

            return floatArray;
        }

        // G.711 μ-law byte array를 float array로 변환
        private float[] G711uLawToFloatArray(byte[] g711uLawBytes)
        {
            float[] floatArray = new float[g711uLawBytes.Length];

            for (int i = 0; i < g711uLawBytes.Length; i++)
            {
                floatArray[i] = DecodeG711uLaw(g711uLawBytes[i]) / 32768f;
            }

            return floatArray;
        }

        // G.711 A-law byte array를 float array로 변환
        private float[] G711aLawToFloatArray(byte[] g711aLawBytes)
        {
            float[] floatArray = new float[g711aLawBytes.Length];

            for (int i = 0; i < g711aLawBytes.Length; i++)
            {
                floatArray[i] = DecodeG711aLaw(g711aLawBytes[i]) / 32768f;
            }

            return floatArray;
        }

        // G.711 μ-law 디코딩
        private short DecodeG711uLaw(byte ulawByte)
        {
            ulawByte = (byte)~ulawByte;
            int sign = (ulawByte & 0x80);
            int exponent = (ulawByte >> 4) & 0x07;
            int mantissa = ulawByte & 0x0F;
            int sample = ((mantissa << 3) | 0x84) << (exponent + 2);
            return (short)(sign != 0 ? -sample : sample);
        }

        // G.711 A-law 디코딩
        private short DecodeG711aLaw(byte alawByte)
        {
            alawByte ^= 0x55;
            int sign = (alawByte & 0x80);
            int exponent = (alawByte >> 4) & 0x07;
            int mantissa = alawByte & 0x0F;
            int sample = (mantissa << 4) | 0x08;
            if (exponent != 0)
            {
                sample += 0x100;
                sample <<= (exponent - 1);
            }
            return (short)(sign == 0 ? sample : -sample);
        }

        // float array 데이터를 이용해 AudioClip 생성
        private AudioClip CreateAudioClipFromFloatArray(float[] audioData, int sampleRate, int channels)
        {
            AudioClip audioClip = AudioClip.Create("DecodedAudioClip", audioData.Length, channels, sampleRate, false);
            audioClip.SetData(audioData, 0);
            return audioClip;
        }
    }
}
