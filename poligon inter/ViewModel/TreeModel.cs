using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.Input;


namespace poligon_inter.ViewModel;


public partial class TreeModel<T1> :ObservableObject
{

    public TreeModel()
    {
        this.IsSelected = false;
        _children = new ObservableCollection<TreeModel<T1>>();
    }

    #region fields
    [ObservableProperty]
    [Column("Id")]
    private int _Id;
    [ObservableProperty]
    [Column("ParentID")]
    public int _ParentID; 
    [ObservableProperty]
    private TreeModel<T1> _Parent;
    [ObservableProperty]
    protected ObservableCollection<TreeModel<T1>> _children;
    [ObservableProperty]
    private T1 _selectedValue;
    [ObservableProperty]
    private string _Name;
    [ObservableProperty]
    private bool _isSelected;
    [ObservableProperty]
    private bool _isExpanded;
    [ObservableProperty]
    private bool _isRightSelected;
    #endregion fields

    #region properties
    


    /*
    public IEnumerable<TreeModel<T1>> Children
    {
        get { return _children; }
        //set { _children = value}
    }
    */




    #endregion properties

    #region methods

    public override string ToString()
    {
        return this._Name;
    }

    public void AddChild(TreeModel<T1> child)
    {
        child._Parent = this;
        this._children.Add(child);
    }

    #endregion methods


    #region static methods
    // metody statyczne można przenieść do innego obiektu, tu raczej nie mają sensu 
    // operują na nim ale się do niego nie odwołują
    public static TreeModel<T1> GetNodeById(T1 id, IEnumerable<TreeModel<T1>> nodes)
    {
        foreach (var node in nodes)
        {
            if (node.SelectedValue.Equals(id))
                return node;

            var foundChild = GetNodeById(id, node._children);
            if (foundChild != null)
                return foundChild;
        }
        return null;
    }

    public static TreeModel<T1> GetSelectedNode(IEnumerable<TreeModel<T1>> nodes)
    {
        foreach (var node in nodes)
        {
            if (node.IsSelected)
                return node;

            var selectedChild = GetSelectedNode(node._children);
            if (selectedChild != null)
                return selectedChild;
        }

        return null;
    }

    public static void ExpandParentNodes(TreeModel<T1> node)
    {
        if (node._Parent != null)
        {
            node._Parent.IsExpanded = true;
            ExpandParentNodes(node._Parent);
        }
    }

    public static void ToggleExpanded(IEnumerable<TreeModel<T1>> nodes, bool isExpanded)
    {
        foreach (var node in nodes)
        {
            node.IsExpanded = isExpanded;
            ToggleExpanded(node._children, isExpanded);
        }
    }

    #endregion static methods

}

public class TreeModel : TreeModel<Guid>
{
}
/*
public partial class Category : ObservableObject
{
    public Category()
    {
        SubCategories = new ObservableCollection<Category>();
    }

    [Column("Id")]
    public int Id { get; set; }

    [StringLength(100)]
    public string Name { get; set; }

    [Column("ParentID")]
    public int? ParentID { get; set; }

    [ObservableProperty]
    [ForeignKey("ParentID")]
    private ObservableCollection<Category> _SubCategories;// { get; set; }
}
*/
