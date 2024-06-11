namespace Glitch9.IO.Network
{

    public class CloudConverterLogger : ILogger
    {
        private const string TAG = "CloudConverter";
        public void Info(string message)
        {
            GNLog.Info(TAG, message);
        }

        public void Warning(string message)
        {
            GNLog.Warning(TAG, message);
        }

        public void Error(string message)
        {
            GNLog.Error(TAG, message);
        }

        public void Info(string tag, string message)
        {
            GNLog.Info(tag, message);
        }

        public void Warning(string tag, string message)
        {
            GNLog.Warning(tag, message);
        }

        public void Error(string tag, string message)
        {
            GNLog.Error(tag, message);
        }
    }
}