using System.Globalization;
using System.Windows.Data;

namespace poligon_inter.View.Converters;

class IsNotDirConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // throw new NotImplementedException();
        string tekst = (string)value;
        return tekst.ToLower() != "dir" ? "Visible" : "Hidden";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
