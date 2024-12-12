using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using poligon_inter.Model;

namespace poligon_inter.ViewModel
{

    public partial class CreateDBViewModel : ObservableObject
    {   

        // gdzieś trzeba dodać że jak mysz zejdzie z pola tekstu to następuje lost focus
        // i wtedy idzie informacja o przeładowaniu przycisku    
        // warto przejżeć te konstrukcje odnośnie okna do mvvm żeby nie robić takiego klejonego bo jeszcze nie sprawdziłem jak to działa
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(onClouseCommand))]
        private string nazwa =string.Empty;
        [ObservableProperty]
        private string pass =string.Empty;

        [RelayCommand(CanExecute = nameof(CanOnClouse)) ]
        private void onClouse()
        {
            /*
            //tu kopiowanie danych do modelu współdzielonego
            ModelPB model = Tools.getModel();
            if(pass != string.Empty)
            model.password = Tools.CalculateMD5Sting(pass);
            model.nazwa = nazwa;
            */
        }


        private bool CanOnClouse() => Nazwa != string.Empty;
    


    }
}
