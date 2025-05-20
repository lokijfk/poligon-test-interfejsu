using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using System.Collections.ObjectModel;
using poligon_inter.View;
using System.Diagnostics;
using poligon_inter.Model;
using System.Windows;
using Microsoft.Win32;
using System.Reflection;
using System.Windows.Media;
using System.IO;


namespace poligon_inter.ViewModel;

public partial class MainWindowViewModel: ObservableObject
{

    #region pola
    
    private BrokerDB BrokerDB ;    

    private readonly BrokerIni iniFile;
    private TreeModel? RActiveTreeModelItem = null;
    //LA - jest odbierany również  w pasku ToolBar,
    //jak tego nie ma to nie pokazuje błędu ale ikona cały czas się pali
    private ImageSource BlinkIcom { get; set; } = Tools.CreateEmtpyBitmapSource();

    //private IFileDialogService _iFileDialog;
    #endregion pola

    #region ObservableProperty

    // jest wykozystywane w widoku, przy drzewie??
    [ObservableProperty]
    private TreeModel? _lActiveTreeModelItem = null; 

    //public ObservableCollection<ContextMenuTreeView> TVCommandList { get; set; }

    public ObservableCollection<TreeModel>? Tree { get; set; }

    public ObservableCollection<FilesIO> FilesList { get; set; } = [];

    [ObservableProperty]
    private double _MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;

    [ObservableProperty]
    private bool _IsDialogOpen;

    [ObservableProperty]
    private object _selectedViewModel;


    //[ObservableProperty]
    //private bool _isExpand = false;// działa ale trzeba powiązać z ini

    #endregion ObservableProperty
    public MainWindowViewModel()
    {
        /*
        using (var edb = new DBSQLite())
        {
            Tree = edb.GetTree();
        }*/
        BrokerDB = new();
        Tree = BrokerDB.GetTree();
        iniFile = new BrokerIni();
        SelectedViewModel = CallMethod("Hello");
         //SelectedViewModel = new Welcome();
        //SelectedViewModel = System.Reflection.Assembly.GetExecutingAssembly().CreateInstance("Welcome");
        ContentMax = CurMainWindowState == WindowState.Normal ? "1" : "2";

        //trzeba zbudować menu item zależne od tego co było kliknięte prawym przyciskiem
        //ale prawdopodobnie tree będzie jedynym elementem obsługiwanym przez to menu to nie wiem czy jest sens komplikowania tego
        /*
        TVCommandList = new ObservableCollection<ContextMenuTreeView> {
        
            new ContextMenuTreeView( "Add", AddFolder ),//oo chodzi i można wyłączać jak coś nie gra :)
            new ContextMenuTreeView( "Delete", RemoveFolder )
        
        
            //new ContextMenuTreeView( "Add", AddFolder , CanExecuteEx),//oo chodzi i można wyłączać jak coś nie gra :)
            //new ContextMenuTreeView( "Delete", RemoveFolder , CanExecuteEx)
        
            };
        */
    }

    #region RelayCommand

    [RelayCommand]
    private void AddFolder(object t)
    {
        var name = "AAAXXd-new";//do zmiany na okno z możliwością wpisania nazwy folderu/kategorii
        if (RActiveTreeModelItem != null)
        {
            //wywołać okno do wpisania nazwy, ale czy to nie trzeba będzie zmienić metodę na async?
            RActiveTreeModelItem.AddChild(new TreeModel { Name = name, IsExpanded = true, Parent = RActiveTreeModelItem });
            AddCategory(name, RActiveTreeModelItem);
            RActiveTreeModelItem = null;
        }
        else
        {
            //TreeModel<Guid>? SelectedItem = TreeModel.GetSelectedNode(Tree);
            // to jakoś poprawić
            //Debug.WriteLine("Znalazłem Element: ");
            if ((t is TreeModel) && (t != null))
            {
                TreeModel SelectedItem = t as TreeModel;
                SelectedItem?.AddChild(new TreeModel { Name = name, IsExpanded = true, Parent = SelectedItem });                
                AddCategory(name, SelectedItem);
            }
            else if (LActiveTreeModelItem != null)
            {
                TreeModel SelectedItem = LActiveTreeModelItem as TreeModel;
                SelectedItem.AddChild(new TreeModel { Name = name, IsExpanded = true, Parent = SelectedItem });                
                AddCategory(name, SelectedItem);
            }        
       
        }
    }


    private void AddCategory(string name, TreeModel parent)
    {
        BrokerDB?.AddCategory(name, parent);
    }

    private void RemoveCategory(TreeModel id)
    {
        BrokerDB?.DeleteCategory(id);
    }

