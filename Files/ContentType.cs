using Glitch9.IO.RESTApi;
using System.IO;

namespace Glitch9.IO.Files
{
    public enum ContentType
    {
        Unknown,

        /// <summary>
        /// Represents JSON formatted text (.json).
        /// </summary>
        Json,

        /// <summary>
        /// Represents JSON Lines formatted text (.jsonl).
        /// </summary>
        Jsonl,

        /// <summary>
        /// Represents XML formatted text (.xml).
        /// </summary>
        Xml,

        /// <summary>
        /// Represents form data encoded as URL parameters.
        /// </summary>
        WWWForm,

        /// <summary>
        /// Represents multipart form data, allowing multiple values and file uploads.
        /// </summary>
        MultipartForm,

        /// <summary>
        /// Represents plain text data (.txt).
        /// </summary>
        PlainText,

        /// <summary>
        /// Represents HTML formatted text (.html).
        /// </summary>
        HTML,

        /// <summary>
        /// Represents comma-separated values (.csv).
        /// </summary>
        CSV,

        /// <summary>
        /// Represents explicitly formatted XML text (.xml).
        /// </summary>
        XmlText,

        /// <summary>
        /// Represents raw binary data (no specific file extension).
        /// </summary>
        OctetStream,

        /// <summary>
        /// Represents Adobe Portable Document Format (.pdf).
        /// </summary>
        PDF,

        /// <summary>
        /// Represents MPEG video format (.mpeg).
        /// </summary>
        MPEG,

        /// <summary>
        /// Represents Waveform Audio File Format (.wav).
        /// </summary>
        WAV,

        /// <summary>
        /// Represents Base64 encoded image data.
        /// </summary>
        Base64Image,

        /// <summary>
        /// Represents JPEG image format (.jpeg).
        /// </summary>
        JPEG,

        /// <summary>
        /// Represents JPEG image format (.jpg).
        /// </summary>
        JPG,

        /// <summary>
        /// Represents Portable Network Graphics format (.png).
        /// </summary>
        PNG,

        /// <summary>
        /// Represents Graphics Interchange Format (.gif).
        /// </summary>
        GIF,

        /// <summary>
        /// Represents MPEG-4 video format (.mp4).
        /// </summary>
        MP4,

        /// <summary>
        /// Represents Audio Video Interleave format (.avi).
        /// </summary>
        AVI,

        /// <summary>
        /// Represents C source code (.c).
        /// </summary>
        C,

        /// <summary>
        /// Represents C# source code (.cs).
        /// </summary>
        CSharp,

        /// <summary>
        /// Represents C++ source code (.cpp).
        /// </summary>
        CPP,

        /// <summary>
        /// Represents a Microsoft Word binary document (.doc).
        /// </summary>
        MsDoc,

        /// <summary>
        /// Represents a Microsoft Word document in XML format (.docx).
        /// </summary>
        MsDocXML,

        /// <summary>
        /// Represents a Microsoft PowerPoint Presentation (.pptx).
        /// </summary>
        MsPowerPointPresentation,

        /// <summary>
        /// Represents a Microsoft Excel Spreadsheet in XML format (.xlsx).
        /// </summary>
        MsExcelXML,

        /// <summary>
        /// Represents Java source code (.java).
        /// </summary>
        Java,

        /// <summary>
        /// Represents Markdown formatted text (.md).
        /// </summary>
        Markdown,

        /// <summary>
        /// Represents PHP source code (PHP: Hypertext Preprocessor) (.php).
        /// </summary>
        HypertextPreprocessor,

        /// <summary>
        /// Represents Python source code (.py).
        /// </summary>
        Python,

        /// <summary>
        /// Represents Python script files (.py).
        /// </summary>
        PythonScript,

        /// <summary>
        /// Represents Ruby source code (.rb).
        /// </summary>
        Ruby,

        /// <summary>
        /// Represents TeX typesetting source (.tex).
        /// </summary>
        TeX,

