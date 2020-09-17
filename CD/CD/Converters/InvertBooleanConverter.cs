using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using Xamarin.Forms;

namespace CD.Converters
{
    public class InvertBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            Invert((bool)value);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            Invert((bool)value);

        bool Invert(bool val) => !val;
    }
}
