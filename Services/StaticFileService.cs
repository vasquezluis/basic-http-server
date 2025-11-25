using SimpleHttpServer.Server;

namespace SimpleHttpServer.Services
{
    public class StaticFileService
    {
        private static readonly Dictionary<string, string> _mimeTypes = new()
        {
            { ".html", "text/html" },
            { ".htm",  "text/html" },
            { ".css",  "text/css" },
            { ".js",   "application/javascript" },
            { ".json", "application/json" },
            { ".png",  "image/png" },
            { ".jpg",  "image/jpeg" },
            { ".jpeg", "image/jpeg" },
            { ".gif",  "image/gif" },
            { ".svg",  "image/svg+xml" },
            { ".txt",  "text/plain" },
        };

        public static bool TryGetFile(string path, out HttpResponse response)
        {
            response = null!;

            if (path == "/")
                path = "/index.html"; // default file

            string filePath = "wwwroot" + path.Replace("/", Path.DirectorySeparatorChar.ToString());

            if (!File.Exists(filePath))
                return false;

            // read file
            byte[] fileBytes = File.ReadAllBytes(filePath);

            string extension = Path.GetExtension(filePath).ToLower();
            string contentType = _mimeTypes.ContainsKey(extension)
                ? _mimeTypes[extension]
                : "application/octet-stream";

            // create response
            response = new HttpResponse(
                body: fileBytes,
                contentType: contentType,
                isBinary: true
            );

            return true;
        }
    }
}