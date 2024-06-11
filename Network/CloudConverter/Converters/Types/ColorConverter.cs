using UnityEngine;

namespace Glitch9.IO.Network
{
    public class ColorConverter : CloudConverter<Color>
    {
        private static readonly Color k_DefaultColor = ExColor.lavender;

        public override Color ToLocalFormat(string propertyName, object propertyValue)
        {
            string stringValue = CloudConverterUtils.SafeConvertToString(propertyValue);
            if (stringValue == null) return k_DefaultColor;
            if (stringValue.TryParseToColor(out Color color))
            {
                return color;
            }
            else
            {
                GNLog.Warning($"string '{stringValue}'를 Color로 변환할 수 없습니다. 기본값으로 lavender를 반환합니다.");
                return k_DefaultColor;
            }
        }

        public override object ToCloudFormat(Color propertyValue)
        {
            return propertyValue.ToHex();
        }
    }
}