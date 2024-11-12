using System.IO;
using UnityEngine;

namespace Glitch9.IO.Files
{
    public class AudioUtils
    {
        public static AudioType GetAudioTypeFromFileExtension(string extensionString)
        {
            if (string.IsNullOrEmpty(extensionString)) return AudioType.UNKNOWN;
            if (!extensionString.StartsWith(".")) extensionString = "." + extensionString;
            
            switch (extensionString)
            {
                case ".wav": return AudioType.WAV;
                case ".mp3": return AudioType.MPEG;
                case ".ogg": return AudioType.OGGVORBIS;
                case ".aif":
                case ".aiff": return AudioType.AIFF;
                case ".acc": return AudioType.ACC;
                case ".it": return AudioType.IT;
                case ".mod": return AudioType.MOD;
                case ".s3m": return AudioType.S3M;
                case ".xm": return AudioType.XM;
                case ".xma": return AudioType.XMA;
                case ".vag": return AudioType.VAG;
                default:
                    GNLog.Error($"Unsupported Audio Format: {extensionString}");
                    return AudioType.UNKNOWN;
            }
        }

        public static AudioType GetAudioTypeFromFilePath(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLower();
            return GetAudioTypeFromFileExtension(extension);
        }
    }
}