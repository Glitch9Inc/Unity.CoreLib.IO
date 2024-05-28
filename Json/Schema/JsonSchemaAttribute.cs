using System;

namespace Glitch9.IO.Json.Schema
{
    [AttributeUsage(AttributeTargets.Property)]
    public class JsonSchemaAttribute : Attribute
    {
        public string PropertyName { get; set; }
        public string Description { get; set; }
        public bool Required { get; set; }
        
        public JsonSchemaAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }
    }
}