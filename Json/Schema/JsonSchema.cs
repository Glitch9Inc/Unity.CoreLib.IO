using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Glitch9.IO.Json.Schema
{
    /// <summary>
    /// The Schema object allows the definition of input and output data types.
    /// These types can be objects, but also primitives and arrays.
    /// Represents a select subset of an OpenAPI 3.0 schema object.
    /// </summary>
    public class JsonSchema
    {
        /// <summary>
        /// Required.
        /// Data type.
        /// </summary>
        [JsonProperty("type")] public JsonSchemaType Type { get; set; }

        /// <summary>
        /// Optional.
        /// The format of the data.
        /// This is used only for primitive data types.
        /// <para>
        /// OpenAI Supported formats: string only.
        /// </para>
        /// <para>
        /// Google Generative AI Supported formats: for NUMBER type: float, double for INTEGER type: int32, int64
        /// </para>
        /// </summary>
        [JsonProperty("format")] public string Format { get; set; }

        /// <summary>
        /// Optional.
        /// A brief description of the parameter.
        /// This could contain examples of use.
        /// Parameter description may be formatted as Markdown.
        /// </summary>
        [JsonProperty("description")] public string Description { get; set; }

        /// <summary>
        /// Optional.
        /// Indicates if the value may be null.
        /// </summary>
        [JsonProperty("nullable")] public bool? Nullable { get; set; }

        /// <summary>
        /// Optional.
        /// Possible values of the element of Type.STRING with enum format.
        /// For example we can define an Enum Direction as : {type:STRING, format:enum, enum:["EAST", NORTH", "SOUTH", "WEST"]}
        /// </summary>
        [JsonProperty("enum")] public List<string> Enum { get; set; }

        /// <summary>
        /// map (key: string, value: object (Schema))
        /// Optional. Properties of Type.OBJECT.
        /// An object containing a list of "key": value pairs. Example: { "name": "wrench", "mass": "1.3kg", "count": "3" }.
        /// </summary>
        [JsonProperty("properties")] public Dictionary<string, JsonSchema> Properties { get; set; }

        /// <summary>
        /// Optional.
        /// Required properties of Type.OBJECT.
        /// </summary>
        [JsonProperty("required")] public List<string> Required { get; set; }

        /// <summary>
        /// Optional.
        /// Schema of the elements of Type.ARRAY.
        /// </summary>
        [JsonProperty("items")] public JsonSchema Items { get; set; }


        public static JsonSchema Read(JsonReader reader)
        {
            if (reader == null) throw new System.ArgumentNullException(nameof(reader));
            JsonSchemaReader jsonSchemaReader = new();
            return jsonSchemaReader.Read(reader);
        }

        public void WriteTo(JsonWriter writer, StringCase typeStringCase)
        {
            JsonSchemaWriter jsonSchemaWriter = new(writer);
            jsonSchemaWriter.WriteSchema(this, typeStringCase);
        }

        public string ToString(StringCase typeStringCase)
        {
            StringWriter writer = new(CultureInfo.InvariantCulture);
            using (JsonTextWriter jsonWriter = new(writer))
            {
                jsonWriter.Formatting = Formatting.Indented;
                WriteTo(jsonWriter, typeStringCase);
            }

            return writer.ToString();
        }

        public static JsonSchema Create<TToolResponse>()
        {
            return Create(typeof(TToolResponse));
        }

        public static JsonSchema Create(Type type)
        {
            JsonSchemaGenerator generator = new();
            return generator.Generate(type);
        }

        public static JsonSchema Create(Dictionary<string, object> dictionary)
        {
            JsonSchemaGenerator generator = new();
            return generator.Generate(dictionary);
        }
    }
}
