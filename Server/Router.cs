namespace SimpleHttpServer.Server
{
    public class Router
    {
        private readonly Dictionary<string, Func<string>> _routes = new();

        public void AddRoute(string path, Func<string> handler)
        {
            _routes[path] = handler;
        }

        public string Handle(string path)
        {
            if (_routes.TryGetValue(path, out var handler))
                return handler();

            return "<h1>404 - Not Found</h1>";
        }
    }
}