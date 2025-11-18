using SimpleHttpServer.Server;

namespace SimpleHttpServer.Controllers
{
    public class TimeController
    {
        public HttpResponse Time()
        {
            return new HttpResponse($"<p>The time is: {DateTime.Now}</p>", "text/html");
        }
    }
}