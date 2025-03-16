
namespace Glitch9.IO.Files
{
    public class UnityFileResult<TBaseFile> : Result
        where TBaseFile : UnityFileBase<TBaseFile>
    {
        /// <summary>
        /// The file is being loaded.
        /// </summary>
        public bool IsLoading { get; set; } = false;

        /// <summary>
        /// The file has been loaded.
        /// </summary>
        public bool IsLoaded { get; set; } = false;

        public TBaseFile Result { get; protected set; }

        public UnityFileResult(TBaseFile result)
        {
            Result = result;
            IsSuccess = true;
        }

        public UnityFileResult(string failReason)
        {
            IsSuccess = false;
            FailReason = failReason;
        }

        public static UnityFileResult<TBaseFile> Success(TBaseFile result)
        {
            return new UnityFileResult<TBaseFile>(result);
        }

        public new static UnityFileResult<TBaseFile> Fail(string failReason)
        {
            return new UnityFileResult<TBaseFile>(failReason);
        }
    }
}