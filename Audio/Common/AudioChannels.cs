using Glitch9.IO.RESTApi;
using UnityEngine;

namespace Glitch9.CoreLib.IO.Audio
{
    public enum AudioChannels
    {
        [ApiEnum("Mono", "1")]
        Mono = 1,
        [ApiEnum("Stereo", "2")]
        Stereo = 2,
        [ApiEnum("3.1", "4")]
        ThreePointOne = 4,
        [ApiEnum("5.1", "6")]
        FivePointOne = 6,
        [ApiEnum("7.1", "8")]
        SevenPointOne = 8
    }

    public static class AudioChannelsExtensions
    {
        public static int ToInt(this AudioChannels channels)
        {
            return (int)channels;
        }

        public static AudioSpeakerMode ToAudioSpeakerMode(this AudioChannels channels)
        {
            return channels switch
            {
                AudioChannels.Mono => AudioSpeakerMode.Mono,
                AudioChannels.Stereo => AudioSpeakerMode.Stereo,
                AudioChannels.ThreePointOne => AudioSpeakerMode.Prologic,
                AudioChannels.FivePointOne => AudioSpeakerMode.Quad,
                AudioChannels.SevenPointOne => AudioSpeakerMode.Surround,
                _ => AudioSpeakerMode.Stereo,
            };
        }
    }
}