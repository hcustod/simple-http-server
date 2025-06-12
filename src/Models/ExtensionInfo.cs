using simple_http_server.Models;

namespace simple_http_server.Models;

public class ExtensionInfo
{
    public string ContentType { get; set; }
    
    public Func<string, string, ExtensionInfo, ResponsePacket> Loader { get; set; }
}