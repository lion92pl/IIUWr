using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace IIUWr.Converters
{
    public class IndentConverter : IValueConverter
    {
        public double LevelIndent { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var convertedValue = System.Convert.ToDouble(value);
            var returnValue = convertedValue * LevelIndent;
            if (targetType == typeof(Thickness))
            {
                return new Thickness(returnValue, 0, 0, 0);
            }
            if (targetType == typeof(GridLength))
            {
                return new GridLength(returnValue);
            }
            
            return returnValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is Thickness t)
            {
                value = t.Left;
            }
            if (value is GridLength gl)
            {
                value = gl.Value;
            }

            double convertedValue = System.Convert.ToDouble(value);
            return convertedValue / LevelIndent;
        }
    }
}
