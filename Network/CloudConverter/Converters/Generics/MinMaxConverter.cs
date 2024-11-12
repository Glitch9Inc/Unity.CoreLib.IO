using System;

namespace Glitch9.IO.Network
{
    public class MinMaxConverter<T> : CloudConverter<MinMax<T>> where T : IComparable<T>
    {
        public override MinMax<T> ToLocalFormat(string propertyName, object propertyValue)
        {
            T convertedValue = (T)CloudConverterUtils.ConvertIComparable(typeof(T), propertyValue);
            return new MinMax<T> { Value = convertedValue };
        }

        public override object ToCloudFormat(MinMax<T> propertyValue)
        {
            return propertyValue.Value;
        }
    }
}