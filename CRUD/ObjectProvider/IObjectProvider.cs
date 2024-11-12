using Cysharp.Threading.Tasks;
using System;

namespace Glitch9.IO.RESTApi
{
    public interface IObjectProvider<TObject> 
        where TObject : RESTResponse
    {
        EventHandler<TObject> OnCreate { get; set; }
        EventHandler<TObject> OnRetrieve { get; set; }
        EventHandler<TObject[]> OnList { get; set; }
        EventHandler<bool> OnDelete { get; set; }

        UniTask<IResult> CreateAsync(params object[] args);
        UniTask<IResult> RetrieveAsync(string id, params object[] args);
        UniTask<IResult> RetrieveOrCreateAsync(string id, params object[] args);
        UniTask<IResult> ListAsync(params object[] args);
        UniTask<IResult> DeleteAsync(string id, params object[] args);
    }
}