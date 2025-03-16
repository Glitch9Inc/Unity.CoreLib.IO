using System;

namespace Glitch9.IO.RESTApi
{
    public class NoAPIKeyException : Exception
    {
        public NoAPIKeyException(string apiName) : base($"{apiName} API requires an API key to be set.")
        {
        }
    }

    public class NoAPIKeyQueryKeyException : Exception
    {
        public NoAPIKeyQueryKeyException(string apiName) : base($"{apiName} API requires a query key for your API key.")
        {
        }
    }
}