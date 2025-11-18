using System.Text;

namespace SimpleHttpServer.Server
{
    // response structure
    public class HttpResponse
    {
        public byte[] BodyBytes { get; set; }
        public string ContentType { get; set; }
        public bool IsBinary { get; }

        public HttpResponse(string body, string contentType = "text/html")
        {
            BodyBytes = Encoding.UTF8.GetBytes(body);
            ContentType = contentType;
            IsBinary = false;
        }

        public HttpResponse(byte[] body, string contentType, bool isBinary)
        {
            BodyBytes = body;
            ContentType = contentType;
            IsBinary = isBinary;
        }
    }
}