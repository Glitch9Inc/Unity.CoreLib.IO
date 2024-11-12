using System;
using System.Collections.Generic;
using System.Linq;

namespace Glitch9.IO.RESTApi
{
    /// <summary>
    /// Provides methods to manage Firestore document and collection references dynamically.
    /// </summary>
    public static class RouteBuilder
    {
        public const string VER = "{ver}";
        private static string AddLastSlash(string text) => text.EndsWith('/') ? text : text + '/';
        private static string RemoveFirstSlash(string text) => text.StartsWith('/') ? text.Substring(1) : text;

        public static string Build<T>(CRUDMethod crudMethod, CRUDService<T> service, string routeOverride, params IPathParam[] pathParams) where T : CRUDClient<T>
        {
            // basePath Example: https://generativelanguage.googleapis.com/v1beta/corpora/{0}/documents/{1}
            // basePath without id in the end: https://generativelanguage.googleapis.com/v1beta/corpora/{0}/documents

            string baseUrl = service.Client.BaseUrl;
            string route = string.IsNullOrEmpty(routeOverride) ? service.GetRoute(crudMethod) : routeOverride;

            if (string.IsNullOrEmpty(route))
            {
                throw new ArgumentException($"Route not defined for method {crudMethod}.");
            }

            ILogger logger = service.Client.Logger;

            List<QueryParam> queryParams = new();
            bool idsAlreadyDefined = false;
            bool methodAlreadyDefined = false;

            baseUrl = AddLastSlash(baseUrl);
            route = RemoveFirstSlash(route);

            if (!pathParams.IsNullOrEmpty())
            {
                foreach (IPathParam pathParam in pathParams)
                {
                    if (pathParam == null) continue;
                    if (!pathParam.IsValid())
                    {
                        logger.Warning($"Invalid path parameter {pathParam} for endpoint {route}. Ignoring.");
                        continue;
                    }

                    if (pathParam is IdParam idParam)
                    {
                        if (idsAlreadyDefined)
                        {
                            logger.Warning($"Ids already defined for endpoint {route}. Ignoring additional ids {string.Join(", ", idParam.ids)}.");
                            continue;
                        }

                        for (int i = 0; i < idParam.ids.Length; i++)
                        {
                            route = route.Replace($"{{{i}}}", idParam.ids[i]);
                        }

                        idsAlreadyDefined = true;
                        continue;
                    }

                    if (pathParam is MethodParam methodParam)
                    {
                        if (methodAlreadyDefined)
                        {
                            logger.Warning($"Method parameter already defined for endpoint {route}. Ignoring additional method parameter {methodParam.method}.");
                            continue;
                        }

                        route += methodParam;
                        methodAlreadyDefined = true;
                        continue;
                    }

                    if (pathParam is QueryParam queryParam)
                    {
                        queryParams.Add(queryParam);
                        continue;
                    }

                    if (pathParam is ChildParam childParam)
                    {
                        route = AddLastSlash(route) + childParam.childPath;
                        continue;
                    }

                    if (pathParam is VersionParam versionParam)
                    {
                        route = route.Replace(VER, versionParam.version);
                    }
                }
            }

            if (route.Contains(VER))
            {
                throw new ArgumentException($"Version parameter not defined for endpoint {route}.");
            }

            if (queryParams.Count > 0)
            {
                string query = string.Join("&", queryParams.Select(queryParam => $"{queryParam.key}={Uri.EscapeDataString(queryParam.value)}"));
                route = $"{route}?{query}";
            }


            string result = baseUrl + route;
            //Debug.LogError("Endpoint Result: " + result);

            return result;
        }
    }
}
