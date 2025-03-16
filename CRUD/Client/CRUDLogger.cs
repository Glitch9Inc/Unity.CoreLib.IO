using System.Collections.Generic;
using System.Reflection;

namespace Glitch9.IO.RESTApi
{
    public class CRUDLogger : ILogger
    {
        internal static class CRUDOps
        {
            internal const string UNKNOWN = "Unknown CRUD operation executing";
            internal const string CREATE = "Creating";
            internal const string UPDATE = "Updating";
            internal const string PATCH = "Patching";
            internal const string GET = "Getting";
            internal const string RETRIEVE = "Retrieving";
            internal const string LIST = "Getting the list of";
            internal const string QUERY = "Querying";
            internal const string DELETE = "Deleting";
            internal const string CANCEL = "Cancelling";
        }

        private static readonly Dictionary<CRUDMethod, string> _messages = new()
        {
            {CRUDMethod.Create, CRUDOps.CREATE},
            {CRUDMethod.Update, CRUDOps.UPDATE},
            {CRUDMethod.Patch, CRUDOps.PATCH},
            {CRUDMethod.Get, CRUDOps.GET},
            {CRUDMethod.Retrieve, CRUDOps.RETRIEVE},
            {CRUDMethod.List, CRUDOps.LIST},
            {CRUDMethod.Query, CRUDOps.QUERY},
            {CRUDMethod.Delete, CRUDOps.DELETE},
            {CRUDMethod.Cancel, CRUDOps.CANCEL}
        };

        private readonly ILogger _logger;
        private readonly string _tag;

        public CRUDLogger(string tag, ILogger logger = null)
        {
            _tag = tag;
            _logger = logger ?? new GNLogger(tag);
        }

        public void Info(CRUDMethod method, MemberInfo type)
        {
            if (_messages.TryGetValue(method, out string message))
            {
                LogMessage(message, type);
            }

            LogMessage(CRUDOps.UNKNOWN, type);
        }

        public void Create(MemberInfo type)
        {
            LogMessage(CRUDOps.CREATE, type);
        }

        public void Update(MemberInfo type)
        {
            LogMessage(CRUDOps.UPDATE, type);
        }

        public void Patch(MemberInfo type)
        {
            LogMessage(CRUDOps.PATCH, type);
        }

        public void Get(MemberInfo type)
        {
            LogMessage(CRUDOps.GET, type);
        }

        public void Retrieve(MemberInfo type)
        {
            LogMessage(CRUDOps.RETRIEVE, type);
        }

        public void List(MemberInfo type)
        {
            LogMessage(CRUDOps.LIST, type);
        }

        public void Query(MemberInfo type)
        {
            LogMessage(CRUDOps.QUERY, type);
        }

        public void Delete(MemberInfo type)
        {
            LogMessage(CRUDOps.DELETE, type);
        }

        public void Cancel(MemberInfo type)
        {
            LogMessage(CRUDOps.CANCEL, type);
        }

        

        private void LogMessage(string action, MemberInfo type)
        {
            Info($"{action} {type.Name}.");
        }

        public void Info(string message)
        {
            Info(_tag, message);
        }

        public void Warning(string message)
        {
            Warning(_tag, message);
        }

        public void Error(string message)
        {
            Error(_tag, message);
        }

        public void Info(object sender, string message)
        {
            _logger.Info(sender, message);
        }

        public void Warning(object sender, string message)
        {
            _logger.Warning(sender, message);
        }

        public void Error(object sender, string message)
        {
            _logger.Error(sender, message);
        }
    }
}