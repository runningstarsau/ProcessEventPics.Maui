namespace ProcessEventPics.Maui.Converters
{
    public class StringToColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string colorString && !string.IsNullOrEmpty(colorString))
            {
                try
                {
                    return Color.FromArgb(colorString);
                }
                catch
                {
                    return Colors.Black; // Default fallback
                }
            }
            return Colors.Black;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Color color)
            {
                return color.ToArgbHex();
            }
            return "#000000";
        }
    }
}
