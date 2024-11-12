using System;

namespace Glitch9.IO.Network
{
    public class TimeSpanConverter : CloudConverter<TimeSpan>
    {
        public override TimeSpan ToLocalFormat(string propertyName, object propertyValue)
        {
            if (propertyValue is double doubleValue)
            {
                return TimeSpan.FromMilliseconds(doubleValue);
            }

            GNLog.Error($"Failed to parse TimeSpan: {propertyValue}");
            return TimeSpan.Zero;
        }

        public override object ToCloudFormat(TimeSpan propertyValue)
        {
            return propertyValue.TotalMilliseconds;
        }
    }
}