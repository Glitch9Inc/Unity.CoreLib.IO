using System;

namespace Glitch9.IO.Files
{
    public enum FileNamingRule
    {
        DateTime,
        GUID,
    }
    public class FileNameUtils
    {
        public static string GetUniqueFileName(ContentType fileContentType, FileNamingRule namingRule)
        {
            return GetUniqueFileName(null, fileContentType, namingRule);
        }

        public static string GetUniqueFileName(string tag, ContentType fileContentType, FileNamingRule namingRule)
        {
            tag ??= fileContentType.ToString();
            
            string fileName = string.Empty;
            switch (namingRule)
            {
                case FileNamingRule.DateTime:
                    fileName = $"{tag}_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}";
                    break;
                case FileNamingRule.GUID:
                    fileName = $"{tag}_{Guid.NewGuid()}";
                    break;
            }

            string extension = ContentTypeUtils.GetFileExtension(fileContentType);
            return $"{fileName}{extension}";
        }
    }
}