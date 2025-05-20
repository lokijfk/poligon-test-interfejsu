
using CommunityToolkit.Mvvm.ComponentModel;
using MaterialDesignThemes.Wpf;
using poligon_inter.Model;
using poligon_inter.View;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Media;




namespace poligon_inter.ViewModel;
/// <summary>
/// do kasacji, zastąpiony przez widok główny
/// </summary>
public partial class FilesViewModel : ObservableObject
{
    //private string _pathSearch;
    //[ObservableProperty]
    //private string _path = string.Empty;

    public TreeModel SelectedItem;
    public ObservableCollection<FilesIO> FilesList { get; set; } = [];

    private ImageSource BlinkIcom { get; set; } = Tools.CreateEmtpyBitmapSource();

    public async Task ScanPath(string path)
    {
        await ScanPath(path,true);
    }

    /// <summary>
    /// skanuje wybrany katalog i dodaje pliki z zakresu do bazy
    /// zakres - to pliki o rozszeżeniach określonych jako możliwe do gromadznia w tej bzie
    /// </summary>
    /// <param name="path">ścieżka do katalogu</param>
    /// <param name="branch">skanuj podkatalogi</param>
    public async Task ScanPath(string path, bool branch)
    {
        if (!string.IsNullOrWhiteSpace(path))
        {
            path = path.Trim();
        }
        //if (string.IsNullOrWhiteSpace(PathC)) return;
        if (!string.IsNullOrEmpty(path))
        {
            try
            {
                var imFiles = Directory.EnumerateFiles(path);
                var imDirectories = Directory.EnumerateDirectories(path);

                AddFilesToList(path);
                if (branch)
                    foreach (var imDir in imDirectories)
                    {
                        DirectoryInfo directoryInfo = new(imDir);

                        if (Tools.AccessDirectory(imDir) && Tools.AtrDir(imDir))
                        {
                            await  ScanPath(imDir, true);
                        }
                    }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }

    
    private void AddFilesToList(string path)
    {
        var imFiles = Directory.EnumerateFiles(path);
        FileInfo finfo;
        string ext, name;
        foreach (var imFile in imFiles.Select((value, i) => (value, i)))
        {
            ext = System.IO.Path.GetExtension(imFile.value);
            name = System.IO.Path.GetFileName(imFile.value);
            finfo = new FileInfo(imFile.value);
            FilesList.Add(new FilesIO()
            {
                Id = imFile.i,
                Name = name,
                Extension = ext,
                Path = path,
                Icon = BlinkIcom,
                Size = Tools.Prdouble(finfo.Length),
                RealSize = finfo.Length.ToString(),
                MD5 = "0"
            });
        }
    }


    /// <summary>
    /// dodaje wybrany plik do bazy
    /// </summary>
    /// <param name="pathFile">ścieżka z plikiem</param>
    /// <param name="path">ścieżka bez pliku</param>
    /// <returns>czy dodano plik?</returns>
    public static bool AddFileToDB(string pathFile, string path)
    {
        //var imFiles = Directory.EnumerateFiles(path);
        FileInfo finfo = new FileInfo(pathFile);
        string Ext, Name;
        Ext = System.IO.Path.GetExtension(pathFile);
        Name = System.IO.Path.GetFileName(pathFile);
        //no dobra ale do jakiej bazy dodajemy ??
        // jak by to był MV główny to bym to miał ale tu nie mam :/


        return false;
    } 
    
    /// <summary>
    /// wywołuje okno pomocnicze do wyboru plików
    /// </summary>
    /// <param name="path"></param>
    public async void PathSearch(string path)
    {
        /*
        //ObservableCollection<FilesIO> Files = [];
        Path = path.Trim();
        if (!string.IsNullOrEmpty(path))
        {
            //Files = new ObservableCollection<FilesIO>();
            try
            {
                var imFiles = Directory.EnumerateFiles(Path);
                var imDirectories = Directory.EnumerateDirectories(Path);

                foreach (var imDir in imDirectories)
                {

                    Files.Add(new FilesIO() { Name = imDir.Substring(imDir.LastIndexOf('\\') + 1), Extension = "DIR", Path = Path, icon = "", size = "0", realSize = "0", MD5 = "0" });
                }

                FileInfo finfo;
                string ext, name;
                foreach (var imFile in imFiles.Select((value, i) => (value, i)))
                {
                    ext = System.IO.Path.GetExtension(imFile.value);
                    name = System.IO.Path.GetFileName(imFile.value);
                    finfo = new FileInfo(imFile.value);
                    ///* przeliczanie MD5 za długo trwa tu trzeba coś innego wymyśleć
                    Files.Add(new FilesIO() { id = imFile.i, Name = name, Extension = ext, Path = Path, icon = "", size = Tools.Prdouble(finfo.Length), realSize = finfo.Length.ToString(), MD5 = "0" });
                    // AllowUIToUpdate(); to nie zdaje tutaj efektu
                    //Widok.ItemsSource = items;
                }
            }
            catch (Exception ex) 
            {
                Debug.WriteLine(ex);
            }


            await FilesFindWindow(Files);
        */
        await FilesFindWindow(path);// tu powinno być inne okno do wybierania plikó
                                    //tu powinno być dodawanie wybranych plików do bazy
                                    // i tu ponowne ładowanie plików z bazy do listy ewdług założonego sortowania

        // }
    }

    /// <summary>
    /// okno pomocnicze do wyboru plików
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    //[RelayCommand]
    private static async Task FilesFindWindow(string path)
    {
        FilesFindViewModel FFVM = new()
        {
            PathC = path
        };
        //FFVM.PathSearch(string.Empty);
        object? view = new FilesFind
        {
            DataContext = FFVM
        };

        var result = await DialogHost.Show(view, "RootDialog");

        //Debug.WriteLine("Dialog was closed, the CommandParameter used to close it was: " + (result ?? "NULL")+" - " + DC.Name);
        //to poniżej można przenieść do metody ClosedEventHandler
        //Debug.WriteLine("result : " + result);
    }



    /*
    [RelayCommand]
    private async Task FilesFindWindow(ObservableCollection<FilesIO> Files)
    {
        FilesFindViewModel FFVM = new FilesFindViewModel
        {
            //PathC = path
            Files = Files
        };
        //FFVM.PathSearch(string.Empty);
        object? view = new FilesFind
        {
            DataContext = FFVM
        };

        object? result = await DialogHost.Show(view, "RootDialog");

        //Debug.WriteLine("Dialog was closed, the CommandParameter used to close it was: " + (result ?? "NULL")+" - " + DC.Name);
        //to poniżej można przenieść do metody ClosedEventHandler
        Debug.WriteLine("result : " + (result ?? (bool)result));
    }
    */

}


