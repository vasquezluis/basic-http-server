using System;
using System.Collections.Generic; // >> for Dictionary
using System.IO; // >> For StreamReader/StreamWriter to read/Write text over network streams
using System.Net; // >> Contains IPAddress for network addressing
using System.Net.Sockets; // >> Provides TcpListener and TcpClient for low-level TCP networking
using System.Text; // >> For Encoding.UTF8 to convert between strings and bytes
using System.Threading.Tasks; // >> For async/await operations

namespace HttpServer
{
    class SimpleHttpServer
    {
        private readonly int _port;
        private TcpListener? _listener; // ? waits for incomming connections
        private bool _isRunning;

        public SimpleHttpServer(int port = 8080)
        {
            _port = port;
        }

        public async Task StartAsync()
        {
            // ? accept connections on any network interface
            _listener = new TcpListener(IPAddress.Any, _port);
            _listener.Start();
            _isRunning = true;

            Console.WriteLine($"HTTP Server started on http://localhost:{_port}");
            Console.WriteLine("Press Ctrl+C to stop the server");

            while (_isRunning)
            {
                try
                {
                    // ? wait for client connection
                    TcpClient client = await _listener.AcceptTcpClientAsync();

                    // ? handle each client in a separate task (non-blocking)
                    // ? don't store to go for the next connection
                    _ = Task.Run(() => HandleClientAsync(client));
                }
                catch (ObjectDisposedException)
                {
                    // ? server was stopped
                    break;
                }
            }
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            try
            {
                // * All "using" statements ensure resources are disposed of properly, even if exceptions occur

                using (client) // ? Ensures CTP connection is properly closed when done
                using (NetworkStream stream = client.GetStream()) // ? Gets the raw network stream for reading/writing bytes
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8)) // ? Wraps the stream to read text (HTTP is text-based)
                using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8)) // ? Whaps the stream to write text response
                {
                    string httpRequest = await ReadHttpRequestAsync(reader); // ? the actual request

                    if (!string.IsNullOrEmpty(httpRequest))
                    {
                        // ? parse the request line (first line)
                        string[] requestLines = httpRequest.Split("\n");
                        string requestLine = requestLines[0].Trim();

                        // >> each requestLine looks like
                        /*
                          GET / HTTP/1.1
                          POST /api/users HTTP/1.1
                          GET /favicon.ico HTTP/1.1
                        */

                        // ? create a response
                        await SendResponseAsync(writer, requestLine);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling client: {ex.Message}");
            }
        }

        private async Task<string> ReadHttpRequestAsync(StreamReader reader)
        {
            var headers = new Dictionary<string, string>();
            var requestBuilder = new StringBuilder();
            string? line; // Made nullable to match ReadLineAsync return type

            Console.WriteLine("---- RAW REQUEST BEGIN ----");

            // ? read the request line and headers
            while (!string.IsNullOrEmpty(line = await reader.ReadLineAsync()))
            {
                // ? Print the raw line (escaped to show CRLF, tabs, etc.)
                Console.WriteLine($"[RAW] {EscapeSpecialChars(line)}");

                // ? Add line to request builder
                requestBuilder.AppendLine(line);

                int sep = line.IndexOf(":");
                if (sep > 0)
                {
                    headers[line[..sep]] = line[(sep + 1)..].Trim();
                }
                else if (string.IsNullOrWhiteSpace(line))
                {
                    // ? empty line indicates end of headers
                    break;
                }
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

        private async Task SendResponseAsync(StreamWriter writer, string requestLine)
        {
            // ? parse the requext method and path
            string[] parts = requestLine.Split(' ');
            string method = parts.Length > 0 ? parts[0] : "UNKNOWN";
            string path = parts.Length > 1 ? parts[1] : "/";

            // ? create simple HTML response
            string htmlContent = $@"
<!DOCTYPE html>
<html>
<head>
    <title>Simple HTTP Server</title>
</head>
<body>
    <h1>Hello World!</h1>
    <p><strong>Method:</strong> {method}</p>
    <p><strong>Path:</strong> {path}</p>
    <p><strong>Time:</strong> {DateTime.Now}</p>
</body>
</html>";

            // ? get the content in bytes from the html content (the client reads by the content length in bytes)
            byte[] contentBytes = Encoding.UTF8.GetBytes(htmlContent);

            // ? use \r\n explicitly for HTTP header lines (EOL -> RFCL)
            string header =
                "HTTP/1.1 200 OK\r\n" +
                "Content-Type: text/html; charset=utf-8\r\n" +
                $"Content-Length: {contentBytes.Length}\r\n" +
                "Connection: close\r\n" +
                "\r\n"; // end of headers

            // ? write directly to the base stream instead of StreamWriter
            Stream stream = writer.BaseStream;

            // ? get the headers in bytes
            byte[] headerBytes = Encoding.ASCII.GetBytes(header);

            // ? write headers and content
            await stream.WriteAsync(headerBytes, 0, headerBytes.Length);
            await stream.WriteAsync(contentBytes, 0, contentBytes.Length);
            await stream.FlushAsync();
        }

        // Helper to visualize control characters
        private static string EscapeSpecialChars(string input)
        {
            return input
                .Replace("\r", "\\r")
                .Replace("\n", "\\n")
                .Replace("\t", "\\t");
        }

        public void Stop()
        {
            _isRunning = false;
            _listener?.Stop();
        }
    }

    class Program
    {
        public static async Task Main(string[] args)
        {
            SimpleHttpServer server = new SimpleHttpServer(8080);

            // handle Ctrl+C gracefully
            // ? event that fires when user press Ctrl+C
            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true; // ? prevents the default behavior (immediate program termination)
                server.Stop(); // ? call the custom stop method instead
            };

            await server.StartAsync(); // ? running the server
        }
    }
}
