namespace simple_http_server;

public class Logger
{
    private static readonly string filepath = "server.log";

    public static void Info(string message)
    {
        WriteLog("INFO")
    }
}