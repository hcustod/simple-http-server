namespace simple_http_server;

public class Logger
{
    private static readonly string filepath = "server.log";

    public static void Info(string message)
    {
        WriteLog("INFO", message);
    }

    public static void Error(string message)
    {
        WriteLog("ERROR", message);
    }

    public static void Warning(string message)
    {
        WriteLog("WARN", message);
    }

    private static void WriteLog(string level, string message)
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        string logLine = $"[{timestamp}] [{level}] {message}";
        
        Console.WriteLine(logLine);

        try
        {
            File.AppendAllText(filepath, logLine + Environment.NewLine);
        }
        catch (Exception e)
        {
            Console.WriteLine($"[Logger] Failed to wrtie to log file: {e.Message}");
        }
    }
}