using System.Runtime.InteropServices;
using System.Text;
using simple_http_server.Models;
using static simple_http_server.Helpers.Loaders;

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

    public ResponsePacket Route(string action, string path, Dictionary<string, string> queryParams)
    {
        string ext = Path.GetExtension(path).TrimStart('.').ToLower();
        string cleanPath = path.TrimStart('/');

        // Unknown extension or type 
        if (!extFolderMap.TryGetValue(ext, out ExtensionInfo extInfo))
        {
            return new ResponsePacket
            {
                Data = Encoding.UTF8.GetBytes("<h1> 404 not found </h1>"),
                ContentType = "text/html",
                Encoding = Encoding.UTF8
            };
        }

        string fullpath = Path.Combine(WebsitePath, cleanPath);
        return extInfo.Loader(fullpath, ext, extInfo);
    }
}