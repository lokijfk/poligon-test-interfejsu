
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace poligon_inter.ViewModel;

public class ContextMenuTreeView
{
    public string Displayname { get; private set; }
    public ICommand MyContextMenuCommand { get; private set; }

    public ContextMenuTreeView(string header, Action<object> execute)
    {
        Displayname = header;
        MyContextMenuCommand = new RelayCommand<object>(execute);
    }


    /*
    public ContextMenuTreeView(string header, Action<object> execute, Func<object,bool> canExecute)
    {
        this.Displayname = header;
        this.MyContextMenuCommand = new RelayCommand<object>(execute, canExecute);
    }
    */

    public ContextMenuTreeView(string header, Action execute, Func<bool> canExecute)
    {
        Displayname = header;
        MyContextMenuCommand = new RelayCommand(execute, canExecute);
    }
}
