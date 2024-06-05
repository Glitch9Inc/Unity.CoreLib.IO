using System;

namespace Glitch9.IO.RESTApi
{
    public readonly struct ApiEnum<T> where T : Enum
    {
        public static implicit operator T(ApiEnum<T> apiEnum) => apiEnum._value;
        public static implicit operator ApiEnum<T>(T value) => new(value);
        public static implicit operator string(ApiEnum<T> apiEnum) => apiEnum._name;
        public static implicit operator ApiEnum<T>(string apiName) => new(apiName);

        private readonly T _value;
        private readonly string _name;

        public ApiEnum(T value)
        {
            _value = value;
            _name = value.ToApiName();
        }

        public ApiEnum(string apiName)
        {
            _value = ApiEnumUtils.ParseEnum<T>(apiName);
            _name = apiName;
        }

        public override bool Equals(object obj)
        {
            return obj is ApiEnum<T> other && Equals(other);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public override string ToString()
        {
            return _name;
        }
    }
}