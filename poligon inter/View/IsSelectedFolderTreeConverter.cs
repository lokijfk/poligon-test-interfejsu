using poligon_inter.Model;
using System.Globalization;
using System.Windows.Data;

namespace poligon_inter.View
{
    [ValueConversion(typeof(TreeModel<Guid>), typeof(bool))]
    public class IsSelectedFolderTreeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // throw new NotImplementedException();
            TreeModel<Guid> Element = (TreeModel<Guid>)value;
            return (Element != null ?true : false) ;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
             throw new NotImplementedException();
        }
    }
}
