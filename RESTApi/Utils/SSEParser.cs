using System;
using System.Collections.Generic;

namespace Glitch9.IO.RESTApi
{
    public class SSEParser
    {
        private static readonly Dictionary<SSEField, string> _defaultFieldsMap = new()
        {
            { SSEField.Id, "id" },
            { SSEField.Event, "event" },
            { SSEField.Data, "data" },
            { SSEField.Retry, "retry" },
            { SSEField.Error, "error" },
        };

        private readonly Dictionary<SSEField, string> _fieldsMap;

        public SSEParser(Dictionary<SSEField, string> fieldsMap = null)
        {
            _fieldsMap = fieldsMap ?? _defaultFieldsMap;
        }

        public List<(SSEField field, string result)> Parse(string sseString)
        {
            List<(SSEField field, string result)> results = new();
            string[] lines = sseString.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                (SSEField field, string data)? field = GetField(line);
                if (field == null) continue;
                results.Add((field.Value.field, field.Value.data));
            }

            return results;
        }

        private (SSEField field, string data)? GetField(string line)
        {
            foreach (KeyValuePair<SSEField, string> kvp in _fieldsMap)
            {
                if (line.StartsWith(kvp.Value))
                {
                    string data = line.Substring(kvp.Value.Length + 1);
                    return (kvp.Key, data);
                }
            }

            return null;
        }
    }
}