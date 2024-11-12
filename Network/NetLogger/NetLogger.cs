namespace Glitch9.IO.Network
{
    [System.Flags]
    public enum NetLogType
    {
        None = 0,
        Verbose = 1 << 0,
        RequestHeader = 1 << 1,
        RequestBody = 1 << 2,
        ResponseBody = 1 << 3,
        ResponseDelta = 1 << 4,
        All = Verbose | RequestHeader | RequestBody | ResponseBody | ResponseDelta,
    }

    public enum NetLogLevel
    {
        [DisplayName("Default")]
        Default,

        [DisplayName("Log Operation Details")]
        Verbose,

        [DisplayName("Log Request Details")]
        DebugRequest,

        [DisplayName("Log Response Details")]
        DebugResponse,

        [DisplayName("Log Stream Events")]
        DebugStream,

        [DisplayName("Log All")]
        DebugAll,
    }

    public static class NetLogLevelExtensions
    {
        public static NetLogType ToNetLogType(this NetLogLevel level)
        {
            return level switch
            {
                NetLogLevel.Verbose => NetLogType.Verbose,
                NetLogLevel.DebugRequest => NetLogType.RequestHeader | NetLogType.RequestBody,
                NetLogLevel.DebugResponse => NetLogType.ResponseBody,
                NetLogLevel.DebugStream => NetLogType.ResponseBody | NetLogType.ResponseDelta,
                NetLogLevel.DebugAll => NetLogType.All,
                _ => NetLogType.None,
            };
        }
    }

    public class NetLogger : GNLogger
    {
        private NetLogType _enabledTypes = NetLogType.All;
        public NetLogger(string tag) : base(tag: tag)
        {
        }

        public void SetLogLevel(NetLogLevel level)
        {
            _enabledTypes = level.ToNetLogType();
        }

        public void SetLogLevel(NetLogType types)
        {
            _enabledTypes = types;
        }

        public void Verbose(object sender, string message)
        {
            if ((_enabledTypes & NetLogType.Verbose) == NetLogType.Verbose)
            {
                Info(sender, message);
            }
        }

        public void RequestHeader(object sender, string message)
        {
            if ((_enabledTypes & NetLogType.RequestHeader) == NetLogType.RequestHeader)
            {
                Info(sender, message);
            }
        }

        public void RequestBody(object sender, string message)
        {
            if ((_enabledTypes & NetLogType.RequestBody) == NetLogType.RequestBody)
            {
                Info(sender, message);
            }
        }

        public void ResponseBody(object sender, string message)
        {
            if ((_enabledTypes & NetLogType.ResponseBody) == NetLogType.ResponseBody)
            {
                Info(sender, message);
            }
        }

        public void Delta(object sender, string message)
        {
            if ((_enabledTypes & NetLogType.ResponseDelta) == NetLogType.ResponseDelta)
            {
                Info(sender, message);
            }
        }
    }
}