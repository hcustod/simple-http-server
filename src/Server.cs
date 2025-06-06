using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace simple_http_server;

public class Server
{
    private static TcpListener listener;
    private static int maxConConnections = 20;
    private static Semaphore sem = new Semaphore(maxConConnections, maxConConnections);

    public static void Start()
    {
        // Log all local IP's
        var localIPs = GetAllLocalHost();
        foreach (var ip in localIPs)
        {
            Console.WriteLine($"Will be listening on http://{ip}:8080/");
        }
        
        listener = new TcpListener(IPAddress.Any, 8080);
        listener.Start();
        
        Console.WriteLine("Server started");
        foreach (var ip in localIPs)
        {
            Console.WriteLine($"Listening on {ip} :8080");
        }
        
        Task.Run(() => RunServer());
    }

    private static List<IPAddress> GetAllLocalHost()
    {
        return Dns.GetHostEntry(Dns.GetHostName()).AddressList
            .Where(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToList();
    }

    private static void RunServer()
    {
        while (true)
        {
            sem.WaitOne();
            var client = listener.AcceptTcpClient();
            Task.Run(() =>
            {
                HandleClient(client);
                sem.Release();
            });
        }
    }

    private static void HandleClient(TcpClient client)
    {
        using var stream = client.GetStream();
        using var reader = new StreamReader(stream);
        using var writer = new StreamWriter(stream) { AutoFlush = true };

        string requestLine = reader.ReadLine();
        Console.WriteLine($"Received: {requestLine}"); 
        
        // Basic response
        string responseBody = 
            "<html><body><h1>Hello Browser!</h1></body></html>";
        string response = 
            "HTTP/1.1 200 OK\r\n" +
            "Content-Type: text/html; charset=utf-8\r\n" +
            $"Content-Length: {Encoding.UTF8.GetByteCount(responseBody)}\r\n" +
            "\r\n" +
            responseBody;
        
        writer.Write(response);
    }
    
}