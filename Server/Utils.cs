namespace SimpleHttpServer.Server
{
    public static class Utils
    {
        // Helper to visualize control characters
        public static string EscapeSpecialChars(string input)
        {
            return input
                .Replace("\r", "\\r")
                .Replace("\n", "\\n")
                .Replace("\t", "\\t");
        }
    }
}
