using System.Net; // >> Contains IPAddress for network addressing
using System.Net.Sockets;
using http_server.Controllers; // >> Provides TcpListener and TcpClient for low-level TCP networking

namespace SimpleHttpServer.Server
{
    class HttpServer
    {
        private readonly int _port;
        private TcpListener? _listener; // ? waits for incomming connections
        private bool _isRunning;
        private readonly Router _router = new();

        public HttpServer(int port = 8080)
        {
            _port = port;

            var home = new HomeController();
            var time = new TimeController();

            _router.AddRoute("/", home.Index);
            _router.AddRoute("/hello", home.Hello);
            _router.AddRoute("/time", time.Time);
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
                using (var stream = client.GetStream()) // ? Gets the raw network stream for reading/writing bytes
                using (var reader = HttpRequestReader.Create(stream)) // ? Wraps the stream to read text (HTTP is text-based)
                using (var writer = HttpResponseWriter.Create(stream)) // ? Whaps the stream to write text response
                {
                    string httpRequest = await HttpRequestReader.ReadAsync(reader); // ? the actual request

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
                        await HttpResponseWriter.WriteAsync(writer, requestLine, _router);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling client: {ex.Message}");
            }
        }

        public void Stop()
        {
            _isRunning = false;
            _listener?.Stop();
        }
    }
}
