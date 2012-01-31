using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Microsoft.Phone.Controls;

namespace Utilities
{
    public sealed class RelativeAndAbsoluteTimeConverter : IValueConverter
    {
        private const string timeDisplayFormat = "{1:t}, {0}";
        private readonly RelativeTimeConverter converter = new RelativeTimeConverter();

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is DateTime)
            {
                return string.Format(timeDisplayFormat, converter.Convert(value, targetType, parameter, culture), value);
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}