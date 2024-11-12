using Cysharp.Threading.Tasks;

namespace Glitch9.IO.RESTApi
{
    public abstract partial class CRUDClient<TSelf>
        where TSelf : CRUDClient<TSelf>
    {
        public class CRUD
        {
            public static async UniTask<TResponse> Create<TRequest, TResponse>(CRUDService<TSelf> service, TRequest req, params IPathParam[] pathParams)
                where TRequest : RESTRequest
                where TResponse : RESTResponse, new()
            {
                return await Create<TRequest, TResponse>(null, service, req, pathParams);
            }

            public static async UniTask<TResponse> Create<TResponse>(CRUDService<TSelf> service, RESTRequest<TResponse> req, params IPathParam[] pathParams)
                where TResponse : RESTResponse, new()
            {
                return await Create<RESTRequest<TResponse>, TResponse>(null, service, req, pathParams);
            }

            public static async UniTask<TResponse> Create<TRequest, TResponse>(string routeOverride, CRUDService<TSelf> service, TRequest req, params IPathParam[] pathParams)
                where TRequest : RESTRequest
                where TResponse : RESTResponse, new()
            {
                (TRequest req, IPathParam[] pathParams) tuple = AutoParamHelper.HandleAutoParams(service, req, pathParams);
                string endpoint = RouteBuilder.Build(CRUDMethod.Create, service, routeOverride, tuple.pathParams);
                return await service.Client.Create<TRequest, TResponse>(tuple.req, endpoint);
            }

            public static async UniTask<TQueryList> List<TRequest, TQueryList, TResponse>(CRUDService<TSelf> service, TRequest req, params IPathParam[] pathParams)
                where TRequest : RESTRequest
                where TQueryList : RESTQueryResponse<TResponse>, new()
                where TResponse : class, new()
            {
                return await List<TRequest, TQueryList, TResponse>(null, service, req, pathParams);
            }

            public static async UniTask<TQueryList> List<TRequest, TQueryList, TResponse>(string routeOverride, CRUDService<TSelf> service, TRequest req, params IPathParam[] pathParams)
                where TRequest : RESTRequest
                where TQueryList : RESTQueryResponse<TResponse>, new()
                where TResponse : class, new()
            {
                (TRequest req, IPathParam[] pathParams) tuple = AutoParamHelper.HandleAutoParams(service, req, pathParams);
               
                string endpoint = RouteBuilder.Build(CRUDMethod.List, service, routeOverride, tuple.pathParams);
                return await service.Client.List<TRequest, TQueryList, TResponse>(tuple.req, endpoint);
            }

            public static async UniTask<TResponse> Update<TRequest, TResponse>(CRUDService<TSelf> service, TRequest req, params IPathParam[] pathParams)
                where TRequest : RESTRequest
                where TResponse : RESTResponse, new()
            {
                return await Update<TRequest, TResponse>(null, service, req, pathParams);
            }

            public static async UniTask<TResponse> Update<TRequest, TResponse>(string routeOverride, CRUDService<TSelf> service, TRequest req, params IPathParam[] pathParams)
                where TRequest : RESTRequest
                where TResponse : RESTResponse, new()
            {
                (TRequest req, IPathParam[] pathParams) tuple = AutoParamHelper.HandleAutoParams(service, req, pathParams);
               
                string endpoint = RouteBuilder.Build(CRUDMethod.Update, service, routeOverride, tuple.pathParams);
                return await service.Client.Update<TRequest, TResponse>(tuple.req, endpoint);
            }

            public static async UniTask<TResponse> Get<TResponse>(CRUDService<TSelf> service, params IPathParam[] pathParams)
                where TResponse : RESTResponse, new()
            {
                return await Get<TResponse>(null, service, pathParams);
            }

            public static async UniTask<TResponse> Get<TResponse>(string routeOverride, CRUDService<TSelf> service, params IPathParam[] pathParams)
                where TResponse : RESTResponse, new()
            {
                RESTRequest req = RESTRequest.Empty();
                (RESTRequest req, IPathParam[] pathParams) tuple = AutoParamHelper.HandleAutoParams(service, req, pathParams);
               
                string endpoint = RouteBuilder.Build(CRUDMethod.Get, service, routeOverride, tuple.pathParams);
                return await service.Client.Get<TResponse>(tuple.req, endpoint);
            }

            public static async UniTask<TResponse> Retrieve<TResponse>(CRUDService<TSelf> service, params IPathParam[] pathParams)
                where TResponse : RESTResponse, new()
            {
                return await Get<TResponse>(null, service, pathParams);
            }

            public static async UniTask<TResponse> Retrieve<TResponse>(string routeOverride, CRUDService<TSelf> service, params IPathParam[] pathParams)
                where TResponse : RESTResponse, new()
            {
                return await Get<TResponse>(routeOverride, service, pathParams);
            }

            public static async UniTask<TResponse> Patch<TResponse>(CRUDService<TSelf> service, params IPathParam[] pathParams)
                where TResponse : RESTResponse, new()
            {
                return await Patch<TResponse>(null, service, pathParams);
            }

            public static async UniTask<TResponse> Patch<TResponse>(string routeOverride, CRUDService<TSelf> service, params IPathParam[] pathParams)
                where TResponse : RESTResponse, new()
            {
                RESTRequest req = RESTRequest.Empty();
                (RESTRequest req, IPathParam[] pathParams) tuple = AutoParamHelper.HandleAutoParams(service, req, pathParams);
               
                string endpoint = RouteBuilder.Build(CRUDMethod.Patch, service, routeOverride, tuple.pathParams);
                return await service.Client.Patch<TResponse>(tuple.req, endpoint);
            }

            public static async UniTask<bool> Delete<TResponse>(CRUDService<TSelf> service, params IPathParam[] pathParams)
                where TResponse : RESTResponse, new()
            {
                return await Delete<TResponse>(null, service, pathParams);
            }

            public static async UniTask<bool> Delete<TResponse>(string routeOverride, CRUDService<TSelf> service, params IPathParam[] pathParams)
                where TResponse : RESTResponse, new()
            {
                RESTRequest req = RESTRequest.Empty();
                (RESTRequest req, IPathParam[] pathParams) tuple = AutoParamHelper.HandleAutoParams(service, req, pathParams);
               
                string endpoint = RouteBuilder.Build(CRUDMethod.Delete, service, routeOverride, tuple.pathParams);
                return await service.Client.Delete<TResponse>(tuple.req, endpoint);
            }

            public static async UniTask<TResponse> Cancel<TResponse>(CRUDService<TSelf> service, params IPathParam[] pathParams)
                where TResponse : RESTResponse, new()
            {
                return await Cancel<TResponse>(null, service, pathParams);
            }

            public static async UniTask<TResponse> Cancel<TResponse>(string routeOverride, CRUDService<TSelf> service, params IPathParam[] pathParams)
                where TResponse : RESTResponse, new()
            {
                RESTRequest req = RESTRequest.Empty();
                (RESTRequest req, IPathParam[] pathParams) tuple = AutoParamHelper.HandleAutoParams(service, req, pathParams);
               
                string endpoint = RouteBuilder.Build(CRUDMethod.Cancel, service, routeOverride, tuple.pathParams);
                return await service.Client.Cancel<TResponse>(tuple.req, endpoint);
            }

            public static async UniTask<TResponse> Query<TRequest, TResponse>(CRUDService<TSelf> service, TRequest req, params IPathParam[] pathParams)
                where TRequest : RESTRequest
                where TResponse : RESTResponse, new()
            {
                return await Query<TRequest, TResponse>(null, service, req, pathParams);
            }

            public static async UniTask<TResponse> Query<TRequest, TResponse>(string routeOverride, CRUDService<TSelf> service, TRequest req, params IPathParam[] pathParams)
                where TRequest : RESTRequest
                where TResponse : RESTResponse, new()
            {
                (TRequest req, IPathParam[] pathParams) tuple = AutoParamHelper.HandleAutoParams(service, req, pathParams);
               
                string endpoint = RouteBuilder.Build(CRUDMethod.Query, service, routeOverride, tuple.pathParams);
                return await service.Client.Query<TRequest, TResponse>(tuple.req, endpoint);
            }

            public static async UniTask<bool> CreateAndGetEmptyResponse<TRequest>(CRUDService<TSelf> service, TRequest req, params IPathParam[] pathParams)
                where TRequest : RESTRequest
            {
                return await CreateAndGetEmptyResponse<TRequest>(null, service, req, pathParams);
            }

            public static async UniTask<bool> CreateAndGetEmptyResponse<TRequest>(string routeOverride, CRUDService<TSelf> service, TRequest req, params IPathParam[] pathParams)
                where TRequest : RESTRequest
            {
                (TRequest req, IPathParam[] pathParams) tuple = AutoParamHelper.HandleAutoParams(service, req, pathParams);
               
                string endpoint = RouteBuilder.Build(CRUDMethod.Create, service, routeOverride, tuple.pathParams);
                return await service.Client.CreateAndGetEmptyResponse(tuple.req, endpoint);
            }
        }
    }
}