using Dapper;
using Microsoft.Data.Sqlite;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;



namespace poligon_inter.Model;

//do przerobienia !!
//ma zwracać orginalną baze danych i metody mają na niej pracować
//to dopracować jako egzemplaż bazy a dorobić Broker (provider?) rzeby obsłuzyć kilka baz na raz
//może to przerobić na broker ??
//obiekt bazy jest obiejtem z biblioteki a tu będą zbierane, reszta chyba jest
// tylko obsługękatalogów wypchnąć na zewnątrz
public class BrokerDB
{

    #region BrokerDB

    private Dictionary<string, SqliteConnection> DB = [];
    //to aktualnie wybrana baza danych, wymagane jest sprawdzanie czy nie jest null
    private SqliteConnection? s_conn = null;
    public BrokerDB()
    {
        string path = Tools.GetUserAppDataPath;
        //DB = [];
        if (Directory.Exists(path))
        {
            string[] files = Directory.GetFiles(path, "*.db");
            foreach (string file in files)
            {

                var dbName = file.Substring(file.LastIndexOf("\\") + 1,
                    (file.Length - (file.LastIndexOf("\\") + 1)) - 3);            
                DB[dbName] = new SqliteConnection("Data Source=" + file);                
            }
        }
    }



    #endregion

    // tu chodzi o nazwę bazy widzianą w drzewie a nie o nazwę pliku bazy
    private string DBName
    {
        get => s_conn != null ? s_conn.QuerySingle<string>(@"SELECT wartosc FROM slowniki Where nazwa = 'DBname'") : "";
        set
        {
            //tu należy sprawdzić czy nie ma przypadkiem już rekordu o takiej nazwi 
            //jak jest to robimy update a nie insert
            if ((s_conn != null) && (value != null))
                s_conn.Execute(@"INSERT INTO slowniki (nazwa,wartosc) values ('DBname',@value)", new { value });
        }
    }


    private void DbuildTableDb(string name)
    {
        //var param = "jakaś baza";
        if (s_conn != null)
        {
            //to katalogi w których są pliki dodane do bazy
            s_conn.Execute("CREATE TABLE IF NOT EXISTS Katalogi (path , Id  INTEGER PRIMARY KEY)");
            // pliki dodane do bazy
            s_conn.Execute("CREATE TABLE IF NOT EXISTS Pliki (nazwa varchar(255),rozszezenie varchar(5), id_katalogu, usuniety bool, MD5 , Id  INTEGER PRIMARY KEY)");
            // słowniki to wartości różne przechowywane w jednej tabeli, ale nie jest ich aż tyle żeby tworzyć osobną tabele dla nich
            s_conn.Execute("CREATE TABLE IF NOT EXISTS Slowniki (nazwa,wartosc , Id  INTEGER PRIMARY KEY)");
            //kategorie to foldery w drzewie, ple view odpowiada za widok jaki się wyświetli po kliknięciu LPM            
            s_conn.Execute("CREATE TABLE IF NOT EXISTS Categories ( Name ,View , ParentID  INTEGER NOT NULL, Id    INTEGER NOT NULL UNIQUE, PRIMARY KEY(Id AUTOINCREMENT))");
            s_conn.Execute("INSERT INTO Slowniki (nazwa,wartosc) values ('DBname',@name)", new { name });
            s_conn.Execute("INSERT INTO Categories (Name ,view , ParentID) values ('Pliki',@view,0)", new { view = "Files" });
            s_conn.Execute("INSERT INTO Slowniki (nazwa,wartosc) values ('extension',@name)", new { name = "*.avi,*.mp4,*.mkv" });
            s_conn.Execute("CREATE VIEW Files AS \r\nSELECT P.nazwa as Name, P.rozszezenie as Extension,P.id as id, K.path as Path, K.path||\"\\\"||P.nazwa as File\r\n FROM pliki AS P INNER JOIN katalogi AS K ON P.id_katalogu =K.id");
            /*
             * CREATE VIEW Files AS 
SELECT P.nazwa as Name, P.rozszezenie as Extension,P.id as id, K.path as Path, K.path||"\"||P.nazwa as File
 FROM pliki AS P INNER JOIN katalogi AS K ON P.id_katalogu =K.id 

            DROP VIEW IF EXISTS Files

            */
        }
    }

