using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;


namespace poligon_inter.ViewModel;

public partial class WindowAddDBViewModel : ObservableObject
{

    //public event EventHandler Close;

    
    //[NotifyCanExecuteChangedFor(nameof(OKCommand))]
    [ObservableProperty]
    private string? _name;
    [ObservableProperty]
    private string? _hint;
    [ObservableProperty]
    private string? _WindowName;
    [ObservableProperty]
    private bool _isValidate = false;

    /*
    private bool CanOK()
    {
        IsValidate = !string.IsNullOrWhiteSpace(Name);
        Debug.WriteLine("name:" + Name+" validate:"+ IsValidate);
    
        return IsValidate;//to nie działa
        //return true;
    }



    [RelayCommand(CanExecute = nameof(CanOK))]
    private async Task<bool> OnOK()
    {
        //Debug.WriteLine("XXXL = name:" + Name + " validate:" + _isValidate);
        // to jest wywo lane po oknie w main...
        if (IsValidate)
        {
            Close?.Invoke(this, EventArgs.Empty);
        }
        return true;
        //TODO: Else show error indicating failed login
    }
    */


}
