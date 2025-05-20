using Microsoft.Win32;
using poligon_inter.ViewModel;
using System.Windows;
using System.Windows.Controls;


namespace poligon_inter.View;

/// <summary>
/// Logika interakcji dla klasy Files.xaml
/// </summary>
public partial class Files : UserControl
{
    public Files( )
    {
        InitializeComponent();
        //DataContext = VM;
    }


    //public FilesViewModel VM;

    private void FindFiles_Click(object sender, RoutedEventArgs e)
    {
        var folder = new OpenFolderDialog();
        string path = string.Empty;
        if(folder.ShowDialog() == true)
        {
            path = folder.FolderName;
            //DataContext.PathSearch = path;
            (DataContext as MainWindowViewModel).OpenWindowPath(path);
        }
    }

    private void ScanFolder_Click(object sender, RoutedEventArgs e)
    {
        var folder = new OpenFolderDialog();
        string path = string.Empty;
        if (folder.ShowDialog() == true)
        {
            path = folder.FolderName;
            //DataContext.PathSearch = path;
            (DataContext as MainWindowViewModel).ScanPath(path);
        }
    }
}
