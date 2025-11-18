namespace http_server.Controllers
{
    public class HomeController
    {
        public string Index() => "<h1>Welcome!</h1>";
        public string Hello() => "<h1>Hello from the router!</h1>";
    }
}