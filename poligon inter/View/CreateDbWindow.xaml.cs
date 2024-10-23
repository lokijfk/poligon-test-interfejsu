using System.Windows;
using poligon_inter.ViewModel;


namespace poligon_inter.View
{
    /// <summary>
    /// Logika interakcji dla klasy CreateDbWindow.xaml
    /// </summary>
    public partial class CreateDbWindow : Window
    {
        public CreateDbWindow()
        {
            DataContext = new CreateDBViewModel();
            InitializeComponent();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void btnAnuluj_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
