using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using poligon_inter.Model;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;


namespace poligon_inter.ViewModel;

public partial class FilesFindViewModel : ObservableObject
{

    /* tu dodajemy przeszukiwanie katalogu +
     * tu dodajemy listę  plików i katalogów +
     * tu dodajemu dodawanie wybranego pliku do bazy - zrobić listę osobną i z niej dodawać
     * tu dodajemy zmianę katalogów
     * czy coś jeszcze ?
     * może być zamiast dodawnia do bazy to dodawanie do osobnej listy
     * i jak będzie ok to metoda wywołująca sama sobie doda wybrane pliki
     * 
     * 
     */

    

    [ObservableProperty]
    private string _pathC = string.Empty;

    public ObservableCollection<FilesIO> Files { get; set; } = [];

    [RelayCommand]
    private void onLoaded()
    {
        PathSearch(string.Empty);
    }
    public void PathSearch(string path)
    {
        if (!string.IsNullOrWhiteSpace(path))
        {
            PathC = path.Trim();
        }
        //if (string.IsNullOrWhiteSpace(PathC)) return;
        if (!string.IsNullOrEmpty(PathC))
        {
            
            //Files = new ObservableCollection<FilesIO>();
            //trzeba zobaczyć jak to przebudować żeby było szybsz, bo jest tragedia
            // jak by dało się włączyć skanowanie po uruchomieniu okna to bło by git
            try
            {
                var imFiles = Directory.EnumerateFiles(PathC);
                var imDirectories = Directory.EnumerateDirectories(PathC);

                foreach (var imDir in imDirectories)
                {
                    //Debug.WriteLine(imDir.Substring(imDir.LastIndexOf('\\') + 1));
                    Files.Add(new FilesIO() { Name = imDir.Substring(imDir.LastIndexOf('\\') + 1), Extension = "DIR", Path = PathC, icon = "", size = "0", realSize = "0", MD5 = "0" });
                }

                FileInfo finfo;
                string ext, name;
                foreach (var imFile in imFiles.Select((value, i) => (value, i)))
                {
                    ext = System.IO.Path.GetExtension(imFile.value);
                    name = System.IO.Path.GetFileName(imFile.value);
                    //Debug.WriteLine(name);
                    finfo = new FileInfo(imFile.value);
                    /* przeliczanie MD5 za długo trwa tu trzeba coś innego wymyśleć*/
                    Files.Add(new FilesIO() { id = imFile.i, Name = name, Extension = ext, Path = PathC, icon = "", size = Tools.Prdouble(finfo.Length), realSize = finfo.Length.ToString(), MD5 = "0"/* CalculateMD5(imFile)*/ });
                    // AllowUIToUpdate(); to nie zdaje tutaj efektu
                    //Widok.ItemsSource = items;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            //await CreateDB();// tu powinno być inne okno do wybierania plikó
            //tu powinno być dodawanie wybranych plików do bazy
            // i tu ponowne ładowanie plików z bazy do listy ewdług założonego sortowania

        }
    }
}
