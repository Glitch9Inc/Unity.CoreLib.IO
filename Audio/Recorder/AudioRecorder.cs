using UnityEngine;

namespace Glitch9.CoreLib.IO.Audio
{
    public class AudioRecorder
    {
        private AudioClip _audioClip;
        private bool _isRecording = false;
        private readonly ILogger _logger;

        public AudioRecorder(ILogger logger = null)
        {
            _logger = logger ?? new GNLogger(nameof(AudioRecorder));
        }

        public void StartRecording()
        {
            if (_isRecording)
            {
                _logger.Warning("Already recording!");
                return;
            }

            // Start recording with the default microphone, with a length of 10 seconds and at a frequency of 44100 Hz
#if UNITY_WEBGL && !UNITY_EDITOR
            Microphone.onAudioClipReceived = clip => OnAudioClipReceived(clip);
            Microphone.Start(null, false, 10, 44100);
#else
            AudioClip clip = Microphone.Start(null, false, 10, 44100);
            OnAudioClipReceived(clip);
#endif
            _isRecording = true;

            _logger.Info("Recording started");
        }

        private void OnAudioClipReceived(AudioClip clip)
        {
            if (clip == null)
            {
                _logger.Error("Failed to start recording...");
                return;
            }
            _audioClip = clip;
        }

        public AudioClip StopRecording()
        {
            if (!_isRecording)
            {
                _logger.Warning("Not currently recording!");
                return null;
            }

            // Stop the recording
            Microphone.End(null);
            _isRecording = false;
            _logger.Info("Recording stopped");

            return _audioClip;
        }

        public AudioClip GetRecording()
        {
            if (_isRecording)
            {
                _logger.Warning("Cannot get recording while recording!");
                return null;
            }

            if (_audioClip == null)
            {
                _logger.Warning("No recording found!");
                return null;
            }

            return _audioClip;
        }

        public void PlayRecording(GameObject gameObject)
        {
            if (_isRecording)
            {
                _logger.Warning("Cannot play while recording!");
                return;
            }

            if (_audioClip == null)
            {
                _logger.Warning("No recording found!");
                return;
            }

            // Create an AudioSource component dynamically and play the recorded audio
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = _audioClip;
            audioSource.Play();
            _logger.Info("Playing recording");
        }
    }
}