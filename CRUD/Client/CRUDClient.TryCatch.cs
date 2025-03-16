using Cysharp.Threading.Tasks;
using System;


namespace Glitch9.IO.RESTApi
{
    public abstract partial class CRUDClient<TSelf>
        where TSelf : CRUDClient<TSelf>
    {
        public class TryCatch
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
                try
                {
                    return await CRUD.Create<TRequest, TResponse>(routeOverride, service, req, pathParams);
                }
                catch (Exception e)
                {
                    service.Client.HandleException(e);
                    return null;
                }
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
                try
                {
                    return await CRUD.List<TRequest, TQueryList, TResponse>(routeOverride, service, req, pathParams);
                }
                catch (Exception e)
                {
                    service.Client.HandleException(e);
                    return null;
                }
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
                try
                {
                    return await CRUD.Update<TRequest, TResponse>(service, req, pathParams);
                }
                catch (Exception e)
                {
                    service.Client.HandleException(e);
                    return null;
                }
            }

            public static async UniTask<TResponse> Get<TResponse>(CRUDService<TSelf> service, params IPathParam[] pathParams)
                where TResponse : RESTResponse, new()
            {
                return await Get<TResponse>(null, service, pathParams);
            }
            
            public static async UniTask<TResponse> Get<TResponse>(string routeOverride, CRUDService<TSelf> service, params IPathParam[] pathParams)
                where TResponse : RESTResponse, new()
            {
                try
                {
                    return await CRUD.Get<TResponse>(routeOverride, service, pathParams);
                }
                catch (Exception e)
                {
                    service.Client.HandleException(e);
                    return null;
                }
            }
            
            public static async UniTask<TResponse> Retrieve<TResponse>(CRUDService<TSelf> service, params IPathParam[] pathParams)
                where TResponse : RESTResponse, new()
            {
                return await Retrieve<TResponse>(null, service, pathParams);
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
                try
                {
                    return await CRUD.Patch<TResponse>(routeOverride, service, pathParams);
                }
                catch (Exception e)
                {
                    service.Client.HandleException(e);
                    return null;
                }
            }

            public static async UniTask<bool> Delete<TResponse>(CRUDService<TSelf> service, params IPathParam[] pathParams)
                where TResponse : RESTResponse, new()
            {
                return await Delete<TResponse>(null, service, pathParams);
            }

            public static async UniTask<bool> Delete<TResponse>(string routeOverride, CRUDService<TSelf> service, params IPathParam[] pathParams)
                where TResponse : RESTResponse, new()
            {
                try
                {
                    return await CRUD.Delete<TResponse>(service, pathParams);
                }
                catch (Exception e)
                {
                    service.Client.HandleException(e);
                    return false;
                }
            }

            public static async UniTask<TResponse> Cancel<TResponse>(CRUDService<TSelf> service, params IPathParam[] pathParams)
                where TResponse : RESTResponse, new()
            {
                return await Cancel<TResponse>(null, service, pathParams);
            }

            public static async UniTask<TResponse> Cancel<TResponse>(string routeOverride, CRUDService<TSelf> service, params IPathParam[] pathParams)
                where TResponse : RESTResponse, new()
            {
                try
                {
                    return await CRUD.Cancel<TResponse>(service, pathParams);
                }
                catch (Exception e)
                {
                    service.Client.HandleException(e);
                    return null;
                }
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
                try
                {
                    return await CRUD.Query<TRequest, TResponse>(routeOverride, service, req, pathParams);
                }
                catch (Exception e)
                {
                    service.Client.HandleException(e);
                    return null;
                }
            }

            public static async UniTask<bool> CreateAndGetEmptyResponse<TRequest>(CRUDService<TSelf> service, TRequest req, params IPathParam[] pathParams)
                where TRequest : RESTRequest
            {
                return await CreateAndGetEmptyResponse(null, service, req, pathParams);
            }

            public static async UniTask<bool> CreateAndGetEmptyResponse<TRequest>(string routeOverride, CRUDService<TSelf> service, TRequest req, params IPathParam[] pathParams)
                where TRequest : RESTRequest
            {
                try
                {
                    return await CRUD.CreateAndGetEmptyResponse(routeOverride, service, req, pathParams);
                }
                catch (Exception e)
                {
                    service.Client.HandleException(e);
                    return false;
                }
            }
        }
    }
}