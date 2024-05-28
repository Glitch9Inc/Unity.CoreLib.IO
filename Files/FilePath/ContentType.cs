using Glitch9.IO.RESTApi;

namespace Glitch9.IO.Files
{
    public enum ContentType
    {
        /// <summary>
        /// Represents JSON formatted text (.json).
        /// </summary>
        Json,

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
        /// Represents other forms of multipart data.
        /// </summary>
        Multipart,

        /// <summary>
        /// Represents plain text data (.txt).
        /// </summary>
        PlainText,

        /// <summary>
        /// Represents HTML formatted text (.html).
        /// </summary>
        Html,

        /// <summary>
        /// Represents comma-separated values (.csv).
        /// </summary>
        Csv,

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
        Pdf,

        /// <summary>
        /// Represents MPEG video format (.mpeg).
        /// </summary>
        Mpeg,

        /// <summary>
        /// Represents Waveform Audio File Format (.wav).
        /// </summary>
        Wav,

        /// <summary>
        /// Represents JPEG image format (.jpeg).
        /// </summary>
        Jpeg,

        /// <summary>
        /// Represents JPEG image format (.jpg).
        /// </summary>
        Jpg,

        /// <summary>
        /// Represents Portable Network Graphics format (.png).
        /// </summary>
        Png,

        /// <summary>
        /// Represents Graphics Interchange Format (.gif).
        /// </summary>
        Gif,

        /// <summary>
        /// Represents MPEG-4 video format (.mp4).
        /// </summary>
        Mp4,

        /// <summary>
        /// Represents Audio Video Interleave format (.avi).
        /// </summary>
        Avi,

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
        MicrosoftDocument,

        /// <summary>
        /// Represents a Microsoft Word document in XML format (.docx).
        /// </summary>
        MicrosoftDocumentXML,

        /// <summary>
        /// Represents a Microsoft PowerPoint Presentation (.pptx).
        /// </summary>
        MicrosoftPowerPointPresentation,

        /// <summary>
        /// Represents a Microsoft Excel Spreadsheet in XML format (.xlsx).
        /// </summary>
        MicrosoftExcelSpreadsheetXML,

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
        Zip,
    }



    public static class ContentTypeExtensions
    {
        public static string ToMIME(this ContentType contentType)
        {
            return contentType switch
            {
                // 'application/json' - Commonly used in web APIs to exchange data that is structured in a compact, easy-to-parse format.
                ContentType.Json => "application/json",

                // 'application/xml' or 'text/xml'
                ContentType.Xml => "application/xml",
                ContentType.XmlText => "Text/xml",

                // Form Requests - Form requests are specifically used when you are submitting HTML form data to a server.
                ContentType.WWWForm => "application/x-www-form-urlencoded",
                ContentType.MultipartForm => "application/x-www-form-urlencoded",
                ContentType.Multipart => "multipart/form-data",

                // Text-Based Types (For API interactions, logging, configurations, etc.)
                ContentType.PlainText => "Text/plain",
                ContentType.Html => "Text/html",
                ContentType.Csv => "Text/csv",
                ContentType.C => "text/x-c",
                ContentType.CSharp => "text/x-csharp",
                ContentType.CPP => "text/x-c++",
                ContentType.MicrosoftDocument => "application/msword",
                ContentType.MicrosoftDocumentXML => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ContentType.MicrosoftPowerPointPresentation => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
                ContentType.MicrosoftExcelSpreadsheetXML => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
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
                ContentType.Pdf => "application/pdf",
                ContentType.Mpeg => "audio/mpeg",
                ContentType.Wav => "audio/wav",
                ContentType.Jpeg => "image/jpeg",
                ContentType.Png => "image/png",
                ContentType.Gif => "image/gif",
                ContentType.Mp4 => "video/mp4",
                ContentType.Avi => "video/x-msvideo",

                // Archive Types (For compressed files and collections of files)
                ContentType.TapeArchive => "application/x-tar",
                ContentType.Zip => "application/zip",

                _ => "application/json"
            };
        }

        public static RESTHeader GetHeader(this ContentType contentType)
        {
            return new RESTHeader("Content-Type", contentType.ToMIME());
        }
    }
}