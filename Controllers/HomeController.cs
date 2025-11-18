using SimpleHttpServer.Server;

namespace SimpleHttpServer.Controllers
{
    public class HomeController
    {
        public HttpResponse Index()
        {
            return new HttpResponse("<h1>Welcome!</h1>", "text/html");
        }

        public HttpResponse Hello()
        {
            return new HttpResponse("<h1>Hello from the router!</h1>", "text/html");
        }
    }
}