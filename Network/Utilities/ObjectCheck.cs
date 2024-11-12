using System;

namespace Glitch9.IO.Network
{
    public class ObjectCheck<TObject> where TObject : class, new()
    {
        public Func<TObject, bool> Check { get; }
        public string ExceptionMessage { get; }

        public ObjectCheck(Func<TObject, bool> check, string exceptionMessage)
        {
            Check = check;
            ExceptionMessage = exceptionMessage;
        }
    }
}