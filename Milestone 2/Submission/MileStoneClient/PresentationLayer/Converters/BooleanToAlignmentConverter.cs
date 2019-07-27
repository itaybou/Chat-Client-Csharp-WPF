using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MileStoneClient.PresentationLayer.Converters
{
    /// <summary>
    /// Boolean the Horizontal Alignment convertor, True aligns right, false aligns left.
    /// Used to align messages by current user and by other users.
    /// </summary>
    class BooleanToAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value == true)
            {
                return HorizontalAlignment.Right;
            }
            else return HorizontalAlignment.Left;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