    public string GetTreeDBName(TreeModel SelectedItem)
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
                return s1.GetKeyDB;
            }
            else return SelectedItem.GetKeyDB;
        }
        return string.Empty;
    }

    public TreeModel? CreateDB(string name)
    {
        string path = Tools.GetUserAppDataPath;
        if (DB == null) DB = [];
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        //dodany do słownika
        DB[name] = new SqliteConnection("Data Source=" + path + "\\" + name + ".db");
        s_conn = DB[name];
        //budowa struktury
        DbuildTableDb(name);
        return GetTreeDB(name);

    }

    public void DeleteDB(TreeModel DBx)
    {
        //Debug.WriteLine("usuwamy bazę");
        if (DB == null) return;
        //Debug.WriteLine("usuwamy bazę 2");
        try
        {
            s_conn = DB[DBx.GetKeyDB];
            s_conn.Close();
            s_conn.Dispose();
            SqliteConnection.ClearAllPools();
            var dsource = s_conn.DataSource;
            s_conn = null;
            File.Delete(dsource);
            DB.Remove(DBx.GetKeyDB);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);//tu dodać wrzucanie do logów błędu
        }
    }

    public int AddCategory(string category, TreeModel SelectedItem)
    {
        if (DB == null) return -1;// to usunąć
        s_conn = DB[GetTreeDBName(SelectedItem)];// a tu sprawdzić czy istnieje
        int id = SelectedItem.Id;
        if (id == -1) id = 0;
        s_conn.Execute("insert into Categories (Name,ParentID) values (@category,@id)",
                            new { category, id });
        Int64 LastRowID64 = s_conn.QuerySingle<Int64>("select last_insert_rowid()");

        s_conn = null;
        return (int)LastRowID64;
    }

    public void DeleteCategory(TreeModel SelectedItem)
    {

        //throw new Exception("nie wybrano bazy");
        if (DB == null) return;
        if (SelectedItem.Parent != null)
        {
            s_conn = DB[GetTreeDBName(SelectedItem)];
            s_conn.Execute("Delete From Categories WHERE id = @id", new { id = SelectedItem.Id });
            s_conn = null;
        }
    }

    #region Tree


    public ObservableCollection<TreeModel>? GetTree()
    {
        ObservableCollection<TreeModel> Tree = [];        
        if (DB == null) return null;// to usunąć konstruktor buduje
        foreach (KeyValuePair<string, SqliteConnection> con in DB)
        {
            if (con.Value != null)
            {
                Tree.Add(item: GetTreeDB(con.Key));
                //Debug.WriteLine("Key: "+con.Key+" Value: "+con.Value);
            }
        }
        s_conn = null;//??
        return Tree;
    }

    /// <summary>
    /// zwraca drzewo z bazy o podanej nazwie o ile ta baza jest dołączona do słownika DB
    /// </summary>
    /// <param name="name">nazwa bazy</param>
    /// <returns>TreeModel - w raz z pod gałęziami</returns>
    private TreeModel GetTreeDB(string name)
    {
        TreeModel branch;    

        if (DB[name] != null)
        {
            s_conn = DB[name];
            //tu powinno być pobieranie nazwy z bazy
            string ViewName = this.DBName;
            branch = new TreeModel
            {
                Name = ViewName,
                IsExpanded = true,
                Children = GetTreeBranch(),
                GetKeyDB = name
            };
            if ((branch.Children != null) && (branch.Children.Count > 0))
                foreach (var child in branch.Children)
                    child.Parent = branch;

            s_conn = null;
            return branch;
        }
        return null;
    }

    // przejżeć obie matody naspokojnie i zrobić z nich jedną, są podobne
    private ObservableCollection<TreeModel> GetTreeBranch()
    {
        ObservableCollection<TreeModel> Tree;
        //to i tak jest na początek później w planach jest zmiana tego na jakiś obiekt
        if (s_conn != null)
        {
            Tree = [.. s_conn.Query<TreeModel>
                ("SELECT * FROM Categories WHERE ParentID == 0", new DynamicParameters())];
            foreach (var item in Tree)
            {
                //item.IsExpanded = false;
                item.IsExpanded = true;
                item.IsSelected = false;
                LoadSubCategories(item);
            }
            return Tree;
        }
        return [];
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
    private void LoadSubCategories(TreeModel item)
    {
        if (s_conn != null)
        {
            item.Children = [.. s_conn.Query<TreeModel>
                ("select * from Categories where ParentID == @Id", new { Id = item.Id })];//!!!
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
    /*
    public void AddPassword(string password)
    {
        string pass = Tools.CalculateMD5Sting(password);
        var command = s_conn.CreateCommand();
        command.CommandText = String.Format(@"insert into slowniki (nazwa,wartosc) values (pass,'{0}')", pass);
        command.ExecuteNonQuery();
    }
    */
    #region Files

    /// <summary>
    /// docelowo ma pobierać dane z vidoku Files i zwrcać w postaci listy 
    /// obiektów LilesIO
    /// </summary>
    public List<FilesIO>? GetFiles(string DBName, TreeModel SelectedItem = null)
    {
        try
        {
            if (DBName != string.Empty)
            {
                s_conn = DB[DBName];
            }
            else
            {
                s_conn = DB[GetTreeDBName(SelectedItem)];
            }

            List<FilesIO> x = [.. s_conn.Query<FilesIO>("Select * From Files", new DynamicParameters()) ];
            return x;
        }
        catch(Exception ex)
        {
            Debug.WriteLine(ex.Message);    
            return null;
        }

        //return null;
    }

    public void AddFile(string nazwa, string DBName)
    {   
        if ((string.IsNullOrWhiteSpace(DBName))&& (string.IsNullOrWhiteSpace(nazwa)))
        {
            Debug.WriteLine("DBName: " + DBName + " nazwa: " + nazwa + " is empty or null");
            return;
        }

        
        try
        {
            string PAthX = System.IO.Path.GetDirectoryName(nazwa);
            if (!string.IsNullOrWhiteSpace(PAthX))
            {
                int idDir = AddDirectory(PAthX, DBName);
                //Debug.WriteLine(" id katalogu: " + idDir);
                if (idDir > 0)
                {   
                    string Nazwa = System.IO.Path.GetFileName(nazwa);
                    string Ext = System.IO.Path.GetExtension(nazwa);
                    s_conn = DB[DBName];
                    int idFile = s_conn.QuerySingleOrDefault<int>("select Id from pliki where nazwa=@nazwa and id_katalogu=@idDir", new { nazwa = Nazwa,idDir });
                    //(nazwa varchar(255),rozszezenie varchar(5), id_katalogu, usuniety bool, MD5 , Id  INTEGER PRIMARY KEY)");
                    if (idFile <= 0)
                    {
                        s_conn.Execute("INSERT INTO pliki (nazwa ,rozszezenie , id_katalogu) values (@nazwa,@ext,@idDir)",
                            new { nazwa = Nazwa, ext = Ext, idDir });
                    }
                }
            }
            
        }
        catch(Exception ex)
        {
            Debug.WriteLine(ex.ToString());   
        }
        


    }

    public int AddDirectory(string Dir,string DBName)
    {

        //Debug.WriteLine("Dir: "+Dir+" DBName: "+DBName);
        if ((!string.IsNullOrWhiteSpace(DBName))&&(!string.IsNullOrWhiteSpace(Dir)))
        {
            try
            {
                s_conn = DB[DBName];
                int id = s_conn.QuerySingleOrDefault<int>("select Id from katalogi where path=@path", new { path = Dir });
                if (id > 0)
                {
                    return id;
                }
                else
                {
                    s_conn.Execute("insert into katalogi (path) values (@Dir)",
                                new { Dir });
                    Int64 LastRowID64 = s_conn.QuerySingle<Int64>("select last_insert_rowid()");
                    return (int)LastRowID64;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return -1;
            }
        }
        else
        {
            Debug.WriteLine("DBName is empty or null");
            return -1;
        }
    }

    #endregion Files



}
