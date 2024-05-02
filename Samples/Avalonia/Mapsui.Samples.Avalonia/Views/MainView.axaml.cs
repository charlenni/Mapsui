using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using AvaloniaApplication1.ViewModels;
using Mapsui.Samples.Common.Maps.Widgets;
using System.Linq;

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
            mainViewModel.SelectSample(mainViewModel.Samples.FirstOrDefault());
        }
    }

    private void Slider_ValueChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        if (DataContext is MainViewModel mainViewModel && mainViewModel.Map?.Navigator.Viewport.Rotation != e.NewValue)
            mainViewModel.Map?.Navigator.RotateTo(e.NewValue);
    }

    private void RadioButton_IsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        if (sender is RadioButton rb && (rb.IsChecked ?? false) && DataContext is MainViewModel mainViewModel)
        {
            var sample = mainViewModel.Samples.FirstOrDefault(s => s.Name == (string)(rb.Content ?? ""));
            mainViewModel.SelectSample(sample);
        }
    }

    private void CheckBox_IsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        (DataContext as MainViewModel)?.Map?.Refresh();
    }
}
