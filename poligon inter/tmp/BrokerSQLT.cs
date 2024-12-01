

using Dapper;
using Microsoft.Data.Sqlite;
using poligon_inter.Model;
using poligon_inter.ViewModel;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;

namespace poligon_inter.tmp;

internal class BrokerSQLT
{

    private Dictionary<string, DBSQLite>? DB;

    //powinien być statyczny i zwracać ścieszkę do katalogu progrmu w profilu usera
    // jak nie ma katalogu to powinien go tworzyć
    private void CreateDirX(string patch)
    {
        string dir = patch.Substring(0, patch.LastIndexOf('\\'));
        //MessageBox.Show(dir);
        Directory.CreateDirectory(dir);
    }

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

}
