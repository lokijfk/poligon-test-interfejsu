using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using System.Windows.Input;


namespace poligon_inter.ViewModel;

public partial class WindowAddDBViewModel : ObservableObject
{

    public event EventHandler Close;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OKCommand))]
    private string? _name;
    [ObservableProperty]
    private string? _hint;
    [ObservableProperty]
    private string? _WindowName;
    [ObservableProperty]
    private bool _isValidate = false;
 
    private bool CanOK()
    {
        _isValidate = !string.IsNullOrWhiteSpace(Name);
        Debug.WriteLine("name:" + Name+" validate:"+ _isValidate);
        
        return _isValidate;//to nie działa
        //return true;
    }



    [RelayCommand(CanExecute = nameof(CanOK))]
    private async Task<bool> OnOK()
    {
        Debug.WriteLine("XXXL = name:" + Name + " validate:" + _isValidate);
        // to jest wywo lane po oknie w main...
        if (_isValidate)
        {
            Close?.Invoke(this, EventArgs.Empty);
        }
        return true;
        //TODO: Else show error indicating failed login
    }



}
