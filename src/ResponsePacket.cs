using System.Text;

namespace simple_http_server;

public class ResponsePacket
{
    public byte[] Data { get; set; }
    public string ContentType { get; set; }
    public Encoding? Encoding { get; set; }
}