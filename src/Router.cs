using System.Runtime.InteropServices;
using System.Text;

namespace simple_http_server;

using System.Net;
using System.Net.Sockets;
using System.Threading;

public class Router
{
    public string WebsitePath { get; set; }
    
    private Dictionary<string, ExtensionInfo> extFolderMap;
    
    public Router()
    {
        extFolderMap = new Dictionary<string, ExtensionInfo>()
        {
            { "ico", new ExtensionInfo() { Loader = ImageLoader, ContentType = "image/x-icon" } },
            { "png", new ExtensionInfo() { Loader = ImageLoader, ContentType = "image/png" } },
            { "jpg", new ExtensionInfo() { Loader = ImageLoader, ContentType = "image/jpeg" } },
            { "gif", new ExtensionInfo() { Loader = ImageLoader, ContentType = "image/gif" } },
            { "bmp", new ExtensionInfo() { Loader = ImageLoader, ContentType = "image/bmp" } },
            { "html", new ExtensionInfo() { Loader = PageLoader, ContentType = "text/html" } },
            { "css", new ExtensionInfo() { Loader = FileLoader, ContentType = "text/css" } },
            { "js", new ExtensionInfo() { Loader = FileLoader, ContentType = "text/javascript" } },
            { "", new ExtensionInfo() { Loader = PageLoader, ContentType = "text/html" } }, // fallback
        };
    }
}