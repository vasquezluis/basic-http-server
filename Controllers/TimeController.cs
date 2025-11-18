namespace http_server.Controllers
{
    public class TimeController
    {
        public string Time() => $"<p>The time is: {DateTime.Now}</p>";
    }
}