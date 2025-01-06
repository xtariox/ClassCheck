using System;
using System.Globalization;

namespace ClassCheck.Converters
{
    public class InverseBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return !boolValue;  // Invert the boolean value
            }
            return value;  // If the value is not boolean, return it unchanged
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return !boolValue;  // Invert the boolean value for two-way binding (if needed)
            }
            return value;  // If the value is not boolean, return it unchanged
        }
    }
}

