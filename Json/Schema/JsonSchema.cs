using Newtonsoft.Json;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Glitch9.IO.Json.Schema
{
    public class JsonSchema
    {
        [JsonProperty("type")] public string Type { get; set; }
        [JsonProperty("description")] public string Description { get; set; }
        [JsonProperty("format")] public string Format { get; set; }
        [JsonProperty("properties")] public Dictionary<string, JsonSchema> Properties { get; set; }
        [JsonProperty("enum")] public List<string> Enum { get; set; }
        [JsonProperty("required")] public List<string> Required { get; set; }


        public static JsonSchema Read(JsonReader reader)
        {
            JsonSchemaBuilder jsonSchemaBuilder = new();
            return jsonSchemaBuilder.Read(reader);
        }

        public void WriteTo(JsonWriter writer)
        {
            JsonSchemaWriter jsonSchemaWriter = new(writer);
            jsonSchemaWriter.WriteSchema(this);
        }

        public override string ToString()
        {
            StringWriter writer = new(CultureInfo.InvariantCulture);
            using (JsonTextWriter jsonWriter = new(writer))
            {
                jsonWriter.Formatting = Formatting.Indented;
                WriteTo(jsonWriter);
            }

            return writer.ToString();
        }

        public static JsonSchema Create<TToolResponse>()
        {
            JsonSchemaGenerator generator = new();
            return generator.Generate(typeof(TToolResponse));
        }
    }
}
