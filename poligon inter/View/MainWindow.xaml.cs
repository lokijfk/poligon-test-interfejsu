using Microsoft.Win32;
using poligon_inter.ViewModel;
using System.Windows;

namespace poligon_inter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataContext = new MainWindowViewModel();
            InitializeComponent();
        }

        private void AddFolder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                string destinationFilePath = dialog.FileName;
                (this.DataContext as MainWindowViewModel)?.SaveAlbumName(destinationFilePath);
            }
        }
    }
}