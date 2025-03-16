using Glitch9.IO.RESTApi;
using System;

namespace Glitch9.IO.Network
{
    public class EnumConverter<TEnum> : CloudConverter<TEnum> where TEnum : Enum
    {
        public override TEnum ToLocalFormat(string propertyName, object propertyValue)
        {
            string stringValue = CloudConverterUtils.SafeConvertToString(propertyValue);
            if (stringValue == null) return default;

            if (ApiEnumUtils.TryParse(typeof(TEnum), stringValue, out object result, true))
            {
                return (TEnum)result;
            }
            else
            {
                GNLog.Error($"Failed to parse enum: {stringValue}");
                return default;
            }
        }

        public override object ToCloudFormat(TEnum propertyValue)
        {
            if (propertyValue.GetType().TryGetAttribute(out ApiEnumAttribute attribute))
            {
                return attribute.ApiName;
            }
            return propertyValue.ToString();
        }
    }
}