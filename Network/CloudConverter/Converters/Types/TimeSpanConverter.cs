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

            GNLog.ParseFail(typeof(TimeSpan));
            return TimeSpan.Zero;
        }

        public override object ToCloudFormat(TimeSpan propertyValue)
        {
            return propertyValue.TotalMilliseconds;
        }
    }
}