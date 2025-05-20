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
     * tu dodajemy zmianę katalogów +
     * czy coś jeszcze ?
     * może być zamiast dodawnia do bazy to dodawanie do osobnej listy
     * i jak będzie ok to metoda wywołująca sama sobie doda wybrane pliki
     * 
     * 
     */



    [ObservableProperty]
    private string _pathC = string.Empty;

    public ObservableCollection<FilesIO> FilesList { get; set; } = [];

    [RelayCommand]
    private void ReLoad(object t)
    {
        if (t != null)
        {
            FilesIO f = t as FilesIO;
            if (f.Extension == "DIR")
            {
                if (f.Name != ". . .")
                {
                    PathSearch(f.Path +@"\"+ f.Name + @"\");                    
                }
                else if(f.Name == ". . .")               
                    PathSearch(f.Path[..(f.Path[..^2].LastIndexOf('\\') + 1)]);
            }            
        }        
    }

    [RelayCommand]
    private void OnLoaded()
    {
        PathSearch(string.Empty);
    }
    //wczytuje dane z wskazanego katalogu, dodać filtację plików
    // ma pokazywać tylko wybrane
    private void PathSearch(string path)
    {
        if (!string.IsNullOrWhiteSpace(path))
        {
            PathC = path.Trim();
        }
        //if (string.IsNullOrWhiteSpace(PathC)) return;
        if (!string.IsNullOrEmpty(PathC))
        {
            FilesList.Clear();
            try
            {
                var imFiles = Directory.EnumerateFiles(PathC);
                var imDirectories = Directory.EnumerateDirectories(PathC);
                if (!IsDrive(PathC))
                {
                    FilesList.Add(new FilesIO()
                    {
                        Name = ". . .",
                        Extension = "DIR",
                        Path = PathC,
                        Icon = null,
                        Size = "0",
                        RealSize = "0",
                        MD5 = "0"
                    });

                }


                foreach (var imDir in imDirectories)
                {
                    //Debug.WriteLine(imDir.Substring(imDir.LastIndexOf('\\') + 1));
                    FilesList.Add(new FilesIO()
                    {
                        Name = imDir[(imDir.LastIndexOf('\\') + 1)..],
                        Extension = "DIR",
                        Path = PathC,
                        Icon = null,
                        Size = "0",
                        RealSize = "0",
                        MD5 = "0"
                    });
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
                    FilesList.Add(new FilesIO()
                    {
                        Id = imFile.i,
                        Name = name,
                        Extension = ext,
                        Path = PathC,
                        Icon = null,
                        Size = Tools.Prdouble(finfo.Length),
                        RealSize = finfo.Length.ToString(),
                        MD5 = "0"/* CalculateMD5(imFile)*/
                    });
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

    private static bool IsDrive(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return false;
        string pathX = path.Trim();
        if (pathX.Length == 3)
            if ((pathX[1] == ':') && (pathX[2] == '\\'))
                return true;
        return false;
    }
}
