using System.Text;
using simple_http_server.Models;
using simple_http_server.Helpers;
using static simple_http_server.Helpers.Loaders;

namespace simple_http_server;

public class Router
{
    public string WebsitePath { get; set; }

    private Dictionary<string, ExtensionInfo> extFolderMap;

    public Router()
    {
        // Assign this router instance to the loader
        Loaders.router = this;

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

        // Use fallback content type and loader if extension is unknown
        if (!extFolderMap.TryGetValue(ext, out ExtensionInfo extInfo))
        {
            extInfo = extFolderMap[""];
        }

        string fullPath = Path.Combine(WebsitePath, cleanPath);
        return extInfo.Loader(fullPath, ext, extInfo);
    }
}