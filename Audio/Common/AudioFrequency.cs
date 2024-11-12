using Glitch9.IO.RESTApi;

namespace Glitch9.CoreLib.IO.Audio
{
    public enum AudioFrequency
    {
        [ApiEnum("8KHz", "8000")]
        Hz8000 = 8000,
        [ApiEnum("16KHz", "16000")]
        Hz16000 = 16000,
        [ApiEnum("24KHz", "24000")]
        Hz24000 = 24000,
        [ApiEnum("32KHz", "32000")]
        Hz32000 = 32000,
        [ApiEnum("44.1KHz", "44100")]
        Hz44100 = 44100,
        [ApiEnum("48KHz", "48000")]
        Hz48000 = 48000,
        [ApiEnum("96KHz", "96000")]
        Hz96000 = 96000,
        [ApiEnum("192KHz", "192000")]
        Hz192000 = 192000
    }
}