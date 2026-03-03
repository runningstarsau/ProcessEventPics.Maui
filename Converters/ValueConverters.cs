using System.Globalization;
using ProcessEventPics.Maui.Models;

namespace ProcessEventPics.Maui.Converters;

public class BoolToColorConverter : IValueConverter
{


    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue && boolValue)
        {
            return Colors.Blue;
        }
        return Colors.Gray;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class InverseBoolConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return !boolValue;
        }
        return true;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return !boolValue;
        }
        return false;
    }
}

public class EventTypeToLabelConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string brandName)
        {
            return $"Select {brandName} Location";
        }
        return "Select Location";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class SelectedItemBackgroundConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values == null || values.Length < 3)
            return Colors.White;

        // values[0] = current item (string location)
        // values[1] = selected item (string location)
        // values[2] = selected brand (Brand object)

        bool isSelected = values[0] is string itemValue && 
                         values[1] is string selectedValue && 
                         itemValue == selectedValue;

        if (!isSelected)
            return Colors.White;

        // Use the brand's PrimaryColor for selected item background
        if (values[2] is Brand brand && !string.IsNullOrEmpty(brand.PrimaryColor))
        {
            try
            {
                return Color.FromArgb(brand.PrimaryColor);
            }
            catch
            {
                return Color.FromArgb("#FFA300");
            }
        }

        return Color.FromArgb("#FFA300");
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class SelectedItemBorderConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // For string items (locations)
        if (value is string itemValue && parameter is string selectedValue)
        {
            return itemValue == selectedValue ? Color.FromArgb("#2196F3") : Colors.LightGray;
        }

        // For Brand items
        if (value is Brand brand && parameter is Brand selectedBrand)
        {
            return brand == selectedBrand ? Color.FromArgb("#2196F3") : Colors.LightGray;
        }

        return Colors.LightGray;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class NotNullConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value != null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class IsZeroConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int intValue)
        {
            return intValue == 0;
        }
        return false;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class BrandColorConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values == null || values.Length < 3)
            return Colors.White;

        // values[0] = current item (string location)
        // values[1] = selected item (string location)
        // values[2] = selected brand (Brand object)

        bool isSelected = values[0] is string itemValue && 
                         values[1] is string selectedValue && 
                         itemValue == selectedValue;

        if (!isSelected)
            return Colors.White;

        if (values[2] is Brand brand && !string.IsNullOrEmpty(brand.TertiaryColor))
        {
            try
            {
                return Color.FromArgb(brand.TertiaryColor);
            }
            catch
            {
                return Color.FromArgb("#E3F2FD");
            }
        }

        return Color.FromArgb("#E3F2FD");
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class SelectedItemTextColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // value is the current item (string location)
        // parameter should be a string in format "selectedLocation|brandPrimaryColor"
        // For simplicity, we'll just return the brand's primary color if selected
        if (value is string itemValue && parameter is string selectedValue)
        {
            // If selected, return a brand color; otherwise return default black
            return itemValue == selectedValue ? Color.FromArgb("#2196F3") : Colors.Black;
        }
        return Colors.Black;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class SelectedLocationTextColorConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values == null || values.Length < 3)
            return Colors.Black;

        // values[0] = current item (string location)
        // values[1] = selected item (string location)
        // values[2] = selected brand (Brand object)

        bool isSelected = values[0] is string itemValue && 
                         values[1] is string selectedValue && 
                         itemValue == selectedValue;

        if (!isSelected)
            return Colors.Black;

        // Use the brand's SecondaryColor for selected item text
        if (values[2] is Brand brand && !string.IsNullOrEmpty(brand.SecondaryColor))
        {
            try
            {
                return Color.FromArgb(brand.SecondaryColor);
            }
            catch
            {
                return Color.FromArgb("#2196F3");
            }
        }

        return Colors.White;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class HideWhenSelectedConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        // values[0] is the current item, values[1] is the selected item
        // Return false (hide) when selected, true (show) when not selected
        if (values.Length == 2 && values[0] is string itemValue && values[1] is string selectedValue)
        {
            return itemValue != selectedValue; // Hide when equal (selected)
        }
        return true; // Show by default
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class IsFavoriteConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        // values[0] = current location (string)
        // values[1] = selected brand (Brand object)

        if (values == null || values.Length < 2)
            return Colors.LightGray; // Default greyed out

        if (values[0] is not string location || values[1] is not Brand brand)
            return Colors.LightGray;

        // Check if location is in favorites
        bool isFavorite = brand.FavoriteLocations?.Contains(location) ?? false;

        if (isFavorite)
        {
            // Use brand's SecondaryColor for active star
            try
            {
                return Color.FromArgb(brand.SecondaryColor);
            }
            catch
            {
                return Colors.Gold; // Fallback
            }
        }

        return Colors.LightGray; // Greyed out when not favorite
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class FavoriteStarTextConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        // values[0] = current location (string)
        // values[1] = selected brand (Brand object)

        if (values == null || values.Length < 2)
            return "☆"; // Outline star (not favorite)

        if (values[0] is not string location || values[1] is not Brand brand)
            return "☆"; // Outline star (not favorite)

        // Check if location is in favorites
        bool isFavorite = brand.FavoriteLocations?.Contains(location) ?? false;

        return isFavorite ? "⭐" : "☆"; // Filled star for favorite, outline star for non-favorite
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class ContainsConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // value should be in format "location|favoriteLocations" as a combined string
        // OR value is the location and we need another approach

        // Since we can't pass dynamic ConverterParameter, we'll use a different strategy
        // This won't work as originally planned with DataTrigger
        // We need to use MultiBinding approach instead

        if (value is List<string> list && parameter is string item)
        {
            return list.Contains(item);
        }

        return false;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class FavoriteBorderColorConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values == null || values.Length < 3)
            return Colors.Transparent;

        // values[0] = current location (string)
        // values[1] = selected location (string) 
        // values[2] = selected brand (Brand object)

        if (values[0] is not string location || values[2] is not Brand brand)
            return Colors.Transparent;

        // Check if location is in favorites
        bool isFavorite = brand.FavoriteLocations?.Contains(location) ?? false;

        if (isFavorite)
        {
            return Colors.Gold;
        }

        // Check if selected
        bool isSelected = values[1] is string selectedValue && location == selectedValue;

        if (isSelected && !string.IsNullOrEmpty(brand.PrimaryColor))
        {
            try
            {
                return Color.FromArgb(brand.PrimaryColor);
            }
            catch
            {
                return Colors.Transparent;
            }
        }

        return Colors.Transparent;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