    [RelayCommand]
    private void RemoveFolder(object t)
    {

        if (RActiveTreeModelItem != null)
        {
            TreeModel? ParentItem = RActiveTreeModelItem.Parent;
       
            if ((ParentItem != null) &&(ParentItem.Children.Count > 0))
            {
                //Debug.WriteLine("usuwamy kategorię");
                ParentItem.Children.Remove(RActiveTreeModelItem);
                // dodać usuwanie elementu zbazy, ale co z zależnościami i kaskadowością?
                RemoveCategory(RActiveTreeModelItem);
            }
            else {
                //tu usuwanie bazy a więc pliku
                
                if (Tree != null) Tree.Remove(RActiveTreeModelItem);
                BrokerDB.DeleteDB(RActiveTreeModelItem);
            }       
            RActiveTreeModelItem = null;
        }

    }

    [RelayCommand]
    private void TreeModelLBMClick(object parameter)
    {
        if ((parameter is TreeModel) && (parameter != null))
        {
            //działa tu zrobić wybór widoku dla klikniętego elementu
            //tu można dodać pole lokalne LActiveTreeModelItem
            // _ = MessageBox.Show("kliknięto: " + (t as TreeModel<Guid>).Name);
            TreeModel c = parameter as TreeModel;
            LActiveTreeModelItem = c;
            //jak tu zrobić wybieralność widoków ?? 
            //SelectedViewModel = new Welcome();

            if((c!=null)&&(c.View != string.Empty))
            {
                SelectedViewModel = CallMethod(c.View);
            }else SelectedViewModel = CallMethod("Hello");
        }
        //Debug.WriteLine("LBM klik");

    }

    [RelayCommand]
    private void AddFile()
    {
        //tu niestetu należy zrobić włąsne okno bo kolory okna wbudowanego zależą  od systemu a nie od aplikacji
        //ale na razieto zostawimy co najwyżej przerobimy to nausługę

        //dodać brokera który będzie załatwiał takie rzeczy, czyli udostępniał metody openfile open dialog i inne okna
        // zwracał ich uchwyt do kontekstu 

        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.ShowDialog();

        // OpenFolderDialog openFoldrfDialog;
    }


    [RelayCommand]
    private void TreeModelRBMClick(object parameter)
    {
        if ((parameter is TreeModel) &&(parameter != null))
        {
            TreeModel c = parameter as TreeModel;
            RActiveTreeModelItem = c;
        }
        else
        {
            RActiveTreeModelItem = null;
        }
       // Debug.WriteLine("RBM klik");
    }



    #endregion RelayCommand

    #region private reakcja na klik katalog w drzewie

    private object? CallMethod(string p, object?[]? x = null)
    {
        Type thisType = GetType();
        if (thisType!= null)
        {   
            MethodInfo theMethod = thisType.GetMethod(p, BindingFlags.NonPublic | BindingFlags.Instance);
            //bez parametrów
            return theMethod?.Invoke(this, x);
            // z  parametrami
            //theMethod.Invoke(this, userParameters);
        }
        return null;
    }

    private object Hello()
    {
         
        return new Welcome();
    }


    private object Files()
    {
        //FilesViewModel vm = new FilesViewModel { SelectedItem = RActiveTreeModelItem };\
        List<FilesIO> files = BrokerDB.GetFiles(BrokerDB.GetTreeDBName(LActiveTreeModelItem));
        if (files != null)
        FilesList = new ObservableCollection<FilesIO>(files);
            return new Files();            
    }
    #endregion private


    #region WindowState

    //private DataRepository DataRepository { get; }

    //public MainViewModel() => this.DataRepository = new DataRepository();

    public void SaveAlbumName(string destinationFilePath)
    {
        // tu zrobić dodanie folderu do drzewa pod plikami jako plików wyszukanych z podanej ścieszki
        // i włączyć odpowiedni widok do operacji na plikach
        //this.DataRepository.SaveData(this.AlbumName, destinationFilePath);
    }

    [RelayCommand]
    private void onCmdMin()
    {
        CurMainWindowState = WindowState.Minimized;
    }

    [RelayCommand]
    private void onCmdMax()
    {   //tu można zostawić sam stan a wielkość i połozenie przeniesć do brokera
        // tam zmienią przy zmianie stanu
        if (CurMainWindowState == WindowState.Normal)
        {
            LastWidth = Width;
            LastHeihgt = Height;
            LastTop = Top;
            LastLeft = Left;
            CurMainWindowState = WindowState.Maximized;
            ContentMax = "2";
        }
        else
        {
            CurMainWindowState = WindowState.Normal;
            ContentMax = "1";
            Width = LastWidth;
            Height = LastHeihgt;
            Top = LastTop;
            Left = LastLeft;
        }
    }


    [ObservableProperty]
    private string _ContentMax;

