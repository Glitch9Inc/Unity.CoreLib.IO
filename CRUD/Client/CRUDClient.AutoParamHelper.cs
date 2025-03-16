using System.Collections.Generic;

namespace Glitch9.IO.RESTApi
{
    public abstract partial class CRUDClient<TSelf> where TSelf : CRUDClient<TSelf>
    {
        public static class AutoParamHelper
        {
            public static (TRequest req, IPathParam[] pathParams) HandleAutoParams<TRequest>
                (CRUDService<TSelf> service, TRequest req, IPathParam[] pathParams) where TRequest : RESTRequest
            {

                ThrowIf.ArgumentIsNull(
                    (service, $"Service"),
                    (service.Client, $"Client-{typeof(TSelf).Name}"),
                    (req, typeof(TRequest).Name));

                string apiName = service.Client.Name;
                ThrowIf.StringIsNullOrEmpty(apiName, "API Name");
                List<IPathParam> newPathParams = new(pathParams);

                if (service.Client._autoApiKey != AutoParam.Unset)
                {
                    string apiKey = service.Client._apiKeyGetter.Invoke();
                    if (string.IsNullOrEmpty(apiKey)) throw new NoAPIKeyException(apiName);

                    if (service.Client._autoApiKey == AutoParam.Header)
                    {
                        req.AddHeader(RESTHeader.AuthHeader(apiKey));
                    }
                    else if (service.Client._autoApiKey == AutoParam.Query)
                    {
                        string key = service.Client._apiKeyQueryKey;
                        if (string.IsNullOrEmpty(key)) throw new NoAPIKeyQueryKeyException(apiName);
                        newPathParams.Add(PathParam.Query(key, apiKey));
                    }
                }

                bool betaVersionParamSet = false;

                if (service.Client._autoBetaParam != AutoParam.Unset)
                {
                    if (service.Client._autoBetaParam == AutoParam.Header)
                    {
                        if (service.BetaHeaders.IsNullOrEmpty())
                        {
                            foreach (RESTHeader header in service.BetaHeaders)
                            {
                                req.AddHeader(header);
                            }
                        }
                        else
                        {
                            if (service.Client._betaHeader == null) throw new NoBetaHeaderException(apiName);
                            req.AddHeader(service.Client._betaHeader.Value);
                        }
                    }
                    else if (service.Client._autoBetaParam == AutoParam.Path)
                    {
                        if (string.IsNullOrEmpty(service.Client.BetaVersion)) throw new NoBetaVersionException(apiName);
                        newPathParams.Add(PathParam.Version(service.Client.BetaVersion));
                        betaVersionParamSet = true;
                    }
                }

                if (!betaVersionParamSet && service.Client._autoVersionParam == AutoParam.Path)
                {
                    if (string.IsNullOrEmpty(service.Client.Version)) throw new NoVersionException(apiName);
                    newPathParams.Add(PathParam.Version(service.Client.Version));
                }

                if (!service.Client._additionalHeaders.IsNullOrEmpty())
                {
                    foreach (RESTHeader header in service.Client._additionalHeaders)
                    {
                        req.AddHeader(header);
                    }
                }

                return (req, newPathParams.ToArray());
            }
        }
    }
}