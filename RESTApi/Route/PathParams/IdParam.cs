namespace Glitch9.IO.RESTApi
{
    public class IdParam : IPathParam
    {
        public string[] ids;

        public IdParam(params string[] ids)
        {
            this.ids = ids;
        }

        public bool IsValid()
        {
            return !ids.IsNullOrEmpty();
        }
    }
}