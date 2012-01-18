using System;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;

namespace DishReaderApp.Utilities
{
    public sealed class HtmlToTextConverter : IValueConverter
    {
        private static Regex html = new Regex("<[^>]+>", RegexOptions.Compiled);

        public string Convert(string value)
        {
            string output = Regex.Replace(value.ToString(), "<[^>]+>", string.Empty);
            output = output.Replace("\r", "").Replace("\n", "");
            output = HttpUtility.HtmlDecode(output);
            output = output.Trim();

            return output;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && (bool)value)
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