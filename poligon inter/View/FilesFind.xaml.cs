using System.Windows.Controls;
using poligon_inter.ViewModel;


namespace poligon_inter.View
{
    /// <summary>
    /// Logika interakcji dla klasy FilesFind.xaml
    /// </summary>
    public partial class FilesFind : UserControl
    {
        public FilesFind()
        {
            InitializeComponent();
            DataContext = new FilesFindViewModel();
            //(DataContext as FilesFindViewModel).PathSearch(string.Empty);
        }
    }
}
