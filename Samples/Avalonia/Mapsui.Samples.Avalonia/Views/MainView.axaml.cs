using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using AvaloniaApplication1.ViewModels;
using Mapsui.Extensions;
using Mapsui.Samples.Common;
using Mapsui.Samples.Common.Extensions;
using Mapsui.Samples.Common.Maps.Widgets;

namespace Mapsui.Samples.Avalonia.Views;

public partial class MainView : UserControl
{
    static MainView()
    {
        Mapsui.Tests.Common.Samples.Register();
        Mapsui.Samples.Common.Samples.Register();
    }

    public MainView()
    {
        InitializeComponent(true);

        DataContextChanged += MainView_DataContextChanged;
    }

    private void MainView_DataContextChanged(object? sender, System.EventArgs e)
    {
        mapControl.Map.Navigator.RotationLock = false;
        mapControl.Map.Navigator.UnSnapRotation = 30;
        mapControl.Map.Navigator.ReSnapRotation = 5;

        slider.ValueChanged += Slider_ValueChanged;

        // Workaround. Samples need the MapControl in the current setup.
        var mainViewModel = (MainViewModel)DataContext;
        mainViewModel.MapControl = mapControl;


        // The CustomWidgetSkiaRenderer needs to be registered to make the CustomWidget sample work.
        // Perhaps it is possible to let the sample itself do this so we do not have to do this for each platform.
        mapControl.Renderer.WidgetRenders[typeof(CustomWidget)] = new CustomWidgetSkiaRenderer();
    }

    private void ComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (DataContext is MainViewModel mainViewModel)
        {
            mainViewModel.PopulateSamples();
        }
    }

    private void Slider_ValueChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        if (DataContext is MainViewModel mainViewModel)
        {
            mainViewModel.Map?.Navigator.RotateTo(e.NewValue);
        }
    }

    private RadioButton CreateRadioButton(ISampleBase sample)
    {
        var radioButton = new RadioButton
        {
            FontSize = 16,
            Content = sample.Name,
            Margin = new Thickness(4)
        };

        radioButton.Click += (s, a) =>
        {
            Catch.Exceptions(async () =>
            {
                mapControl.Map?.Layers.Clear();
                await sample.SetupAsync(mapControl);
                mapControl.Refresh();
            });
        };

        return radioButton;
    }
}
