using System.Text;
using SimpleHttpServer.Server; // >> For Encoding.UTF8 to convert between strings and bytes

namespace SimpleHttpServer.Server
{
    public static class HttpResponseWriter
    {
        public static StreamWriter Create(Stream stream)
        {
            return new StreamWriter(stream, Encoding.UTF8, leaveOpen: true);
        }

        public static async Task WriteAsync(StreamWriter writer, string requestLine, Router router)
        {
            // ? parse the requext method and path
            string[] parts = requestLine.Split(' ');
            string method = parts.Length > 0 ? parts[0] : "UNKNOWN";
            string path = parts.Length > 1 ? parts[1] : "/";

            HttpResponse response = router.Handle(path);

            // ? get the content in bytes from the html content (the client reads by the content length in bytes)
            byte[] contentBytes = Encoding.UTF8.GetBytes(response.Body);

            string header =
                "HTTP/1.1 200 OK\r\n" +
                $"Content-Type: {response.ContentType}; charset=utf-8\r\n" +
                $"Content-Length: {contentBytes.Length}\r\n" +
                "Connection: close\r\n" +
                "\r\n"; // end of headers

            byte[] headerBytes = Encoding.ASCII.GetBytes(header);

            // ? write directly to the base stream instead of StreamWriter
            Stream stream = writer.BaseStream;

            // ? write headers and content
            await stream.WriteAsync(headerBytes);
            await stream.WriteAsync(contentBytes);
            await stream.FlushAsync();
        }
    }
}
