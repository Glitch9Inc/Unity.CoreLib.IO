using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Glitch9.IO.Json.Schema
{
    internal class JsonSchemaWriter
    {
        private readonly JsonWriter _writer;

        public JsonSchemaWriter(JsonWriter writer)
        {
            _writer = writer ?? throw new ArgumentNullException(nameof(writer));
        }

        public void WriteSchema(JsonSchema schema)
        {
            if (schema == null) throw new ArgumentNullException(nameof(schema));

            _writer.WriteStartObject();

            WritePropertyIfNotNull("description", schema.Description);
            WritePropertyIfNotNull("type", schema.Type);
            WritePropertyIfNotNull("format", schema.Format);

            if (schema.Properties != null)
            {
                _writer.WritePropertyName("properties");
                _writer.WriteStartObject();
                foreach (KeyValuePair<string, JsonSchema> property in schema.Properties)
                {
                    _writer.WritePropertyName(property.Key);
                    WriteSchema(property.Value);
                }
                _writer.WriteEndObject();
            }

            if (schema.Enum != null)
            {
                _writer.WritePropertyName("enum");
                _writer.WriteStartArray();
                foreach (string enumValue in schema.Enum)
                {
                    _writer.WriteValue(enumValue);
                }
                _writer.WriteEndArray();
            }

            if (schema.Required != null)
            {
                _writer.WritePropertyName("required");
                _writer.WriteStartArray();
                foreach (string required in schema.Required)
                {
                    _writer.WriteValue(required);
                }
                _writer.WriteEndArray();
            }

            _writer.WriteEndObject();
        }

        private void WritePropertyIfNotNull(string propertyName, object value)
        {
            if (value != null)
            {
                _writer.WritePropertyName(propertyName);
                _writer.WriteValue(value);
            }
        }
    }
}
