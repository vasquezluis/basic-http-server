using System.Text; // >> For Encoding.UTF8 to convert between strings and bytes

namespace SimpleHttpServer.Server
{
    public static class HttpRequestReader
    {
        public static StreamReader Create(Stream stream)
        {
            return new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
        }

        public static async Task<string> ReadAsync(StreamReader reader)
        {
            var requestBuilder = new StringBuilder();
            string? line; // Made nullable to match ReadLineAsync return type

            Console.WriteLine("---- RAW REQUEST BEGIN ----");

            // ? read the request line and headers
            while (!string.IsNullOrEmpty(line = await reader.ReadLineAsync()))
            {
                // ? Print the raw line (escaped to show CRLF, tabs, etc.)
                Console.WriteLine($"[RAW] {Utils.EscapeSpecialChars(line)}");

                // ? Add line to request builder
                requestBuilder.AppendLine(line);
            }

            Console.WriteLine("---- RAW REQUEST END ----");

            /*
                HTTP request structure
                GET /hello HTTP/1.1
                Host: localhost:8080
                User-Agent: Mozilla/5.0...
                Accept: text/html,application/xhtml+xml...
                Accept-Language: en-US,en;q=0.5
                Connection: keep-alive
            */

            return requestBuilder.ToString();
        }
    }
}
