using Cysharp.Threading.Tasks;

namespace Glitch9.IO.Network
{
    public interface ISyncStorage
    {
        UniTask<object> GetDataAsync(string fieldName);
        void SetDataAsync(string fieldName, object value);
    }
}