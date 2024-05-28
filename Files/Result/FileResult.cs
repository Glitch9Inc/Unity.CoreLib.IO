
namespace Glitch9.IO.Files
{
    public class FileResult<TBaseFile> : Result
        where TBaseFile : BaseFile<TBaseFile>
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

        public FileResult(TBaseFile result)
        {
            Result = result;
            IsSuccess = true;
        }

        public FileResult(string failReason)
        {
            IsSuccess = false;
            FailReason = failReason;
        }

        public static FileResult<TBaseFile> Success(TBaseFile result)
        {
            return new FileResult<TBaseFile>(result);
        }

        public new static FileResult<TBaseFile> Fail(string failReason)
        {
            return new FileResult<TBaseFile>(failReason);
        }
    }
}