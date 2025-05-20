using System.Globalization;
using System.Windows.Data;

namespace poligon_inter.View.Converters;

[ValueConversion(typeof(string), typeof(bool))]
public class IsTextNotEmptyConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // throw new NotImplementedException();
        string tekst = (string)value;
        return tekst.Length > 0 ? true : false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
