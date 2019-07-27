using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MileStoneClient.PresentationLayer.Converters
{
    /// <summary>
    /// Boolean to solid brush color convertor.
    /// Used to color message bubbles differntly by current user and other users messages.
    /// </summary>
    class BooleanToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value == true)
            {
               return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD5F0D2"));
            }
            else return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFCFE6FF"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
