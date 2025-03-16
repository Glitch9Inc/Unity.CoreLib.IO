using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Glitch9.IO.Json.Schema
{
    public class JsonSchemaGenerator
    {
        private class TypeSchema
        {
            public Type Type { get; }
            public JsonSchema Schema { get; }

            public TypeSchema(Type type, JsonSchema schema)
            {
                ThrowIf.ArgumentIsNull(type, nameof(type));
                ThrowIf.ArgumentIsNull(schema, nameof(schema));

                Type = type;
                Schema = schema;
            }
        }

        private readonly IList<TypeSchema> _stack = new List<TypeSchema>();
        private JsonSchema CurrentSchema { get; set; }

        private void Push(TypeSchema typeSchema)
        {
            CurrentSchema = typeSchema.Schema;
            _stack.Add(typeSchema);
        }

        private TypeSchema Pop()
        {
            TypeSchema popped = _stack[_stack.Count - 1];
            _stack.RemoveAt(_stack.Count - 1);
            CurrentSchema = _stack.LastOrDefault()?.Schema;
            return popped;
        }

        public JsonSchema Generate(Type type)
        {
            return GenerateInternal(type);
        }

        public JsonSchema Generate(Dictionary<string, object> dictionary)
        {
            return GenerateInternal(dictionary);
        }


        private JsonSchema GenerateInternal(Type type)
        {
            if (_stack.Any(tc => tc.Type == type))
            {
                throw new JsonException($"Unresolved circular reference for type '{type}'.");
            }

            Push(new TypeSchema(type, new JsonSchema()));
            List<string> requiredProperties = new();

            if (type == typeof(string))
            {
                CurrentSchema.Type = JsonSchemaType.String;
            }
            else if (type == typeof(int) || type == typeof(long) || type == typeof(short) ||
                     type == typeof(byte) || type == typeof(uint) || type == typeof(ulong) ||
                     type == typeof(ushort) || type == typeof(sbyte))
            {
                CurrentSchema.Type = JsonSchemaType.Integer;
            }
            else if (type == typeof(float) || type == typeof(double) || type == typeof(decimal))
            {
                CurrentSchema.Type = JsonSchemaType.Float;
            }
            else if (type == typeof(bool))
            {
                CurrentSchema.Type = JsonSchemaType.Bool;
            }
            else if (type == typeof(DateTime))
            {
                CurrentSchema.Type = JsonSchemaType.String;
                CurrentSchema.Format = "date-time";
            }
            else if (type.IsEnum)
            {
                CurrentSchema.Type = JsonSchemaType.Enum;
                CurrentSchema.Enum = Enum.GetNames(type).ToList();
            }
            else if (type.IsClass)
            {
                CurrentSchema.Type = JsonSchemaType.Object;
                CurrentSchema.Properties = new Dictionary<string, JsonSchema>();

                foreach (PropertyInfo property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (property.CanRead && property.CanWrite)
                    {
                        string propertyName = property.Name;
                        JsonSchemaAttribute attribute = AttributeCache<JsonSchemaAttribute>.Get(property);
                        if (attribute != null)
                        {
                            if (!string.IsNullOrEmpty(attribute.PropertyName)) propertyName = attribute.PropertyName;
                            if (attribute.Required) requiredProperties.Add(propertyName);
                        }
                        JsonSchema propertySchema = GenerateInternal(property.PropertyType);
                        CurrentSchema.Properties.Add(propertyName, propertySchema);
                    }
                }
            }

            // Add JsonContainerAttribute metadata if present
            JsonSchemaAttribute schemaAttribute = AttributeCache<JsonSchemaAttribute>.Get(type);
            if (schemaAttribute != null)
            {
                CurrentSchema.Description = schemaAttribute.Description;
            }

            CurrentSchema.Required = requiredProperties;
            return Pop().Schema;
        }

        private JsonSchema GenerateInternal(Dictionary<string, object> dictionary)
        {
            if (_stack.Any(tc => tc.Type == dictionary.GetType()))
            {
                throw new JsonException($"Unresolved circular reference for type '{dictionary.GetType()}'.");
            }

            Push(new TypeSchema(dictionary.GetType(), new JsonSchema()));
            List<string> requiredProperties = new();

            CurrentSchema.Type = JsonSchemaType.Object;
            CurrentSchema.Properties = new Dictionary<string, JsonSchema>();

            foreach (KeyValuePair<string, object> kvp in dictionary)
            {
                JsonSchema propertySchema = GenerateInternal(kvp.Value.GetType());
                CurrentSchema.Properties.Add(kvp.Key, propertySchema);
            }

            CurrentSchema.Required = requiredProperties;
            return Pop().Schema;
        }
    }
}