using SimpleHttpServer.Server;

namespace SimpleHttpServer.Controllers
{
    public class NotFoundController
    {
        public HttpResponse NotFound()
        {
            return new HttpResponse($"<p>Page Not Found!</p>", "text/html");
        }
    }
}