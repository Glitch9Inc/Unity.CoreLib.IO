using System;

namespace Glitch9.IO.RESTApi
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class RequiredAttribute : Attribute
    {
    }
}