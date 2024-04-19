namespace Glitch9.IO.RESTApi
{
    public enum ContentType
    {
        // Text Responses
        JSON,
        XML,
        Form,
        Multipart,
        PlainText,
        HTML,
        CSV,
        XmlText,

        // Binary Responses
        OctetStream,
        PDF, // this might be mistaken for a Text response, but it's a binary response
        MPEG, 
        WAV, 
        JPEG, 
        PNG,
        GIF, 
        MP4, 
        AVI 
    }

    public static class ContentTypeExtensions
    {
        public static string ToMIME(this ContentType contentType)
        {
            return contentType switch
            {
                // Text
                ContentType.JSON => "application/json",
                ContentType.XML => "application/xml",
                ContentType.Form => "application/x-www-form-urlencoded",
                ContentType.Multipart => "multipart/form-data",
                ContentType.PlainText => "Text/plain",
                ContentType.HTML => "Text/html",
                ContentType.CSV => "Text/csv",
                ContentType.XmlText => "Text/xml",

                // Binary
                ContentType.OctetStream => "application/octet-stream",
                ContentType.PDF => "application/pdf",
                ContentType.MPEG => "audio/mpeg",
                ContentType.WAV => "audio/wav",
                ContentType.JPEG => "image/jpeg",
                ContentType.PNG => "image/png",
                ContentType.GIF => "image/gif",
                ContentType.MP4 => "video/mp4",
                ContentType.AVI => "video/x-msvideo",
                _ => "application/json"
            };
        }

        public static RESTHeader GetHeader(this ContentType contentType)
        {
            return new RESTHeader("Content-Type", contentType.ToMIME());
        }
    }
}