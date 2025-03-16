using System.Collections.Generic;

namespace Glitch9.IO.RESTApi
{
    public interface ISSEParser
    {
        List<(SSEField field, string result)> Parse(string sseString);
    }
}