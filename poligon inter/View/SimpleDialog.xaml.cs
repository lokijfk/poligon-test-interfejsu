
using System.Windows.Controls;


namespace poligon_inter.View
{
    /// <summary>
    /// Logika interakcji dla klasy WindowAddDB.xaml
    /// </summary>
    public partial class SimpleDialog : UserControl
    {
        public SimpleDialog()
        {
            InitializeComponent();
        }

        //to mi sie nie podobawolał bym to zrobić na poziomie xaml, ale nie jestem pewien jak !!!
        //no i nie działa wogóle
        private void CommandBinding_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            e.CanExecute = DataContextProperty.Name != string.Empty;
            
            //sender.
        }
    }
}
