using System;
using System.Globalization;
using System.Windows.Data;

namespace MileStoneClient.PresentationLayer.Converters
{
    /// <summary>
    /// Converts an integer font size offset to real integer font size value.
    /// </summary>
    [ValueConversion(typeof(int), typeof(int))]
    public class FontSizeOffsetToRealFontSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((int)value) + 8;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Cannot convert back");
        }
    }
}
