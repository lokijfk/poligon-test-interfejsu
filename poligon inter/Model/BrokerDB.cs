using Dapper;
using MaterialDesignThemes.Wpf;
using Microsoft.Data.Sqlite;
using poligon_inter.ViewModel;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows;

using String = System.String;

namespace poligon_inter.Model;

//do przerobienia !!
//ma zwracać orginalną baze danych i metody mają na niej pracować
//to dopracować jako egzemplaż bazy a dorobić Broker (provider?) rzeby obsłuzyć kilka baz na raz
//może to przerobić na broker ??
//obiekt bazy jest obiejtem z biblioteki a tu będą zbierane, reszta chyba jest
// tylko obsługękatalogów wypchnąć na zewnątrz
public class BrokerDB
{

    //private SqliteConnection s_conn;


    #region BrokerDB
    // to pozostaje prywatne odwołąnie się do bazy danych powinno nastąpić poprzez
    // podanie elementu lub nazwy bazy danych a nie do elementu con
    // dostę do danych następuje poprzez podanie zaznaczonego elementu z którego jest wybierana baza
    // w metodzie zwracającej określony zestaw danych
    // Broker zawiera wszystkie niezbędne kody SQL, co umożliwia w razie potrzeby zmianę bazy
    private Dictionary<string, SqliteConnection>? DB;
    //to aktualnie wybrana baza danych, wymagane jest sprawdzanie czy nie jest null
    private SqliteConnection? s_conn = null;
    public BrokerDB()
    {
        // to będzie konstruktor
        // ma przeszukać katalog z bazami i podłączyć bazy które tam znajdzie 
        // do słownika
        string path = Tools.GetUserAppDataPath;
        DB = new();
        if (Directory.Exists(path))
        {
            string[] files = Directory.GetFiles(path, "*.db");
            //SqliteConnection conn;
            //string dbName = string.Empty;

            foreach (string file in files)
            {
                //conn = new SqliteConnection( "Data Source=" + file);
                //int start = file.LastIndexOf("\\") + 1;
                //int len = file.Length - start - 3;
                //dbName = file.Substring(start,len);
                //tu powinno być sprawdzenie nazy bazy w bazie
                //jak string.empty to dopiero kombinowanie z nazwy pliku
                var dbName = file.Substring(file.LastIndexOf("\\") + 1,
                    (file.Length - (file.LastIndexOf("\\") + 1)) - 3);
                //Debug.WriteLine("dbname: " +file+ " , "+dbName);
                DB[dbName] = new SqliteConnection("Data Source=" + file);
                //DB[dbName].Open();
            }
        }
    }



    #endregion

    //dotyczy wybranej bazy dabch, zastanawiam sięjak to rozwiązać, może zrobić metody statyczne ??
    //cholera wie
    private string DBName
    {
        get => s_conn != null ? s_conn.QuerySingle<string>(@"SELECT wartosc FROM slowniki Where nazwa = 'DBname'") : "";
        set
        {
            if ((s_conn != null) && (value != null))
                s_conn.Execute(@"INSERT INTO slowniki (nazwa,wartosc) values ('DBname',@value)", new { value });
        }
    }

    //broker
    private void CreateDirX(string path)
    {
        //string dir = ;
        //MessageBox.Show(dir);
        Directory.CreateDirectory(path.Substring(0, path.LastIndexOf('\\')));
    }



    private void DbuildDb(string name = "")
    {
        //var param = "jakaś baza";
        if (s_conn != null)
        {
            //to katalogi w których są pliki dodane do bazy
            s_conn.Execute("CREATE TABLE IF NOT EXISTS katalogi (path , Id  INTEGER PRIMARY KEY)");
            // pliki dodane do bazy
            s_conn.Execute("CREATE TABLE IF NOT EXISTS pliki (nazwa varchar(255),rozszezenie varchar(5), id_katalogu, usuniety bool, MD5 , Id  INTEGER PRIMARY KEY)");
            // słowniki to wartości różne przechowywane w jednej tabeli, ale nie jest ich aż tyle żeby tworzyć osobną tabele dla nich
            s_conn.Execute("CREATE TABLE IF NOT EXISTS slowniki (nazwa,wartosc , Id  INTEGER PRIMARY KEY)");
            //kategorie to foldery w drzewie, ple view odpowiada za widok jaki się wyświetli po kliknięciu LPM            
            s_conn.Execute("CREATE TABLE IF NOT EXISTS Categories ( Name ,view , ParentID  INTEGER NOT NULL, Id    INTEGER NOT NULL UNIQUE, PRIMARY KEY(Id AUTOINCREMENT))");
            s_conn.Execute("INSERT INTO slowniki (nazwa,wartosc) values ('DBname',@name)", new { name });
        }
    }

    private string GetTreeDBName(TreeModel<Guid> SelectedItem)
    {
        if (SelectedItem != null)
        {
            if (SelectedItem.Parent != null)
            {
                var s1 = SelectedItem.Parent;
                while (s1.Parent != null)
                {
                    s1 = s1.Parent;
                }
                return s1.Name;
            }
            else return SelectedItem.Name;
        }
        return string.Empty;
    }

