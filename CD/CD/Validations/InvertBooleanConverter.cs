using System;
using System.Globalization;
using Xamarin.Forms;

namespace CD.Validations
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
