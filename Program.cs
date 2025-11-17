using SimpleHttpServer.Server;

namespace SimpleHttpServer
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var server = new HttpServer(8080);

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