    private int LastWidth
    {
        get => iniFile.LastWidth;
        set => SetProperty(iniFile.LastWidth, value, iniFile, (u, n) => u.LastWidth = n);
    }
    private int LastHeihgt
    {
        get => iniFile.LastHeihgt;
        set => SetProperty(iniFile.LastHeihgt, value, iniFile, (u, n) => u.LastHeihgt = n);
    }
    private int LastTop
    {
        get => iniFile.LastTop;
        set => SetProperty(iniFile.LastTop, value, iniFile, (u, n) => u.LastTop = n);
    }
    private int LastLeft
    {
        get => iniFile.LastLeft;
        set => SetProperty(iniFile.LastLeft, value, iniFile, (u, n) => u.LastLeft = n);
    }

    public bool ExtenderIsExpanded
    {
        get => iniFile.ExtenderIsExpanded;
        set => SetProperty(iniFile.ExtenderIsExpanded, value, iniFile, (u, n) => u.ExtenderIsExpanded = n);
    }

    public WindowState CurMainWindowState
    {
        get => iniFile.CurMainWindowState;
        set => SetProperty(iniFile.CurMainWindowState, value, iniFile, (u, n) => u.CurMainWindowState = n);
    }

    public int Width
    {
        get => iniFile.WindowWidth;
        set => SetProperty(iniFile.WindowWidth, value, iniFile, (u, n) => u.WindowWidth = n);            
    }

    public int Height
    {
        get => iniFile.WindowHeight;
        set => SetProperty(iniFile.WindowHeight, value, iniFile, (u, n) => u.WindowHeight = n);       
    }

    public int Top
    {
        get => iniFile.WindowTop;
        set => SetProperty(iniFile.WindowTop, value, iniFile, (u, n) => u.WindowTop = n);
    }

    public int Left
    {
        get => iniFile.WindowLeft;
        set => SetProperty(iniFile.WindowLeft, value, iniFile, (u, n) => u.WindowLeft = n);
    }

    #endregion WindowState


    #region RelayCommand Task

    [RelayCommand]
    private async Task CreateDB()
    {
        WindowAddDBViewModel DC = new WindowAddDBViewModel
        {
            Name = string.Empty,
            WindowName = "Utwórz nową bazę danych",
            Hint = "Nazwa bazy"
        };
          
        object? view = new WindowAddDB
        {
            DataContext = DC
        };

        object? result = await DialogHost.Show(view, "RootDialog");
                
        //to poniżej można przenieść do metody ClosedEventHandler
        //Debug.WriteLine("result : " + (bool)result);
        // czy jak doda obsługę błędów  to wypadną te ostrzeżenia ??
        if ((bool)result)
        {
            //return DC.Name;
            if ((BrokerDB != null)&&(DC.Name != string.Empty))
            {
                //Debug.WriteLine("nazwa jest : " + DC.Name);
                //tu robimy bazę danych, sprawdzić wcześniej czy nie ma juz takiej
                /*
                using (var db = new DBSQLite())
                {
                    int id = db.AddCategory(DC.Name);
                    Tree.Add(new TreeModel { Id = id, Name = DC.Name });
                }*/
                var item = BrokerDB.CreateDB(DC.Name);
                if((Tree != null)&&(item != null)) Tree.Add(item);
                //Tree.Add(new TreeModel { Id = 0, Name = DC.Name, IsExpanded = true });

            }
        }     
    
    }

    /*
    private async Task OnShowCD()
    {
        var vm = new WindowAddDBViewModel
        {
            Name = string.Empty,
            WindowName = "Utwórz nową bazę danych",
            Hint = "Nazwa bazy"
        };

        object? view = new WindowAddDB
        {
            DataContext = vm
        };

        object? result = await DialogHost.Show(view, "RootDialog",(object sender, DialogOpenedEventArgs e) =>
        {
            void OnClose(object _, EventArgs args)
            {
                vm.Close -= OnClose;
                e.Session.Close();
            }
            vm.Close += OnClose;
        }, ClosingEventHandler, ClosedEventHandler);
        Debug.WriteLine("result : " + (result ?? "NULL"));

    }
    

    #region okna testowe
    private async Task ShowMessage(MessageBoxXViewModel DC)
    {
        object? view = new MessageBoxX
        {
            DataContext = DC
        };

        object? result = await DialogHost.Show(view, "RootDialog", null, null, null);
    
    }


    private async Task<string> ShowDialog(SimpledialogViewModel DC)
    {
        //tak wszystko działa, pozostaje pododawać metody żeby mozna było wywołać jedno okno do różnych celów
        // DC = new SimpledialogViewModel { Name = string.Empty, WindowName = "Podaj nazwę nowej bazy", Hint = "Nazwa bazy" };
        object? view = new SimpleDialog
        {
            DataContext = DC
        };

        object? result = await DialogHost.Show(view,"RootDialog",null,null, ClosedEventHandler);

        //Debug.WriteLine("Dialog was closed, the CommandParameter used to close it was: " + (result ?? "NULL")+" - " + DC.Name);
    
        if ((result != null)&&(bool)result)
        {
            return DC.Name;
        }
            return "";        
    }
    
    #endregion okna testowe
    */
    private void ClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
    {
        //to jest opcjonalne, na razie zostawiam może jeszcze wykozystam
        Debug.WriteLine("You can intercept the closed event here (10)." + eventArgs.Parameter);
    }
    private void ClosedEventHandler(object sender, DialogClosedEventArgs eventArgs)
    {
        //to jest opcjonalne, na razie zostawiam może jeszcze wykozystam
        Debug.WriteLine("You can intercept the closed event here (1)." + eventArgs.Parameter );
    }

