// ReSharper disable All

namespace Glitch9.IO.Json.Schema
{
    public enum JsonSchemaType
    {
        String,
        Integer,
        Float,
        Bool,
        Object,
        Array,
        Null,
        Enum
    }


    public class JsonSchemaTypes
    {
        public const string None = "none";
        public const string Object = "object";
        public const string Array = "array";
        public const string Integer = "integer";
        public const string Float = "number";
        public const string String = "string";
        public const string Boolean = "boolean";
        public const string Null = "null";

        public static string GetValue(JsonSchemaType type)
        {
            return type switch
            {
                JsonSchemaType.String => String,
                JsonSchemaType.Integer => Integer,
                JsonSchemaType.Float => Float,
                JsonSchemaType.Bool => Boolean,
                JsonSchemaType.Object => Object,
                JsonSchemaType.Array => Array,
                JsonSchemaType.Null => Null,
                JsonSchemaType.Enum => String,
                _ => None
            };
        }

        public static JsonSchemaType Parse(string typeString)
        {
            return typeString switch
            {
                String => JsonSchemaType.String,
                Float => JsonSchemaType.Float,
                Integer => JsonSchemaType.Integer,
                Boolean => JsonSchemaType.Bool,
                Object => JsonSchemaType.Object,
                Array => JsonSchemaType.Array,
                Null => JsonSchemaType.Null,
                _ => JsonSchemaType.String
            };
        }
    }
}