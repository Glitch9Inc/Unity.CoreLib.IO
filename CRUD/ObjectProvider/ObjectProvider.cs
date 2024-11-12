using Cysharp.Threading.Tasks;
using System;

namespace Glitch9.IO.RESTApi
{
    public abstract class ObjectProvider<TObject> : IObjectProvider<TObject>
        where TObject : RESTResponse
    {
        public EventHandler<TObject> OnCreate { get; set; }
        public EventHandler<TObject> OnRetrieve { get; set; }
        public EventHandler<TObject> OnUpdate { get; set; }
        public EventHandler<TObject[]> OnList { get; set; }
        public EventHandler<bool> OnDelete { get; set; }

        private readonly string _objectName;
        private readonly ILogger _logger;

        protected ObjectProvider(ILogger logger)
        {
            _objectName = typeof(TObject).Name;
            _logger = logger;
        }

        public async UniTask<IResult> CreateAsync(params object[] args)
        {
            try
            {
                TObject obj = await CreateInternalAsync(args);
                if (obj == null) return Result.Fail($"{_objectName} creation failed.");

                OnCreate?.Invoke(this, obj);
                return Result<TObject>.Success(obj);
            }
            catch (Exception e)
            {
                return Result.Fail($"{_objectName} creation failed. {e.Message}");
            }
        }

        public async UniTask<IResult> RetrieveAsync(string id, params object[] args)
        {
            try
            {
                TObject obj = await RetrieveInternalAsync(id, args);
                if (obj == null) return Result.Fail($"{_objectName}({id}) not found.");
                OnRetrieve?.Invoke(this, obj);
                return Result<TObject>.Success(obj);
            }
            catch (Exception e)
            {
                return Result.Fail($"{_objectName} retrieval failed. {e.Message}");
            }
        }

        public async UniTask<IResult> RetrieveOrCreateAsync(string id, params object[] args)
        {
            IResult result = await RetrieveAsync(id, args);
            if (result.IsSuccess) return result;
            _logger?.Warning($"{_objectName}({id}) retrieval failed. Creating new {_objectName}.");
            await UniTask.Delay(RESTApiV3.Config.MIN_INTERNAL_OPERATION_MILLIS);
            return await CreateAsync(args);
        }

        public async UniTask<IResult> UpdateAsync(string id, params object[] args)
        {
            try
            {
                TObject obj = await UpdateInternalAsync(id, args);
                if (obj == null) return Result.Fail($"{_objectName}({id}) update failed.");

                OnUpdate?.Invoke(this, obj);
                return Result<TObject>.Success(obj);
            }
            catch (Exception e)
            {
                return Result.Fail($"{_objectName}({id}) update failed. {e.Message}");
            }
        }

        public async UniTask<IResult> ListAsync(params object[] args)
        {
            try
            {
                TObject[] objs = await ListInternalAsync(args);
                if (objs == null || objs.Length == 0) return Result.Fail($"{_objectName} list is empty.");
                OnList?.Invoke(this, objs);
                return Result<TObject[]>.Success(objs);
            }
            catch (Exception e)
            {
                return Result.Fail($"{_objectName} list retrieval failed. {e.Message}");
            }
        }

        public async UniTask<IResult> DeleteAsync(string id, params object[] args)
        {
            try
            {
                bool deleted = await DeleteInternalAsync(id, args);
                if (!deleted)
                {
                    OnDelete?.Invoke(this, false);
                    return Result.Fail($"{_objectName}({id}) deletion failed.");
                }

                OnDelete?.Invoke(this, true);
                return Result<TObject>.Success(null);
            }
            catch (Exception e)
            {
                return Result.Fail($"{_objectName}({id}) deletion failed. {e.Message}");
            }
        }

        protected abstract UniTask<TObject> CreateInternalAsync(params object[] args);
        protected abstract UniTask<TObject> RetrieveInternalAsync(string id, params object[] args);
        protected abstract UniTask<TObject> UpdateInternalAsync(string id, params object[] args);
        protected abstract UniTask<TObject[]> ListInternalAsync(params object[] args);
        protected abstract UniTask<bool> DeleteInternalAsync(string id, params object[] args);
    }
}