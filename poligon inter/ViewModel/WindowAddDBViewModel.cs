using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poligon_inter.ViewModel;

public partial class WindowAddDBViewModel : ObservableObject
{
    [ObservableProperty]
    private string? _name;
    [ObservableProperty]
    private string? _hint;
    [ObservableProperty]
    private string? _WindowName;

}
