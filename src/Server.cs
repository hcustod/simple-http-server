using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using simple_http_server.Models;

namespace simple_http_server;

public class Server
{
    private static TcpListener listener;
    private static readonly int MaxConnections = 20;
    private static readonly Semaphore Semaphore = new(MaxConnections, MaxConnections);
    private static Router router;

    public static void Start()
    {
        // Determine and print local IPs
        string websitePath = GetWebsitePath();
        Console.WriteLine("Website path resolved to: " + websitePath);

        router = new Router { WebsitePath = websitePath };

        var localIPs = GetAllLocalHost();
        foreach (var ip in localIPs)
        {
            Console.WriteLine($"Will be listening on http://{ip}:8080/");
        }

        listener = new TcpListener(IPAddress.Any, 8080);
        listener.Start();

        Console.WriteLine("Server started.");

        Task.Run(RunServer);
    }

    private static string GetWebsitePath()
    {
        // Example: /Users/hcustodio/Projects/simple-http-server/src/bin/Debug/net8.0/
        string exeLocation = System.Reflection.Assembly.GetEntryAssembly().Location;

        // Navigate to the project root
        string projectRoot = Directory.GetParent(exeLocation).Parent.Parent.Parent.Parent.FullName;

        // Combine with Website folder
        return Path.Combine(projectRoot, "Website");
    }

    private static List<IPAddress> GetAllLocalHost()
    {
        return Dns.GetHostEntry(Dns.GetHostName())
            .AddressList
            .Where(ip => ip.AddressFamily == AddressFamily.InterNetwork)
            .ToList();
    }

    private static void RunServer()
    {
        while (true)
        {
            Semaphore.WaitOne();
            var client = listener.AcceptTcpClient();
            Task.Run(() =>
            {
                HandleClient(client);
                Semaphore.Release();
            });
        }
    }

    private static void HandleClient(TcpClient client)
    {
        using var stream = client.GetStream();
        using var reader = new StreamReader(stream);
        using var writer = new StreamWriter(stream) { AutoFlush = true };

        string requestLine = reader.ReadLine();
        if (string.IsNullOrEmpty(requestLine)) return;

        Console.WriteLine($"Received: {requestLine}");

        string[] tokens = requestLine.Split(' ');
        if (tokens.Length < 2) return;

        string method = tokens[0];
        string rawUrl = tokens[1];
        string path = rawUrl.Split('?')[0];
        string queryString = rawUrl.Contains('?') ? rawUrl.Split('?')[1] : "";

        var kvParams = new Dictionary<string, string>(); // Extend if needed to parse query string

        var responsePacket = router.Route(method, path, kvParams);

        if (responsePacket != null)
        {
            string headers = $"HTTP/1.1 200 OK\r\nContent-Type: {responsePacket.ContentType}\r\nContent-Length: {responsePacket.Data.Length}\r\n\r\n";
            stream.Write(Encoding.UTF8.GetBytes(headers));
            stream.Write(responsePacket.Data);
        }
        else
        {
            string notFound = "<html><body><h1>404 - Not Found</h1></body></html>";
            string headers = "HTTP/1.1 404 Not Found\r\nContent-Type: text/html\r\n" +
                             $"Content-Length: {Encoding.UTF8.GetByteCount(notFound)}\r\n\r\n";
            stream.Write(Encoding.UTF8.GetBytes(headers));
            stream.Write(Encoding.UTF8.GetBytes(notFound));
        }
    }
}
