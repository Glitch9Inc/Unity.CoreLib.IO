using System;

namespace Glitch9.IO.RESTApi
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class ApiEnumAttribute : DisplayNameAttribute
    {
        public string ApiName { get; protected set; }

        public ApiEnumAttribute(string apiName)
        {
            ApiName = apiName;
        }

        public ApiEnumAttribute(string displayName, string apiName)
        {
            DisplayName = displayName;
            ApiName = apiName;
        }
    }
}