    #endregion RelayCommand Task

    #region Files

    public async Task ScanPath(string path)
    {
        await ScanPath(path, true);
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
            //if(Tools.AccessDirectory(path) && Tools.AtrDir(path))
            try
            {
                var imFiles = Directory.EnumerateFiles(path);
                var imDirectories = Directory.EnumerateDirectories(path);

                AddFilesToList(path);
                //if (branch)
                    foreach (var imDir in imDirectories)
                    {
                        DirectoryInfo directoryInfo = new(imDir);

                        //if (Tools.AccessDirectory(imDir) && Tools.AtrDir(imDir))
                        if (Tools.AtrDir(imDir))
                        {
                            await ScanPath(imDir, true);
                            //Debug.WriteLine("-- jest: " + imDir);
                        }
                        /*Debug.WriteLine("-- jest: " + imDir+ " AccessDirectory: "+ Tools.AccessDirectory(imDir)
                            + "AtrDir: "+ Tools.AtrDir(imDir)
                            );*/
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
        //Debug.WriteLine("-- jest: " + path);
        var imFiles = Directory.EnumerateFiles(path);
        FileInfo finfo;
        string ext, name;
        foreach (var imFile in imFiles.Select((value, i) => (value, i)))
        {
            /*
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
            });*/
            AddFileToDB(imFile.value, path);

            //var LX = BrokerDB.GetFiles(BrokerDB.GetTreeDBName(LActiveTreeModelItem));

        }
        List<FilesIO> files = BrokerDB.GetFiles(BrokerDB.GetTreeDBName(LActiveTreeModelItem));
        if (files != null)
        {
            FilesList.Clear();
            
            //FilesList = new ObservableCollection<FilesIO>(files);// to nie działa tak jakby  było potrzebne odświerzeniz einterfejsu
            files.ToList().ForEach(FilesList.Add);

        }
        /*
        Application.Current.Dispatcher.BeginInvoke(() =>
        {
            List<FilesIO> files = BrokerDB.GetFiles(BrokerDB.GetTreeDBName(LActiveTreeModelItem));
            if (files != null)
                FilesList = new ObservableCollection<FilesIO>(files);
        });*/

    }


    /// <summary>
    /// dodaje wybrany plik do bazy
    /// </summary>
    /// <param name="pathFile">ścieżka z plikiem</param>
    /// <param name="path">ścieżka bez pliku</param>
    /// <returns>czy dodano plik?</returns>
    public bool AddFileToDB(string pathFile, string path)
    {
        //var imFiles = Directory.EnumerateFiles(path);
        FileInfo finfo = new FileInfo(pathFile);
        string Ext, Name;
        Ext = System.IO.Path.GetExtension(pathFile);
        Name = System.IO.Path.GetFileName(pathFile);
        string PAthX = System.IO.Path.GetDirectoryName(pathFile);
        //Debug.WriteLine("path to the file: " + PAthX);
        //if (RActiveTreeModelItem == null)
         //   Debug.WriteLine("nie ma RBM");
        string DBName = BrokerDB.GetTreeDBName(LActiveTreeModelItem);
        //dalej dodawannie katalogu do DB o ile go nie ma
        // ... zwraca id katalogu i jest dodawany plik o ile go nie ma 
        //int idDirectory = BrokerDB.AddDirectory(path, DBName);
        //Debug.WriteLine("DBName: " + DBName);
        BrokerDB.AddFile(pathFile, DBName);

        return false;
    }

    /// <summary>
    /// wywołuje okno pomocnicze do wyboru plików
    /// </summary>
    /// <param name="path"></param>
    public async void OpenWindowPath(string path)
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
    private async Task FilesFindWindow(string path)
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

    #endregion Files

}
