using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using System.Collections.ObjectModel;
using poligon_inter.View;
using System.Diagnostics;
using poligon_inter.Model;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;



namespace poligon_inter.ViewModel;


public partial class MainWindowViewModel: ObservableObject
{

    #region pola
    private DBSQLite? DB = null; // mieić nazwę obiektu na SQLBroker będzie mniej myląda
   
    public ObservableCollection<TreeModel>? Tree { get; set; }
    //public ObservableCollection<ContextMenuTreeView> TVCommandList { get; set; }

    private readonly IniBroker? iniFile = null;
    //public MainWindowViewModel(IniFile iniFile) => this.iniFile = iniFile;

    //private string LeftName = "empty";
    private TreeModel<Guid>? RActiveTreeModelItem = null;
    #endregion pola

    public MainWindowViewModel()
    {
        
        using (var edb = new DBSQLite())//(?) a nie lepiej normalnie to ustawić, aaaa bo to tu potrzebne jest tylko na chwilę 
        {
            Tree = edb.GetTree();
        }
        iniFile = new IniBroker();
        
        //trzeba zbudować menu item zależne od tego co było kliknięte prawym przyciskiem
        // ale nie wiem czy przypadkiem w oknie nie będzie obsługiwane tylko drzewo i inne menu może nie być potrzebne
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
        #region Testy
        /*
        string typ, kom, sname;
        if (t != null)
        {
             typ = t.GetType().ToString();

        }
        else
        {
            typ = "null";
        }

        var selectedNode = TreeModel.GetSelectedNode(this.Tree);
        if (selectedNode != null)
        {
                // trzeba zaznaczyć żeby było znajdowane, a jak zrobić żeby szło bez zaznaczenia?
                // polecenia trasowane ??
                sname = selectedNode.Name;
                kom = " wybrano node: "+sname;
                //MessageBox.Show("kliknięto -dodaj: " + t.GetType().ToString() + " wybrano node: " + selectedNode.Name);
        }
        else
        {
                sname = "null";
                kom = " brak zaznaczonego elementu";
            //MessageBox.Show("kliknięto -dodaj: " + t.GetType().ToString() + " brak zaznaczonego elementu" );
        }
        if (LeftName != string.Empty)
        {
            kom += " klik: "+ LeftName;//ogólnie działa ale trzeba jakos przesłać jeszcze obiekt na który kliknięto
        }

        MessageBox.Show("kliknięto -dodaj: " + typ + kom);
        */
        #endregion Testy

        if (RActiveTreeModelItem != null)
        {
            //wywołać okno do wpisania nazwy, ale czy to nie trzeba będzie zmienić metodę na async?
            RActiveTreeModelItem.AddChild(new TreeModel<Guid> { Name = "AAAXXd", IsExpanded = true, Parent = RActiveTreeModelItem });
            //dodać do bazy
        }
    }



    [RelayCommand]
    private void RemoveFolder(object t)
    {
        // Value="{Binding Path=PlacementTarget.SelectedItem, RelativeSource={RelativeSource AncestorType={x:Type ContextMenu}}}"/>
    }

    [RelayCommand]
    private void TreeModelRBMClick(object parameter)
    {
       
        //LeftName = "klik: "+ parameter.GetType().ToString();

        /*
         * if (o is string s) {...}

        // csharp_style_pattern_matching_over_as_with_null_check = false
        var s = o as string;
        if (s != null) {...}
        */
        /*
        TreeModel p = parameter as TreeModel;
        if (p != null)
        {
            LeftName += " name: " + p.Name;
        }
        else*/

        if (parameter is TreeModel<Guid>)
        {
            TreeModel<Guid>? c = parameter as TreeModel<Guid>;
            if (c != null)
            {
                RActiveTreeModelItem = c;
                //LeftName += " name: " + c.Name;
                //c.AddChild(new TreeModel<Guid> { Name = "AAAXXd", IsExpanded = true, Parent = c });//działa
            }
            else
            {
                RActiveTreeModelItem = null;
               // LeftName += " name: none";
            }
        }
    }
    private bool CanExecuteEx(object parameter)
    {
        return true;
    }

    #endregion RelayCommand


    #region ObservableProperty

    [ObservableProperty]
    private double _MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;

    //[ObservableProperty]
    //private bool _isExpand = false;// działa ale trzeba powiązać z ini

    #region WindowState
    
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

    [ObservableProperty]
    private bool _IsDialogOpen;


    #endregion ObservableProperty

    #region RelayCommand Task

    [RelayCommand]
    private async Task CreateDB()
    {
         
        string x = await ShowDialog(new SimpledialogViewModel {
            Name = string.Empty, WindowName = "Utwórz nową bazę danych", Hint = "Nazwa bazy" });
        /*
         * do przebudowania, wszystko ma być w jednym pliku a tzw. baza ma być jako sekcja główna
         * reszta  to podsekcje
         * 
         * za jakiś czas jak to ogarnę to zrobię osobne bazy danych zarzane przez odpowiedni objekt
         * na osobnych plikach
         * 
         * na razie zrobić odczytywanie drzewa z bazy i dodawanie sekcji do bazy i do drzewa
         */
        if (x != string.Empty)
        {
            Debug.WriteLine("nazwa jest : " +x);
            //tu robimy bazę danych, sprawdzić wcześniej czy nie ma juz takiej

            using (var edb = new DBSQLite())
            {
                int id = edb.AddCategory(x);
                Tree.Add(new TreeModel { Id = id, Name = x });
                //Tree = edb.GetTree();
                
            }

        }
        else await ShowMessage(new MessageBoxXViewModel { Message = "nie podałeś nazy, bazy nie utworzono" });

        // Debug.WriteLine("Dialog was closed, the CommandParameter used to close it was: " + x);
    }

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
    private void ClosedEventHandler(object sender, DialogClosedEventArgs eventArgs)
    {
        //to jest opcjonalne, na razie zostawiam może jeszcze wykozystam
        Debug.WriteLine("You can intercept the closed event here (1)." + eventArgs.Parameter );
    }

    #endregion RelayCommand Task

}
