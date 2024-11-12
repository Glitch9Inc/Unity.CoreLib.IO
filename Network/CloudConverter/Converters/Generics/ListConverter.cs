using System;
using System.Collections.Generic;

namespace Glitch9.IO.Network
{
    public class ListConverter<T> : CloudConverter<List<T>>
    {
        public override List<T> ToLocalFormat(string propertyName, object propertyValue)
        {
            if (propertyValue is not List<object> cloudList || cloudList.Count == 0)
                return new List<T>();

            List<T> list = new();
            Type elementType = typeof(T);

            foreach (object item in cloudList)
            {
                T value = (T)CloudConverter.ToLocalFormat(elementType, propertyName, item);
                list.Add(value);
            }

            return list;
        }

        public override object ToCloudFormat(List<T> propertyValue)
        {
            List<object> cloudList = new();

            foreach (T element in propertyValue)
            {
                object value = CloudConverter.ToCloudFormat(element.GetType(), element);
                cloudList.Add(value);
            }

            return cloudList;
        }
    }
}