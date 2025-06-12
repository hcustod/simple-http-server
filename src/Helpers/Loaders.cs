using System.Text;
using simple_http_server.Models;

namespace simple_http_server.Helpers;

public class Loaders
{
    public static Router router;

    public static ResponsePacket ImageLoader(string fullpath, string ext, ExtensionInfo extInfo)
    {
        try
        {
            using FileStream fs = new FileStream(fullpath, FileMode.Open, FileAccess.Read);
            using BinaryReader br = new BinaryReader(fs);

            return new ResponsePacket()
            {
                Data = br.ReadBytes((int)fs.Length),
                ContentType = extInfo.ContentType
            };
        }
        catch
        {
            return Error404();
        }
    }

    public static ResponsePacket FileLoader(string fullpath, string ext, ExtensionInfo extInfo)
    {
        try
        {
            string text = File.ReadAllText(fullpath);

            return new ResponsePacket()
            {
                Data = Encoding.UTF8.GetBytes(text),
                ContentType = extInfo.ContentType,
                Encoding = Encoding.UTF8
            };
        }
        catch
        {
            return Error404();
        }
    }

    public static ResponsePacket PageLoader(string fullpath, string ext, ExtensionInfo extInfo)
    {
        try
        {
            // Handle root or directory access
            if (string.IsNullOrWhiteSpace(Path.GetFileName(fullpath)) || Directory.Exists(fullpath))
            {
                fullpath = Path.Combine(router.WebsitePath, "index.html");
            }
            else
            {
                // If no extension, assume .html
                if (string.IsNullOrEmpty(ext))
                {
                    fullpath += ".html";
                }

                // Rebuild path to search in the "Pages" subdirectory (optional)
                string relativePath = fullpath.Replace(router.WebsitePath, "").TrimStart(Path.DirectorySeparatorChar);
                fullpath = Path.Combine(router.WebsitePath, "Pages", relativePath);
            }

            return FileLoader(fullpath, "html", extInfo);
        }
        catch
        {
            return Error404();
        }
    }

    private static ResponsePacket Error404()
    {
        return new ResponsePacket
        {
            Data = Encoding.UTF8.GetBytes("<h1>404 - File Not Found</h1>"),
            ContentType = "text/html",
            Encoding = Encoding.UTF8
        };
    }
}
