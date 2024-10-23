using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace poligon_inter.View;

[ValueConversion(typeof(String), typeof(bool))]
public class IsTextNotEmptyConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // throw new NotImplementedException();
        string tekst = (string)value;
        return (tekst.Length > 0 ?true : false) ;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
         throw new NotImplementedException();
    }
}
