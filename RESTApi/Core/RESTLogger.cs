namespace Glitch9.IO.RESTApi
{
    public class RESTLogger : ILogger
    {
        internal const string TAG = "RESTApi";
        public virtual void Info(string message) => GNLog.Info(TAG, message);
        public virtual void Warning(string message) => GNLog.Warning(TAG, message);
        public virtual void Error(string message) => GNLog.Error(TAG, message);
        public void Info(string tag, string message) => GNLog.Info(tag, message);
        public void Warning(string tag, string message) => GNLog.Warning(tag, message);
        public void Error(string tag, string message) => GNLog.Error(tag, message);
    }
}