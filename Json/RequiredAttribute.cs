using System;

namespace Glitch9.IO.Json
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RequiredAttribute : Attribute
    {
        public bool AllowEmptyStrings { get; set; } = false;
    }
}