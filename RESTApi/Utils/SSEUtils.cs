using System;
using System.Collections.Generic;

namespace Glitch9.IO.RESTApi
{
    public static class SSEUtils
    {
        public static List<(SSEField field, string result)> Parse(string sseString)
        {
            List<(SSEField field, string result)> results = new();
            string[] lines = sseString.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                if (line.StartsWith("data:"))
                {
                    string data = line.Substring(5);
                    results.Add((SSEField.Data, data));
                }
                else if (line.StartsWith("event:"))
                {
                    string eventName = line.Substring(6);
                    results.Add((SSEField.Event, eventName));
                }
                else if (line.StartsWith("id:"))
                {
                    string id = line.Substring(3);
                    results.Add((SSEField.Id, id));
                }
                else if (line.StartsWith("retry:"))
                {
                    string retry = line.Substring(6);
                    results.Add((SSEField.Retry, retry));
                }
            }

            return results;
        }
    }
}