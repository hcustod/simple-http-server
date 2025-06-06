using System;

namespace simple_http_server;

class Program
{
    static void Main(string[] args)
    {
        Server.Start();
        Console.ReadLine();
    }
}