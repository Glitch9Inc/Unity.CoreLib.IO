using System;
using System.IO;
using Glitch9.IO.Files;
using UnityEngine;

namespace Glitch9.IO.RESTApi
{
    public struct FormFile
    {
        public readonly bool IsEmpty => Data == null || Data.Length == 0;
        public byte[] Data { get; set; }
        public string FileName { get; set; }
        public ContentType ContentType { get; set; } // MIME 타입을 저장할 문자열 프로퍼티

        public FormFile(byte[] data, string fileName, ContentType contentType = ContentType.Json)
        {
            if (data == null || data.Length == 0) throw new ArgumentException("Data cannot be null or empty", nameof(data));
            if (string.IsNullOrEmpty(fileName)) throw new ArgumentException("FileName cannot be null or empty", nameof(fileName));
            Data = data;
            FileName = fileName;
            ContentType = contentType;
        }

        public FormFile(Sprite sprite, string fileName, ContentType contentType = ContentType.Json)
        {
            if (sprite == null) throw new ArgumentException("Sprite cannot be null", nameof(sprite));
            if (string.IsNullOrEmpty(fileName)) throw new ArgumentException("FileName cannot be null or empty", nameof(fileName));
            Data = sprite.texture.EncodeToPNG();
            FileName = fileName;
            ContentType = contentType;
        }

        public FormFile(Texture2D texture, string fileName, ContentType contentType = ContentType.Json)
        {
            if (texture == null) throw new ArgumentException("Texture cannot be null", nameof(texture));
            if (string.IsNullOrEmpty(fileName)) throw new ArgumentException("FileName cannot be null or empty", nameof(fileName));
            Data = texture.EncodeToPNG();
            FileName = fileName;
            ContentType = contentType;
        }

        public FormFile(AudioClip audioClip, string fileName, ContentType contentType = ContentType.Json)
        {
            if (audioClip == null) throw new ArgumentException("AudioClip cannot be null", nameof(audioClip));
            if (string.IsNullOrEmpty(fileName)) throw new ArgumentException("FileName cannot be null or empty", nameof(fileName));
            Data = audioClip.ToBytes();
            FileName = fileName;
            ContentType = contentType;
        }

        public FormFile(string filePath, ContentType contentType = ContentType.Json)
        {
            if (string.IsNullOrEmpty(filePath)) throw new ArgumentException("FilePath cannot be null or empty", nameof(filePath));
            Data = File.ReadAllBytes(filePath);
            FileName = Path.GetFileName(filePath);
            ContentType = contentType;
        }

        public FormFile(string filePath, string fileName, ContentType contentType = ContentType.Json)
        {
            if (string.IsNullOrEmpty(filePath)) throw new ArgumentException("FilePath cannot be null or empty", nameof(filePath));
            if (string.IsNullOrEmpty(fileName)) throw new ArgumentException("FileName cannot be null or empty", nameof(fileName));
            Data = File.ReadAllBytes(filePath);
            FileName = fileName;
            ContentType = contentType;
        }
    }

    public static class FormFileExtensions
    {
        public static FormFile ToFormFile(this UnityAudioFile audioFile)
        {
            return new FormFile(audioFile.Value, audioFile.FileName);
        }

        public static FormFile ToFormFile(this UnityImageFile imageFile)
        {
            return new FormFile(imageFile.Value, imageFile.FileName);
        }

        public static FormFile ToFormFile(this UnityFile unityFile)
        {
            return new FormFile(unityFile.Value, unityFile.FileName);
        }
    }
}