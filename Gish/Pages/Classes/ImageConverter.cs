using System.Globalization;

namespace Gish.Pages.Classes;

public class ImageConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null)
        {
            return null;
        }

        var bytes = (byte[])value;
        
        var streamSource = ImageSource.FromStream(() => new MemoryStream(bytes));

        return streamSource;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}