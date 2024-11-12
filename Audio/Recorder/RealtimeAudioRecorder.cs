using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Glitch9.CoreLib.IO.Audio
{
    public enum RealtimeAudioRecorderState
    {
        NotInitialized,
        Idle,
        Speaking,
        Stopped,
    }

    public class RealtimeAudioRecorder
    {
        public static class MaxValues
        {
            public const int SampleDurationMs = 10000; // 10 seconds
            public const int SilenceDurationMs = 300000; // 5 minutes
            public const float SilenceThreshold = 0.1f;
        }

        public static class MinValues
        {
            public const int SampleDurationMs = 50; // 0.05 seconds
            public const int SilenceDurationMs = 1000; // 1 second
            public const float SilenceThreshold = 0.005f;
        }

        private float _silenceThreshold = 0.01f;
        public float SilenceThreshold
        {
            get => _silenceThreshold;
            set
            {
                if (value < MinValues.SilenceThreshold || value > MaxValues.SilenceThreshold)
                {
                    _logger.Warning("Silence threshold must be between " + MinValues.SilenceThreshold + " and " + MaxValues.SilenceThreshold);
                    return;
                }
                _silenceThreshold = value;
            }
        }

        private int _sampleDurationMs = 100;
        public int SampleDurationMs
        {
            get => _sampleDurationMs;
            set
            {
                if (value < MinValues.SampleDurationMs || value > MaxValues.SampleDurationMs)
                {
                    _logger.Warning("Sample duration must be between " + MinValues.SampleDurationMs + " and " + MaxValues.SampleDurationMs);
                    return;
                }
                _sampleDurationMs = value;
            }
        }

        private int _silenceDurationMs = 5000;
        public int SilenceDurationMs
        {
            get => _silenceDurationMs;
            set
            {
                if (value < MinValues.SilenceDurationMs || value > MaxValues.SilenceDurationMs)
                {
                    _logger.Warning("Silence duration must be between " + MinValues.SilenceDurationMs + " and " + MaxValues.SilenceDurationMs);
                    return;
                }
                _silenceDurationMs = value;
            }
        }


        private AudioClip _audioClip;
        private int _lastSamplePosition = 0;
        private readonly ILogger _logger;
        private readonly Action<float[]> _onAudioDataAvailable; // Callback for handling streaming data
        private readonly Action _onSpeakingEnded; // Callback for when speaking ends

        private string _microphoneDevice;
        private bool _canRecord => State == RealtimeAudioRecorderState.Speaking || State == RealtimeAudioRecorderState.Idle;

        public AudioFrequency SampleRate = AudioFrequency.Hz16000; // Sample rate of the audio data
        private DateTime _lastInputTime;

        private RealtimeAudioRecorderState? _state;
        public RealtimeAudioRecorderState State
        {
            get
            {
                _state ??= RealtimeAudioRecorderState.NotInitialized;
                return _state.Value;
            }
            private set
            {
                _state = value;
                OnStateChanged?.Invoke(_state.Value);
            }
        }

        public event Action<RealtimeAudioRecorderState> OnStateChanged;
        public event Action<float> OnAudioLevelChanged;

        public RealtimeAudioRecorder(Action<float[]> onAudioDataAvailable, Action onSpeakingEnded, ILogger logger = null)
        {
            _logger = logger ?? new GNLogger(nameof(Audio));
            _onAudioDataAvailable = onAudioDataAvailable;
            _onSpeakingEnded = onSpeakingEnded;
            _microphoneDevice = Microphone.devices[0]; // Default microphone device

            _logger.Info("Realtime audio recorder initialized: " + _microphoneDevice);
        }


        public void StartRecording()
        {
            if (_canRecord)
            {
                _logger.Warning("Already recording!");
                return;
            }

            int sampleRateAsInt = (int)SampleRate;
            if (sampleRateAsInt <= 1)
            {
                _logger.Warning("Invalid sample rate: " + sampleRateAsInt + ". Resetting to 16kHz.");
                SampleRate = AudioFrequency.Hz16000;
                sampleRateAsInt = (int)SampleRate;
            }

            // Start recording with the default microphone, with a length of 10 minutes and at a frequency of sampleRate
            _audioClip = Microphone.Start(_microphoneDevice, true, 600, sampleRateAsInt); // Record for up to 10 minutes
            _lastInputTime = DateTime.Now;
            State = RealtimeAudioRecorderState.Idle;

            // Begin the streaming process
            _lastSamplePosition = 0;
            StreamAudioData().Forget();
            MonitorSilence().Forget(); // Monitor silence
        }

        // Version 2
        public async UniTaskVoid StreamAudioData()
        {
            while (_canRecord)
            {
                await UniTask.Delay(SampleDurationMs); // 지정된 시간 대기

                // 새로운 오디오 샘플이 있는지 확인
                int currentSamplePosition = Microphone.GetPosition(null);
                if (currentSamplePosition > _lastSamplePosition)
                {
                    int sampleLength = currentSamplePosition - _lastSamplePosition;
                    float[] audioData = new float[sampleLength];

                    _audioClip.GetData(audioData, _lastSamplePosition);


                    // 오디오 입력 확인 후, 무음이 아닐 때만 데이터 전송
                    if (HasAudioInput(audioData))
                    {
                        _lastInputTime = DateTime.Now; // 마지막 입력 시간 갱신
                        State = RealtimeAudioRecorderState.Speaking;
                        _onAudioDataAvailable?.Invoke(audioData); // 오디오 데이터 전송

                        if (OnAudioLevelChanged != null)
                        {
                            // 오디오 레벨 계산 후 이벤트 호출
                            float audioLevel = CalculateAudioLevel(audioData);
                            OnAudioLevelChanged?.Invoke(audioLevel);
                        }

                        //_logger.Info("Audio input detected, streaming audio data.");
                    }
                    else
                    {
                        OnAudioLevelChanged?.Invoke(0);
                        //_logger.Info("No significant audio input detected, skipping audio frame.");
                    }

                    _lastSamplePosition = currentSamplePosition;
                }
            }
        }


        private bool HasAudioInput(float[] audioData)
        {
            foreach (float sample in audioData)
            {
                if (Math.Abs(sample) > SilenceThreshold)
                {
                    return true; // There is input
                }
            }
            return false; // No input detected
        }

        // Version 2
        public async UniTaskVoid MonitorSilence()
        {
            while (_canRecord)
            {
                await UniTask.Delay(SampleDurationMs);

                // 침묵 시간이 지나면 녹음을 멈추고 Idle 상태로 변경
                if ((DateTime.Now - _lastInputTime).TotalMilliseconds > SilenceDurationMs)
                {
                    if (State == RealtimeAudioRecorderState.Speaking)
                    {
                        _logger.Info("Silence detected, switching to Idle state.");
                        State = RealtimeAudioRecorderState.Idle; // Idle 상태로 변경
                        _onSpeakingEnded?.Invoke(); // Speaking 종료 이벤트 호출
                    }
                }
            }

            // Idle 상태에서도 계속 소리를 감지하여 다시 녹음 시작
            while (State == RealtimeAudioRecorderState.Idle)
            {
                await UniTask.Delay(SampleDurationMs);

                // 소리가 다시 감지되면 녹음 재개
                int currentSamplePosition = Microphone.GetPosition(null);
                if (currentSamplePosition > _lastSamplePosition)
                {
                    int sampleLength = currentSamplePosition - _lastSamplePosition;
                    float[] audioData = new float[sampleLength];
                    _audioClip.GetData(audioData, _lastSamplePosition);

                    if (HasAudioInput(audioData))
                    {
                        _logger.Info("Audio input detected, resuming recording.");
                        _lastInputTime = DateTime.Now;
                        State = RealtimeAudioRecorderState.Speaking; // 다시 Speaking 상태로 변경
                        _onAudioDataAvailable?.Invoke(audioData);
                        _lastSamplePosition = currentSamplePosition; // 샘플 포지션 갱신
                    }
                }
            }
        }


        public void StopRecording(RealtimeAudioRecorderState state = RealtimeAudioRecorderState.Stopped)
        {
            if (!_canRecord)
            {
                //_logger.Warning("Not currently recording!");
                return;
            }

            // Stop the recording
            Microphone.End(null);
            State = state;
            _logger.Info("Recording stopped");
        }

        public void ResumeRecording()
        {
            if (_canRecord)
            {
                _logger.Warning("Already recording!");
                return;
            }

            _logger.Info("Resuming recording.");
            StartRecording();
        }

        public AudioClip GetRecording()
        {
            if (_canRecord)
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
            if (_canRecord)
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

        public void SetMicrophoneDevice(string device)
        {
            _microphoneDevice = device;
        }

        private float CalculateAudioLevel(float[] audioData)
        {
            float sum = 0f;

            // 모든 오디오 샘플의 절대값을 합산
            foreach (float sample in audioData)
            {
                sum += Mathf.Abs(sample);
            }

            // 평균값을 구하고, 샘플 수로 나누어 정규화된 오디오 레벨 계산
            float average = sum / audioData.Length;

            // 오디오 레벨을 0에서 1 사이의 값으로 정규화
            return Mathf.Clamp01(average / SilenceThreshold);
        }
    }
}
