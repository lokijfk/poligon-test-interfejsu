using Microsoft.Win32;
using poligon_inter.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace poligon_inter.View
{
    /// <summary>
    /// Logika interakcji dla klasy Files.xaml
    /// </summary>
    public partial class Files : UserControl
    {
        public Files()
        {
            InitializeComponent();
            DataContext = new FilesViewModel();
        }

        private void FindFiles_Click(object sender, RoutedEventArgs e)
        {
            var folder = new OpenFolderDialog();
            string path = string.Empty;
            if(folder.ShowDialog() == true)
            {
                path = folder.FolderName;
                //DataContext.PathSearch = path;
                (this.DataContext as FilesViewModel).PathSearch(path);
            }
        }
    }
}