    public void AddDB(string name)
    {
        string path = Tools.GetUserAppDataPath;
        if (DB == null) DB = new();
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        DB[name] = new SqliteConnection("Data Source=" + path + "\\" + name + ".db");
        s_conn = DB[name];
        DbuildDb(name);
    }
    public int AddCategory(string category, TreeModel<Guid> SelectedItem)
    {   //brak sprawdzania czy w głównym katalogu nie ma już takiej samej bazy

        //throw new Exception("nie wybrano bazy");
        s_conn = DB[GetTreeDBName(SelectedItem)];

        s_conn.Execute("insert into Categories (Name,ParentID) values (@category,@id)",
                            new { category, id = SelectedItem.Id });
        Int64 LastRowID64 = s_conn.QuerySingle<Int64>("select last_insert_rowid()");

        s_conn = null;
        return (int)LastRowID64;
    }

    public void DeleteCategory(TreeModel<Guid> SelectedItem)
    {

        //throw new Exception("nie wybrano bazy");
        s_conn = DB[GetTreeDBName(SelectedItem)];
        s_conn.Execute("Delete From Categories WHERE id = @id", new { id = SelectedItem.Id });
        s_conn = null;
    }

    #region Tree
    public ObservableCollection<TreeModel>? GetTree()
    {
        ObservableCollection<TreeModel> Tree = new();
        TreeModel branch = null;
        foreach (KeyValuePair<string, SqliteConnection> con in DB)
        {
            if (con.Value != null)
            {
                //con.Value.Open();
                branch = new TreeModel { Name = con.Key, IsExpanded = true, Parent = null };
                // zwraca tree a nie tree<Guid>, jakby się dało to przerobić  bo wkuża okropnie
                s_conn = con.Value;
                branch.Children = GetTreBranch();
                if ((branch.Children != null) && (branch.Children.Count > 0))
                    foreach (var child in branch.Children)
                        child.Parent = branch;
                Tree.Add(branch);
                if (con.Value == null) Debug.WriteLine("GetTreBranch is null");
            }
        }
        s_conn = null;
        return Tree;
    }

    public ObservableCollection<TreeModel<Guid>>? GetTreBranch()
    {
        //_con można zrobić pole statyczne do którego będzie przypisana aktualnie wybrana baza
        // unikamy w ten sposób przekazywania tej bazy między metodami
        ObservableCollection<TreeModel<Guid>> Tree = null;
        //to i tak jest na początek później w planach jest zmiana tego na jakiś obiekt
        if (s_conn != null)
            Tree = new ObservableCollection<TreeModel<Guid>>(
            s_conn.Query<TreeModel<Guid>>("SELECT * FROM Categories WHERE ParentID == 0", new DynamicParameters()));
        foreach (var item in Tree)
        {
            //item.IsExpanded = false;
            item.IsExpanded = true;
            item.IsSelected = false;
            LoadSubCategories(item);
        }
        return Tree;
    }
    /*
    public ObservableCollection<TreeModel>? GetTreBranch(IDbConnection _con)
    {
        ObservableCollection<TreeModel> Tree = null;
        //to i tak jest na początek później w planach jest zmiana tego na jakiś obiekt
        if (_con != null)
                Tree = new ObservableCollection<TreeModel>(
                _con.Query<TreeModel>("SELECT * FROM Categories WHERE ParentID == 0", new DynamicParameters()));
                foreach (var item in Tree)
                {
                    //item.IsExpanded = false;
                    item.IsExpanded = true;
                    item.IsSelected = false;
                    LoadSubCategories(_con,item); 
                }            
        return Tree;
    }
    */
    private void LoadSubCategories(TreeModel<Guid> item)
    {
        // to jest dzięki jakiemuś frameworkowi ale nie wiem jakiemu
        //item.SubCategories = new ObservableCollection<Category>(s_conn.Category.Where(x => x.ParentID == item.Id));
        //to zamienić na uruchomioną  bazę !!!
        item.Children = new ObservableCollection<TreeModel<Guid>>(
        s_conn.Query<TreeModel<Guid>>("select * from Categories where ParentID == @Id", item));//!!!
                                                                                               //item.AddChild(sub);
        foreach (var subitem in item.Children)
        {
            subitem.Parent = item;
            subitem.IsExpanded = true;
            subitem.IsSelected = false;
            LoadSubCategories(subitem);
        }
    }

    #endregion Tree
    public void AddPassword(string password)
    {
        string pass = Tools.CalculateMD5Sting(password);
        var command = s_conn.CreateCommand();
        command.CommandText = String.Format(@"insert into slowniki (nazwa,wartosc) values (pass,'{0}')", pass);
        command.ExecuteNonQuery();
    }

    #region Files
    public void addFile(string nazwa, string path, string MD5 = "")
    {
        //metoda do przepisania na nowo
        string id = string.Empty;
        var command = s_conn.CreateCommand();
        command.CommandText = String.Format("SELECT id FROM katalogi WHERE path='{0}'", path);
        SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())// po co ta pętla ??
        {
            id = reader["id"].ToString();
            //MessageBox.Show("id: " + reader["id"]);
        }
        reader.Close();
        if ((id == string.Empty) || (id == null))
        {
            command.CommandText = String.Format(@"insert into katalogi (path) values ('{0}')", path);
            command.ExecuteNonQuery();
            command.CommandText = String.Format("SELECT id FROM katalogi WHERE path='{0}'", path);
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                id = reader["id"].ToString();
                //MessageBox.Show("id: " + reader["id"]);
            }
            reader.Close();
        }


        command.CommandText = String.Format(@"insert into pliki (nazwa,id_katalogu,usuniety,rozszezenie,MD5) values ('{0}','{1}',false,'','{2}')", nazwa, id, MD5);
        var ile = command.ExecuteNonQuery();
        if (ile < 1) MessageBox.Show(" chyba coś poszło nie tak, ilość dodana: " + ile);
    }

    #endregion Files



}

