
namespace poligon_inter.Model;

public class FilesIO
{
    public bool select { get; set; } = false;
    public string Path { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty ;
    public string Extension { get; set; } = string.Empty;
    public string icon { get; set; } = string.Empty;
    public string size { get; set; } = string.Empty;
    public string realSize { get; set; } = string.Empty;
    public string MD5 { get; set; } = string.Empty;
    public int id { get; set; } = -1;
}
