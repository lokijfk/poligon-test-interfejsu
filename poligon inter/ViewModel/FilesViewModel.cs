
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using poligon_inter.Model;
using poligon_inter.View;
using System.Collections.ObjectModel;
using System.Diagnostics;


namespace poligon_inter.ViewModel;

public partial class FilesViewModel: ObservableObject
{
    //private string _pathSearch;
    [ObservableProperty]
    private string _path = string.Empty;


    public ObservableCollection<FilesIO> Files { get; set; } = [];

    public FilesViewModel()
    {
        //Files = new ObservableCollection<FilesIO>();
        //tu powinnobyć łądowanie plików z bazy danych
        // może być osobną metodą
    }

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

    
    [RelayCommand]
    private async Task FilesFindWindow(string path)
    {
        FilesFindViewModel FFVM = new FilesFindViewModel
        {
            PathC = path
        };
        //FFVM.PathSearch(string.Empty);
        object? view = new FilesFind
        {
            DataContext = FFVM
        };

        object? result = await DialogHost.Show(view, "RootDialog");

        //Debug.WriteLine("Dialog was closed, the CommandParameter used to close it was: " + (result ?? "NULL")+" - " + DC.Name);
        //to poniżej można przenieść do metody ClosedEventHandler
        Debug.WriteLine("result : " + (result??(bool)result));
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


