using Dapper;
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
public class BrokerDB : IDisposable
{

    private SqliteConnection s_conn;

    
    #region BrokerDB
    // to pozostaje prywatne odwołąnie się do bazy danych powinno nastąpić poprzez
    // podanie elementu lub nazwy bazy danych a nie do elementu con
    // dostę do danych następuje poprzez podanie zaznaczonego elementu z którego jest wybierana baza
    // w metodzie zwracającej określony zestaw danych
    // Broker zawiera wszystkie niezbędne kody SQL, co umożliwia w razie potrzeby zmianę bazy
    private Dictionary<string, SqliteConnection>? DB;

    public BrokerDB()
    {
        // to będzie konstruktor
        // ma przeszukać katalog z bazami i podłączyć bazy które tam znajdzie 
        // do słownika
        string path = Tools.GetUserAppDataPath;
        string[] files = Directory.GetFiles(path, "*.db");
        SqliteConnection conn;
        string dbName = string.Empty;
        foreach (string file in files)
        {
            
            conn = new SqliteConnection( "Data Source=" + file);
            dbName = file.Substring(file.LastIndexOf("\\")+1,file.LastIndexOf("\."));
            Debug.WriteLine("dbname: " +file+ " , "+dbName);

        }
    }
        string[] files = Directory.GetFiles(path, "*.db");
        //SqliteConnection conn;
        string dbName = string.Empty;
        DB = new();
        foreach (string file in files)
        {
            
            //conn = new SqliteConnection( "Data Source=" + file);
            int start = file.LastIndexOf("\\") + 1;
            int len = file.Length - start - 3;
            dbName = file.Substring(start,len);
            Debug.WriteLine("dbname: " +file+ " , "+dbName);
            DB[dbName] = new SqliteConnection("Data Source=" + file);
        }
    }



    #endregion


    private string DBName
    {
        get =>  s_conn.Query<string>(@"SELECT wartosc FROM slowniki Where nazwa = 'DBname'").FirstOrDefault();
        set => s_conn.Query<string>(@"INSERT INTO slowniki (nazwa,wartosc) values ('DBname',@value)",new { value });
    }

    //broker
    private void  CreateDirX(string patch)
    {
        string dir = patch.Substring(0, patch.LastIndexOf('\\'));
        //MessageBox.Show(dir);
        Directory.CreateDirectory(dir);
    }

    #region konstruktorySQL
    public void DBSQLite(string path, string dbase)
    {
        //yyy robić pusty ??
        //bo jest potrzebny taki w którym wskazujemy bazę 
        // i by się przydała jakaś zmienna wskazująca  na katalog z danymi programu
        //path powinno być podawane w brokerze chyba!!!
        string CS;
        path = path + "\\" + dbase + ".db";
        CS = "Data Source=" + path ;
        if (!File.Exists(path)) CreateDirX(path);
        s_conn = new SqliteConnection(CS);
        s_conn.Open();

        DbuildDb(dbase);

    }

    public void DBSQLite( string dbase)
    {
        //yyy robić pusty ??
        //bo jest potrzebny taki w którym wskazujemy bazę 
        // i by się przydała jakaś zmienna wskazująca  na katalog z danymi programu
        string CS;
        //to powinno być z brokera a nie z tools, tools wywalamy jak się da
        string path = Tools.GetUserAppDataPath + "\\" + dbase + ".db";
        CS = "Data Source=" + path;
        if (!File.Exists(path)) CreateDirX(path);
        s_conn = new SqliteConnection(CS);
        s_conn.Open();

        DbuildDb(dbase);

    }
    public void DBSQLite()
    {
        //yyy robić pusty ??
        //bo jest potrzebny taki w którym wskazujemy bazę 
        // i by się przydała jakaś zmienna wskazująca  na katalog z danymi programu
        string CS;
        string path = Tools.GetUserAppDataPath + "\\" + Tools.GetProjectName + ".db";
        CS = "Data Source=" + path;
        if (!File.Exists(path)) CreateDirX(path);
        s_conn = new SqliteConnection(CS);
        s_conn.Open();

        DbuildDb("Poligon 0.01");

    }
    #endregion konstruktory

    private void DbuildDb(string name = "")
    {
        // to ma tworzyć automatycznie tabele w nowej bazie lub dodawać nowe jak nie istnieją
        // na zasadzie create if not exist
        //s_conn.Open();
        //var param = "jakaś baza";
        var command = s_conn.CreateCommand();
        command.CommandText = "CREATE TABLE IF NOT EXISTS katalogi (path , Id  INTEGER PRIMARY KEY)";
        command.ExecuteNonQuery();
        command.CommandText = "CREATE TABLE IF NOT EXISTS pliki (nazwa varchar(255),rozszezenie varchar(5), id_katalogu, usuniety bool, MD5 , Id  INTEGER PRIMARY KEY)";
        command.ExecuteNonQuery();
        command.CommandText = "CREATE TABLE IF NOT EXISTS slowniki (nazwa,wartosc , Id  INTEGER PRIMARY KEY)";
        command.ExecuteNonQuery();
        //Debug.WriteLine("dbname: " + DBName);
        string dbn = DBName;
        if ((dbn == String.Empty) || (dbn == null))DBName = name;

        // dodać function- to ma być funkcja jaką pełni
        command.CommandText = "CREATE TABLE IF NOT EXISTS Categories ( Name  , ParentID  INTEGER NOT NULL, Id    INTEGER NOT NULL UNIQUE, PRIMARY KEY(Id AUTOINCREMENT))";
        //CREATE TABLE "Categories" ( "Name"  , "ParentID"  INTEGER NOT NULL, "Id"    INTEGER NOT NULL UNIQUE, PRIMARY KEY("Id" AUTOINCREMENT) );
        command.ExecuteNonQuery();
        /*
        command.CommandText = String.Format(@"insert into kategorie (nazwa,id_rodzica,wartosc) values ('DBname','null','{0}')", name);
        command.ExecuteNonQuery();
        */
    }

    public SqliteConnection GetDB { 
        get  {
            //do przerobienia
            if (s_conn == null)
            {
                string CS = "Data Source=" + Tools.GetUserAppDataPath + "\\" + Tools.GetProjectName + ".db";
                s_conn = new SqliteConnection(CS); 
                s_conn.Open();
            }
            return s_conn;
        } 
    }
    public int AddCategory(string category, int parent=0)
    {   //brak sprawdzania czy w głównym katalogu nie ma już takiej samej bazy
        var command = s_conn.CreateCommand();
        command.CommandText = String.Format(@"insert into Categories (Name,ParentID) values ('{0}','{1}')",
                            category,parent);
        command.ExecuteNonQuery();
        command.CommandText = "select last_insert_rowid()";
        Int64 LastRowID64 = (Int64)command.ExecuteScalar();
        return (int)LastRowID64;
        //int LastRowID = (int)LastRowID64;
        //return LastRowID;
        //return 0;
    }

    public void DeleteCategory(int id)
    {
        var command = s_conn.CreateCommand();
        //DELETE FROM Customers WHERE CustomerName = 'Alfreds Futterkiste';
        command.CommandText = String.Format(@"Delete From Categories WHERE id ='{0}'",
                            id);
        command.ExecuteNonQuery();
    }

    #region Tree
    public ObservableCollection<TreeModel>? GetTree()
    {

        ObservableCollection<TreeModel> Tree = null;
        //to i tak jest na początek później w planach jest zmiana tego na jakiś obiekt
        //przechowujący kilka baz danych i do nich się odnoszącego
        //ogólnie to trzeba to przenieść do obektu odpowiedzialnego za bazę danych
        if (File.Exists(Tools.GetUserAppDataPath + "\\" + Tools.GetProjectName + ".db"))
            using (IDbConnection con = new SqliteConnection("Data Source=" + Tools.GetUserAppDataPath + "\\" + Tools.GetProjectName + ".db"))
            {
                Tree = new ObservableCollection<TreeModel>(
                con.Query<TreeModel>("SELECT * FROM Categories WHERE ParentID == 0", new DynamicParameters()));
                foreach (var item in Tree)
                {
                    //item.IsExpanded = false;
                    item.IsExpanded = true;
                    item.IsSelected = false;
                    LoadSubCategories(item); 
                }
            }
        return Tree;
    }

    private void LoadSubCategories(TreeModel<Guid> item)
    {
        // to jest dzięki jakiemuś frameworkowi ale nie wiem jakiemu
        //item.SubCategories = new ObservableCollection<Category>(s_conn.Category.Where(x => x.ParentID == item.Id));
        //to zamienić na uruchomioną  bazę !!!
        using (IDbConnection con = new SqliteConnection("Data Source=" + Tools.GetUserAppDataPath + "\\" + Tools.GetProjectName + ".db"))
        {

            item.Children = new ObservableCollection<TreeModel<Guid>>(
            con.Query<TreeModel<Guid>>("select * from Categories where ParentID == @Id", item));//!!!
            //item.AddChild(sub);
            foreach (var subitem in item.Children)
            {
                subitem.Parent = item;
                subitem.IsExpanded = true;
                subitem.IsSelected = false;
                LoadSubCategories(subitem);
            }
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
        string id = string.Empty;
        var command = s_conn.CreateCommand();
        command.CommandText = String.Format("SELECT id FROM katalogi WHERE path='{0}'", path);
        SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
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

    public void addFile(string CombinePath)
    {

    }


    public int FileExists(string nazwa)
    {

        return 0;
    }

    public bool CatalogExists(string path)
    {
        return false;
    }

    public bool MD5Exists(string MD5)
    {
        return false;
    }

    #endregion Files
    
    
    public void Dispose()
    {
        ((IDisposable)GetDB).Dispose();
    }
}