        /// <summary>
        /// Represents Cascading Style Sheets (.css).
        /// </summary>
        CascadingStyleSheets,

        /// <summary>
        /// Represents JavaScript code (.js).
        /// </summary>
        JavaScript,

        /// <summary>
        /// Represents Shell script (.sh).
        /// </summary>
        ShellScript,

        /// <summary>
        /// Represents TypeScript source code (.ts).
        /// </summary>
        TypeScript,

        /// <summary>
        /// Represents a Tape Archive file, used for file archiving (.tar).
        /// </summary>
        TapeArchive,

        /// <summary>
        /// Represents a ZIP archive file, used for compression and archiving (.zip).
        /// </summary>
        ZIP,
    }

    public static class ContentTypeUtils
    {
        public static DataTransferMode ToDataTransferMode(this ContentType contentType)
        {
            return contentType switch
            {
                ContentType.Json => DataTransferMode.Text,
                ContentType.Jsonl => DataTransferMode.Text,
                ContentType.Xml => DataTransferMode.Text,
                ContentType.WWWForm => DataTransferMode.Text,
                ContentType.MultipartForm => DataTransferMode.Text,
                ContentType.PlainText => DataTransferMode.Text,
                ContentType.HTML => DataTransferMode.Text,
                ContentType.CSV => DataTransferMode.Text,
                ContentType.XmlText => DataTransferMode.Text,
                ContentType.OctetStream => DataTransferMode.Binary,
                ContentType.PDF => DataTransferMode.Binary,
                ContentType.MPEG => DataTransferMode.Binary,
                ContentType.WAV => DataTransferMode.Binary,
                ContentType.Base64Image => DataTransferMode.Binary,
                ContentType.JPEG => DataTransferMode.Binary,
                ContentType.JPG => DataTransferMode.Binary,
                ContentType.PNG => DataTransferMode.Binary,
                ContentType.GIF => DataTransferMode.Binary,
                ContentType.MP4 => DataTransferMode.Binary,
                ContentType.AVI => DataTransferMode.Binary,
                ContentType.C => DataTransferMode.Text,
                ContentType.CSharp => DataTransferMode.Text,
                ContentType.CPP => DataTransferMode.Text,
                ContentType.MsDoc => DataTransferMode.Binary,
                ContentType.MsDocXML => DataTransferMode.Binary,
                ContentType.MsPowerPointPresentation => DataTransferMode.Binary,
                ContentType.MsExcelXML => DataTransferMode.Binary,
                ContentType.Java => DataTransferMode.Text,
                ContentType.Markdown => DataTransferMode.Text,
                ContentType.HypertextPreprocessor => DataTransferMode.Text,
                ContentType.Python => DataTransferMode.Text,
                ContentType.PythonScript => DataTransferMode.Text,
                ContentType.Ruby => DataTransferMode.Text,
                ContentType.TeX => DataTransferMode.Text,
                ContentType.CascadingStyleSheets => DataTransferMode.Text,
                ContentType.JavaScript => DataTransferMode.Text,
                ContentType.ShellScript => DataTransferMode.Text,
                ContentType.TypeScript => DataTransferMode.Text,
                ContentType.TapeArchive => DataTransferMode.Binary,
                ContentType.ZIP => DataTransferMode.Binary,
                _ => DataTransferMode.Text,
            };
        }

