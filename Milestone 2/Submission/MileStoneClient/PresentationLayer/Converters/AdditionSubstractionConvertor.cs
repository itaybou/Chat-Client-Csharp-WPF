using System;
using System.Windows.Data;

namespace MileStoneClient.PresentationLayer.Converters
{
    /// <summary>
    /// Converts an integer value to the addition or substraction value with the integer parameter value.
    /// </summary>
    public class AdditionSubstractionConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int toAdd = int.Parse(parameter as string);
            int input = (int)value;
            return input + toAdd;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
