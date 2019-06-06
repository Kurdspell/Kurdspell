using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace DictionaryEditor.Helpers
{
    public class BooleanToBrushConverter : IValueConverter
    {
        public Brush True { get; set; }
        public Brush False { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
            {
                return b ? True : False;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Brush brush)
            {
                return brush == True;
            }

            return value;
        }
    }
}
