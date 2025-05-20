
using System.Windows.Media;

namespace poligon_inter.Model;

public class FilesIO
{
    public bool Select { get; set; } = false;
    public string Path { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty ;
    public string Extension { get; set; } = string.Empty;
    public ImageSource Icon { get; set; }
    public string Size { get; set; } = string.Empty;
    public string RealSize { get; set; } = string.Empty;
    public string MD5 { get; set; } = string.Empty;
    public int Id { get; set; } = -1;
    public string File { get; set; } = string.Empty;
}
