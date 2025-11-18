namespace SimpleHttpServer.Server
{
    public class HttpResponse
    {
        public string Body { get; set; }
        public string ContentType { get; set; }

        public HttpResponse(string body, string contentType = "text/html")
        {
            Body = body;
            ContentType = contentType;
        }
    }
}