        public static string ToMIME(this ContentType contentType)
        {
            return contentType switch
            {
                // 'application/json' - Commonly used in web APIs to exchange data that is structured in a compact, easy-to-parse format.
                ContentType.Json => "application/json",
                ContentType.Jsonl => "application/jsonl",

                // 'application/xml' or 'text/xml'
                ContentType.Xml => "application/xml",
                ContentType.XmlText => "Text/xml",

                // Form Requests - Form requests are specifically used when you are submitting HTML form data to a server.
                ContentType.WWWForm => "application/x-www-form-urlencoded",
                ContentType.MultipartForm => "multipart/form-data",

                // Text-Based Types (For API interactions, logging, configurations, etc.)
                ContentType.PlainText => "Text/plain",
                ContentType.HTML => "Text/html",
                ContentType.CSV => "Text/csv",
                ContentType.C => "text/x-c",
                ContentType.CSharp => "text/x-csharp",
                ContentType.CPP => "text/x-c++",
                ContentType.MsDoc => "application/msword",
                ContentType.MsDocXML => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ContentType.MsPowerPointPresentation => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
                ContentType.MsExcelXML => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ContentType.Java => "text/x-java",
                ContentType.Markdown => "text/markdown",
                ContentType.HypertextPreprocessor => "text/x-php",
                ContentType.Python => "text/x-python",
                ContentType.PythonScript => "text/x-script.python",
                ContentType.Ruby => "text/x-ruby",
                ContentType.TeX => "text/x-tex",
                ContentType.CascadingStyleSheets => "text/css",
                ContentType.JavaScript => "text/javascript",
                ContentType.ShellScript => "application/x-sh",
                ContentType.TypeScript => "application/typescript",

                // Binary Types (For transmitting files, multimedia content, etc.)
                ContentType.OctetStream => "application/octet-stream",
                ContentType.PDF => "application/pdf",
                ContentType.MPEG => "audio/mpeg",
                ContentType.WAV => "audio/wav",
                ContentType.JPEG => "image/jpeg",
                ContentType.PNG => "image/png",
                ContentType.GIF => "image/gif",
                ContentType.MP4 => "video/mp4",
                ContentType.AVI => "video/x-msvideo",

                // Archive Types (For compressed files and collections of files)
                ContentType.TapeArchive => "application/x-tar",
                ContentType.ZIP => "application/zip",

                _ => "application/json"
            };
        }

        public static RESTHeader GetHeader(this ContentType contentType)
        {
            return new RESTHeader("Content-Type", contentType.ToMIME());
        }

        public static ContentType Parse(string contentTypeAsString)
        {
            return contentTypeAsString switch
            {
                "application/json" => ContentType.Json,
                "application/jsonl" => ContentType.Jsonl,
                "application/xml" => ContentType.Xml,
                "text/xml" => ContentType.XmlText,
                "application/x-www-form-urlencoded" => ContentType.WWWForm,
                "multipart/form-data" => ContentType.MultipartForm,
                "text/plain" => ContentType.PlainText,
                "text/html" => ContentType.HTML,
                "text/csv" => ContentType.CSV,
                "text/x-c" => ContentType.C,
                "text/x-csharp" => ContentType.CSharp,
                "text/x-c++" => ContentType.CPP,
                "application/msword" => ContentType.MsDoc,
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document" => ContentType.MsDocXML,
                "application/vnd.openxmlformats-officedocument.presentationml.presentation" => ContentType.MsPowerPointPresentation,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" => ContentType.MsExcelXML,
                "text/x-java" => ContentType.Java,
                "text/markdown" => ContentType.Markdown,
                "text/x-php" => ContentType.HypertextPreprocessor,
                "text/x-python" => ContentType.Python,
                "text/x-script.python" => ContentType.PythonScript,
                "text/x-ruby" => ContentType.Ruby,
                "text/x-tex" => ContentType.TeX,
                "text/css" => ContentType.CascadingStyleSheets,
                "text/javascript" => ContentType.JavaScript,
                "application/x-sh" => ContentType.ShellScript,
                "application/typescript" => ContentType.TypeScript,
                "application/octet-stream" => ContentType.OctetStream,
                "application/pdf" => ContentType.PDF,
                "audio/mpeg" => ContentType.MPEG,
                "audio/wav" => ContentType.WAV,
                "image/jpeg" => ContentType.JPEG,
                "image/png" => ContentType.PNG,
                "image/gif" => ContentType.GIF,
                "video/mp4" => ContentType.MP4,
                "video/x-msvideo" => ContentType.AVI,
                "application/x-tar" => ContentType.TapeArchive,
                "application/zip" => ContentType.ZIP,
                _ => ContentType.Json
            };
        }

