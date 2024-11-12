using System;

namespace Glitch9.IO.RESTApi
{
    public class StreamHandler
    {
        public event Action<object> OnStart;
        public event Action<object, string> OnStream;
        public event Action<object> OnDone;

        public void Start(object sender)
        {
            OnStart?.Invoke(sender);
        }

        public void Stream(object sender, string message)
        {
            OnStream?.Invoke(sender, message);
        }
        
        public void Done(object sender)
        {
            OnDone?.Invoke(sender);
        }
    }
}