using System.Globalization;
using Microsoft.Maui.Graphics.Converters;

namespace SpellingTest.Maui;

public class StringToColor : IValueConverter
{

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        ColorTypeConverter converter = new ColorTypeConverter();
        Color color = (Color)(converter.ConvertFromInvariantString((string)value));
        return color;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string colorString = "White"; //TODO

        return colorString;
    }
}