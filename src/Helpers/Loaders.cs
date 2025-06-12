using System.Text;
using simple_http_server.Models;

namespace simple_http_server.Helpers;

public class Loaders
{
    public static Router router;
    
    public static ResponsePacket ImageLoader(string fullpath, string ext, ExtensionInfo extInfo)
    {
        using FileStream fs = new FileStream(fullpath, FileMode.Open);
        using BinaryReader br = new BinaryReader(fs);

        return new ResponsePacket()
        {
            Data = br.ReadBytes((int)fs.Length),
            ContentType = extInfo.ContentType
        };
    }

    public static ResponsePacket FileLoader(string fullpath, string ext, ExtensionInfo extInfo)
    {
        string text = File.ReadAllText(fullpath);

        return new ResponsePacket()
        {
            Data = Encoding.UTF8.GetBytes(text),
            ContentType = extInfo.ContentType,
            Encoding = Encoding.UTF8
        };
    }

    public static ResponsePacket PageLoader(string fullpath, string ext, ExtensionInfo extInfo)
    {
        if (Path.GetFileName(fullpath) == string.Empty || fullpath.EndsWith("Pages"))
        {
            return router.Route("GET", "/index.html", null);
        }

        if (string.IsNullOrEmpty(ext))
        {
            fullpath += ".html";
        }
        
        string relativePath = fullpath.Replace(router.WebsitePath, "").TrimStart(Path.DirectorySeparatorChar);
        string newFullPath = Path.Combine(router.WebsitePath, "Pages", relativePath);
        
        return FileLoader(newFullPath, "html", extInfo);
    }
    
    private ResponsePacket Error404()
    {
        return new ResponsePacket
        {
            Data = Encoding.UTF8.GetBytes("<h1>404 - File Not Found</h1>"),
            ContentType = "text/html",
            Encoding = Encoding.UTF8
        };
    }
}
    
    