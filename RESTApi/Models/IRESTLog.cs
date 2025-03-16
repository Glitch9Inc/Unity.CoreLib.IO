namespace Glitch9.IO.RESTApi
{
    public interface IRESTLog
    {
        bool IsEmpty { get; }
        string Sender { get; }
        UnixTime CreatedAt { get; }
        string InputText { get; }
        Metadata Metadata { get; }
    }
}