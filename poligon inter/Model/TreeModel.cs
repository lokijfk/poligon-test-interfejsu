using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace poligon_inter.Model;


public partial class TreeModel<T1> :ObservableObject
{

    public TreeModel()
    {
        this.IsSelected = false;
        Children = new ObservableCollection<TreeModel<T1>>();
        Name = string.Empty;
    }

    #region fields
    //dodać pole "ukryty" i zaimplementować w drzewie, dodać przycisk pokazyjący okno z ukrytymi i możliwością odkrycia
    [ObservableProperty]    
    private int _Id = -1;
    [ObservableProperty]    
    public int _ParentID = -1; 
    [ObservableProperty]
    private TreeModel<T1>? _Parent;
    [ObservableProperty]
    private ObservableCollection<TreeModel<T1>> _children;
    //[ObservableProperty]
    //private T1? _selectedValue;
    [ObservableProperty]
    private string _Name;
    [ObservableProperty]
    private bool _isSelected;
    [ObservableProperty]
    private bool _isExpanded;
    [ObservableProperty]
    private bool _isRightSelected;
    [ObservableProperty]
    private string _view = string.Empty;
    #endregion fields

    #region properties



    /*
    public IEnumerable<TreeModel<T1>> Children
    {
        get { return _children; }
        //set { _children = value}
    }
    */
    public string GetKeyDB
    {
        get;
        set;
    } = string.Empty;



    #endregion properties

    #region methods

    public override string ToString()
    {
        return this.Name;
    }

    public void AddChild(TreeModel<T1> child)
    {
        child.Parent = this;
        this.Children.Add(child);
    }

    #endregion methods


    #region static methods
    // metody statyczne można przenieść do innego obiektu, tu raczej nie mają sensu 
    // operują na nim ale się do niego nie odwołują
    // część przestaje być urzeteczna z powodu wykozystania zachowań "behaviors" co jest bardziej praktyczne
    // dodać odnajdywanie pierszego elementu- będzie on nazwą bazy w drzewie
    /*
    public static TreeModel<T1>? GetNodeById(T1 id, IEnumerable<TreeModel<T1>> nodes)
    {
        foreach (var node in nodes)
        {
            if ((node.SelectedValue != null) &&(node.SelectedValue.Equals(id)))
                return node;

            var foundChild = GetNodeById(id, node.Children);
            if (foundChild != null)
                return foundChild;
        }
        return null;
    }

    public static TreeModel<T1>? GetSelectedNode(IEnumerable<TreeModel<T1>> nodes)
    {
        foreach (var node in nodes)
        {
            if (node.IsSelected)
                return node;

            var selectedChild = GetSelectedNode(node.Children);
            if (selectedChild != null)
                return selectedChild;
        }

        return null;
    }

    public static void ExpandParentNodes(TreeModel<T1> node)
    {
        if (node.Parent != null)
        {
            node.Parent.IsExpanded = true;
            ExpandParentNodes(node.Parent);
        }
    }

    public static void ToggleExpanded(IEnumerable<TreeModel<T1>> nodes, bool isExpanded)
    {
        foreach (var node in nodes)
        {
            node.IsExpanded = isExpanded;
            ToggleExpanded(node.Children, isExpanded);
        }
    }
    */
    #endregion static methods

}

public class TreeModel : TreeModel<Guid>
{
}
