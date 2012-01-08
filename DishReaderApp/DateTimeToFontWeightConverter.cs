using System;
using System.Windows;
using System.Windows.Data;
using System.Globalization;

namespace DishReaderApp
{
    public class DateTimeToFontWeightConverter : IValueConverter
    {
        public static DateTime ThresholdDate { get; set; }

        static DateTimeToFontWeightConverter()
        {
            ThresholdDate = DateTime.MinValue;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && (DateTime)value > ThresholdDate)
            {
                return FontWeights.Bold;
            }

            return FontWeights.Normal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
