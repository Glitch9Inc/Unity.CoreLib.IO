using System.Collections.Generic;

namespace Glitch9.IO.RESTApi
{
    public abstract class CRUDService<TClient> where TClient : CRUDClient<TClient>
    {
        public TClient Client { get; }
        public Dictionary<CRUDMethod, string> Routes { get; private set; }
        public bool IsBeta { get; }
        public RESTHeader[] BetaHeaders { get; private set; }

        protected CRUDService(TClient client, params RESTHeader[] betaHeaders)
        {
            Client = client;
            BetaHeaders = betaHeaders;
            IsBeta = !betaHeaders.IsNullOrEmpty();
            InitializeRoutes();
        }

        protected CRUDService(TClient client, bool isBeta)
        {
            Client = client;
            IsBeta = isBeta;
            InitializeRoutes();
        }

        private void InitializeRoutes()
        {
            Routes = CreateRoutes();
        }

        protected abstract Dictionary<CRUDMethod, string> CreateRoutes();

        public string GetRoute(CRUDMethod method)
        {
            return Routes.GetValueOrDefault(method);
        }
    }
}