using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using System.Collections.ObjectModel;
using poligon_inter.View;
using System.Diagnostics;
using poligon_inter.Model;
using System.Windows;
using Microsoft.Win32;

namespace poligon_inter.ViewModel;

public partial class MainWindowViewModel: ObservableObject
{

    #region pola
    private DBSQLite? DB = null; // mieić nazwę obiektu na SQLBroker będzie mniej myląda
   
    public ObservableCollection<TreeModel>? Tree { get; set; }
    //public ObservableCollection<ContextMenuTreeView> TVCommandList { get; set; }

    private readonly BrokerIni? iniFile = null;
    //public MainWindowViewModel(IniFile iniFile) => this.iniFile = iniFile;

    private TreeModel<Guid>? RActiveTreeModelItem = null;
    [ObservableProperty]
    private TreeModel<Guid>? _lActiveTreeModelItem = null;

    //private IFileDialogService _iFileDialog;
    #endregion pola
    #region ObservableProperty

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
        
        using (var edb = new DBSQLite())
        {
            Tree = edb.GetTree();
        }
        iniFile = new BrokerIni();
        SelectedViewModel = new Welcome();
        ContentMax = CurMainWindowState == WindowState.Normal ? "1" : "2";

        BrokerDB BrokDB = new();

        //BrokerDB BrokDB = new();

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

    #region Private methods


    #endregion Private methods

    #region RelayCommand

    [RelayCommand]
    private void AddFolder(object t)
    {

        if (RActiveTreeModelItem != null)
        {
            //wywołać okno do wpisania nazwy, ale czy to nie trzeba będzie zmienić metodę na async?
            RActiveTreeModelItem.AddChild(new TreeModel<Guid> { Name = "AAAXXd", IsExpanded = true, Parent = RActiveTreeModelItem });
            //dodać do bazy
            AddCategory("AAAXXd", RActiveTreeModelItem.Id);
            RActiveTreeModelItem = null;
        }
        else
        {
            //TreeModel<Guid>? SelectedItem = TreeModel.GetSelectedNode(Tree);
            // to jakoś poprawić
            //Debug.WriteLine("Znalazłem Element: ");
            if ((t is TreeModel<Guid>) && (t != null))
            {
                TreeModel<Guid> SelectedItem = t as TreeModel<Guid>;
                SelectedItem.AddChild(new TreeModel<Guid> { Name = "AAAXXd", IsExpanded = true, Parent = SelectedItem });
                AddCategory("AAAXXd", SelectedItem.Id);
            }
            else if (LActiveTreeModelItem != null)
            {
                TreeModel<Guid>? SelectedItem = LActiveTreeModelItem as TreeModel<Guid>;
                SelectedItem.AddChild(new TreeModel<Guid> { Name = "AAAXXd", IsExpanded = true, Parent = SelectedItem });
                AddCategory("AAAXXd", SelectedItem.Id);
            }        
           
        }
    }


    private void AddCategory(string name, int parent)
    {
        using (var db = new DBSQLite())
        {
            int id = db.AddCategory(name, parent);
            
        }
    }

    private void RemoveCategory(int id)
    {
        using (var db = new DBSQLite())
        {
           db.DeleteCategory(id);

        }
    }

    [RelayCommand]
    private void RemoveFolder(object t)
    {

        if (RActiveTreeModelItem != null)
        {
            TreeModel<Guid>? ParentItem = RActiveTreeModelItem.Parent;
           
            if ((ParentItem != null) &&(ParentItem.Children.Count > 0))
            {
                ParentItem.Children.Remove(RActiveTreeModelItem);
                // dodać usuwanie elementu zbazy, ale co z zależnościami i kaskadowością?
            }
            else
            {
                //tu dodać okno z pytaniem czy na pewno bo to główna kategoria i można stracić dane
                //jak będzie falxe w odpowiedzi to wtedy zrobić return z funkcji żeby nie wywalało z bazy
                // i zmienić rodzaj metody na task
                Tree.Remove(RActiveTreeModelItem as TreeModel);
            }
            RemoveCategory(RActiveTreeModelItem.Id);
            
            RActiveTreeModelItem = null;
        }

    }

    [RelayCommand]
    private void TreeModelLBMClick(object parameter)
    {
        if ((parameter is TreeModel<Guid>) && (parameter != null))
        {
            //działa tu zrobić wybór widoku dla klikniętego elementu
            //tu można dodać pole lokalne LActiveTreeModelItem
            // _ = MessageBox.Show("kliknięto: " + (t as TreeModel<Guid>).Name);
            TreeModel<Guid>? c = parameter as TreeModel<Guid>;
            LActiveTreeModelItem = c;
        }

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

        if ((parameter is TreeModel<Guid>) &&(parameter != null))
        {
            TreeModel<Guid>? c = parameter as TreeModel<Guid>;
            RActiveTreeModelItem = c;
        }
        else
        {
            RActiveTreeModelItem = null;
        }
    }



    #endregion RelayCommand




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

        //Debug.WriteLine("Dialog was closed, the CommandParameter used to close it was: " + (result ?? "NULL")+" - " + DC.Name);
        //to poniżej można przenieść do metody ClosedEventHandler
        Debug.WriteLine("result : " + (bool)result);
        if ((bool)result)
        {
            //return DC.Name;
            if (DC.Name != string.Empty)
            {
                //Debug.WriteLine("nazwa jest : " + DC.Name);
                //tu robimy bazę danych, sprawdzić wcześniej czy nie ma juz takiej

                using (var db = new DBSQLite())
                {
                    int id = db.AddCategory(DC.Name);
                    Tree.Add(new TreeModel { Id = id, Name = DC.Name });
                }
            }
        }     
        
    }

    
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
        
        if ((bool)result)
        {
            return DC.Name;
        }
            return "";        
    }
    #endregion okna testowe

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

}
