using SimpleHttpServer.Controllers;
using SimpleHttpServer.Services;

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
            // 1. Check static files
            if (StaticFileService.TryGetFile(path, out var fileResponse))
                return fileResponse;

            // 2. Check registered routes
            if (_routes.TryGetValue(path, out var handler))
                return handler();

            // 3. 
            var notFound = new NotFoundController();
            return notFound.NotFound();
        }
    }
}