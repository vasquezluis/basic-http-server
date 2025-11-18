using System.Text.Json;
using SimpleHttpServer.Server;

namespace SimpleHttpServer.Controllers
{
    public class ApiController
    {
        public HttpResponse GetTime()
        {
            var obj = new
            {
                now = DateTime.Now,
                ticks = DateTime.Now.Ticks
            };

            string json = JsonSerializer.Serialize(obj);

            return new HttpResponse(json, "application/json");
        }
    }
}