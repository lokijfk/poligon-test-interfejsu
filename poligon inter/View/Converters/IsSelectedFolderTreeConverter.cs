using poligon_inter.Model;
using System.Globalization;
using System.Windows.Data;

namespace poligon_inter.View.Converters;

[ValueConversion(typeof(TreeModel), typeof(bool))]
public class IsSelectedFolderTreeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // throw new NotImplementedException();
        TreeModel Element = (TreeModel)value;
        return Element != null ? true : false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
