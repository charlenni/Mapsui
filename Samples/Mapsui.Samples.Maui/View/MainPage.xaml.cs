using Mapsui.Samples.Common;
using Mapsui.Samples.Common.Maps.Widgets;
using Mapsui.Samples.Maui.ViewModel;
using Mapsui.UI.Maui;

namespace Mapsui.Samples.Maui.View;

public partial class MainPage : ContentPage
{
	public MainPage(MainViewModel mainViewModel)
	{
		InitializeComponent();

        mapControl.Map.Navigator.RotationLock = false;
        mapControl.Map.Navigator.UnSnapRotation = 30;
        mapControl.Map.Navigator.ReSnapRotation = 5;

        BindingContext = mainViewModel;
        mapControl.SetBinding(MapControl.MapProperty, new Binding(nameof(MainViewModel.Map)));

        // Workaround. Samples need the MapControl in the current setup.
        mainViewModel.MapControl = mapControl;

        // The CustomWidgetSkiaRenderer needs to be registered to make the CustomWidget sample work.
        // Perhaps it is possible to let the sample itself do this so we do not have to do this for each platform.
        mapControl.Renderer.WidgetRenders[typeof(CustomWidget)] = new CustomWidgetSkiaRenderer();
    }

    private void Picker_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (BindingContext is MainViewModel mainViewModel) 
        { 
            mainViewModel.PopulateSamples();
        }
    }

    private void RadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (BindingContext is MainViewModel mainViewModel && sender is RadioButton rb && rb.IsChecked)
        {
            mainViewModel.SelectSample((ISampleBase)rb.Value);
        }
    }

    private void Slider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        if (BindingContext is MainViewModel mainViewModel && mainViewModel.Map?.Navigator.Viewport.Rotation != e.NewValue)
        {
            mainViewModel.Map?.Navigator.RotateTo(e.NewValue);
        }
    }

    private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        (BindingContext as MainViewModel)?.Map?.Refresh();
    }
}

