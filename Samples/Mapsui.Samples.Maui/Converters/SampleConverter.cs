using Mapsui.Samples.Maui.ViewModel;
using System.Globalization;

namespace Mapsui.Samples.Maui;

public class SampleConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return ((MainViewModel)((Slider)parameter)?.BindingContext)?.SelectedSample?.Name == (string)value;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
