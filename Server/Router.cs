using SimpleHttpServer.Controllers;

namespace SimpleHttpServer.Server
{
    public class Router
    {
        private readonly Dictionary<string, Func<HttpResponse>> _routes = new();

        public void AddRoute(string path, Func<HttpResponse> handler)
        {
            _routes[path] = handler;
        }

        public HttpResponse Handle(string path)
        {
            if (_routes.TryGetValue(path, out var handler))
                return handler();

            var notFound = new NotFoundController();
            return notFound.NotFound();
        }
    }
}