        public static ContentType ParseFileExtension(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return ContentType.Unknown;

            string extension = Path.GetExtension(filePath).ToLower();

            return extension switch
            {
                ".json" => ContentType.Json,
                ".jsonl" => ContentType.Jsonl,
                ".xml" => ContentType.Xml,
                ".txt" => ContentType.PlainText,
                ".html" => ContentType.HTML,
                ".csv" => ContentType.CSV,
                ".c" => ContentType.C,
                ".cs" => ContentType.CSharp,
                ".cpp" => ContentType.CPP,
                ".doc" => ContentType.MsDoc,
                ".docx" => ContentType.MsDocXML,
                ".ppt" => ContentType.MsPowerPointPresentation,
                ".pptx" => ContentType.MsPowerPointPresentation,
                ".xls" => ContentType.MsExcelXML,
                ".xlsx" => ContentType.MsExcelXML,
                ".java" => ContentType.Java,
                ".md" => ContentType.Markdown,
                ".php" => ContentType.HypertextPreprocessor,
                ".py" => ContentType.Python,
                ".rb" => ContentType.Ruby,
                ".tex" => ContentType.TeX,
                ".css" => ContentType.CascadingStyleSheets,
                ".js" => ContentType.JavaScript,
                ".sh" => ContentType.ShellScript,
                ".ts" => ContentType.TypeScript,
                ".bin" => ContentType.OctetStream,
                ".pdf" => ContentType.PDF,
                ".mp3" => ContentType.MPEG,
                ".wav" => ContentType.WAV,
                ".jpeg" => ContentType.JPEG,
                ".jpg" => ContentType.JPEG,
                ".png" => ContentType.PNG,
                ".gif" => ContentType.GIF,
                ".mp4" => ContentType.MP4,
                ".avi" => ContentType.AVI,
                ".tar" => ContentType.TapeArchive,
                ".zip" => ContentType.ZIP,
                _ => ContentType.Unknown
            };
        }

        public static string GetFileExtension(ContentType contentType)
        {
            return contentType switch
            {
                ContentType.Json => ".json",
                ContentType.Jsonl => ".jsonl",
                ContentType.Xml => ".xml",
                ContentType.XmlText => ".xml",
                ContentType.WWWForm => ".form",
                ContentType.MultipartForm => ".form",
                ContentType.PlainText => ".txt",
                ContentType.HTML => ".html",
                ContentType.CSV => ".csv",
                ContentType.C => ".c",
                ContentType.CSharp => ".cs",
                ContentType.CPP => ".cpp",
                ContentType.MsDoc => ".doc",
                ContentType.MsDocXML => ".docx",
                ContentType.MsPowerPointPresentation => ".ppt",
                ContentType.MsExcelXML => ".xls",
                ContentType.Java => ".java",
                ContentType.Markdown => ".md",
                ContentType.HypertextPreprocessor => ".php",
                ContentType.Python => ".py",
                ContentType.Ruby => ".rb",
                ContentType.TeX => ".tex",
                ContentType.CascadingStyleSheets => ".css",
                ContentType.JavaScript => ".js",
                ContentType.ShellScript => ".sh",
                ContentType.TypeScript => ".ts",
                ContentType.OctetStream => ".bin",
                ContentType.PDF => ".pdf",
                ContentType.MPEG => ".mp3",
                ContentType.WAV => ".wav",
                ContentType.JPEG => ".jpeg",
                ContentType.PNG => ".png",
                ContentType.GIF => ".gif",
                ContentType.MP4 => ".mp4",
                ContentType.AVI => ".avi",
                ContentType.TapeArchive => ".tar",
                ContentType.ZIP => ".zip",
                _ => ".unknown",
            };
        }
    }
}