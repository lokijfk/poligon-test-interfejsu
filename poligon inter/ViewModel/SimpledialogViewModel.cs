using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;


namespace poligon_inter.ViewModel;

public partial class SimpledialogViewModel : ObservableObject
{
    [ObservableProperty]
    private string? _name;

    [ObservableProperty]
    private string? _windowName;

    [ObservableProperty]
    private string? _hint;

    public bool CanOnClouse()
    {
       return Name != string.Empty;
    } 
}
