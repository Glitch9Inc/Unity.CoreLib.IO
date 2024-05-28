using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Glitch9.IO.Json.Schema
{
    internal class JsonSchemaBuilder
    {
        public JsonSchema Read(JsonReader reader)
        {
            JObject obj = JObject.Load(reader);
            return ParseSchema(obj);
        }

        private JsonSchema ParseSchema(JObject obj)
        {
            JsonSchema schema = new()
            {
                Description = (string)obj["description"],
                Type = (string)obj["type"],
                Format = (string)obj["format"]
            };

            if (obj["properties"] is JObject properties)
            {
                schema.Properties = new Dictionary<string, JsonSchema>();
                foreach (KeyValuePair<string, JToken> property in properties)
                {
                    schema.Properties.Add(property.Key, ParseSchema((JObject)property.Value));
                }
            }

            if (obj["enum"] is JArray enumArray)
            {
                schema.Enum = new List<string>();
                foreach (JToken token in enumArray)
                {
                    schema.Enum.Add(token.ToString());
                }
            }

            if (obj["required"] is JArray requiredArray)
            {
                schema.Required = new List<string>();
                foreach (JToken token in requiredArray)
                {
                    schema.Required.Add(token.ToString());
                }
            }

            return schema;
        }
    }
}