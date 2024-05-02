using CommunityToolkit.Mvvm.ComponentModel;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Logging;
using Mapsui.UI.Avalonia;
using Mapsui.Samples.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace AvaloniaApplication1.ViewModels;

public partial class MainViewModel : ObservableObject
{
    static MainViewModel()
    {
    }

    public MainViewModel()
    {
        var allSamples = AllSamples.GetSamples() ?? new List<ISampleBase>();
        Categories = new ObservableCollection<string>(allSamples.Select(s => s.Category).Distinct().OrderBy(c => c));
        _selectedCategory = Categories.First();
        PopulateSamples();
        _selectedSample = Samples.First();
        Map = new Map();
        Map.Layers.Changed += PopulateLayers;
        if (MapControl != null)
            MapControl.Map = Map;
    }

    [ObservableProperty]
    string _selectedCategory;

    [ObservableProperty]
    SampleBaseExtension _selectedSample;

    [ObservableProperty]
    Map? _map;

    [ObservableProperty]
    double _rotation;

    public ObservableCollection<SampleBaseExtension> Samples { get; set; } = new();
    public ObservableCollection<ILayer> Layers { get; set; } = new();
    public ObservableCollection<string> Categories { get; } = new();

    // MapControl is needed in the samples. Mapsui's design should change so this is not needed anymore.
    public MapControl? MapControl { get; set; }

    public void PopulateSamples()
    {
        var samples1 = AllSamples.GetSamples();
        var samples2 = samples1.OfType<ISampleBase>();
        var samples = samples2.Where(s => s.Category == SelectedCategory);
        Samples.Clear();
        foreach (var sample in samples)
            Samples.Add(new SampleBaseExtension(sample));
        SelectSample(Samples.First());
    }

    [SuppressMessage("Usage", "VSTHRD100:Avoid async void methods")]
    public async void SelectSample(SampleBaseExtension selectedSample)
    {
        try
        {
            if (selectedSample is null)
                return;

            if (SelectedSample != null)
                SelectedSample.IsSelected = false;

            SelectedSample = selectedSample;

            SelectedSample.IsSelected = true;

            if (SelectedSample.Sample is ISample sample)
            {
                if (Map != null)
                {
                    Map.Layers.Changed -= PopulateLayers;
                    Map.Navigator.ViewportChanged -= ViewportChanged;
                }
                Map = await sample.CreateMapAsync();
                Map.Layers.Changed += PopulateLayers;
                Map.Navigator.ViewportChanged += ViewportChanged;

                PopulateLayers(null, null);
                if (MapControl != null)
                    MapControl.Map = Map;
            }
            else if (SelectedSample.Sample is IMapControlSample mapControlSample && MapControl != null)
                mapControlSample.Setup(MapControl);
        }
        catch (Exception ex)
        {
            Logger.Log(LogLevel.Error, ex.Message, ex);
        }
    }

    private void ViewportChanged(object sender, ViewportChangedEventArgs e)
    {
        Rotation = Map?.Navigator.Viewport.Rotation ?? 0.0;
    }

    /// <summary>
    /// Add all layers of map to layer list 
    /// </summary>
    private void PopulateLayers(object sender, LayerCollectionChangedEventArgs args)
    {
        Layers.Clear();
        foreach (var layer in Map?.Layers)
            Layers.Add(layer);
    }
}

/// <summary>
/// Wrapper around the ISampleBase to have the possibility to get a flag which sample is selected
/// </summary>
public class SampleBaseExtension
{
    public ISampleBase Sample { get; init; }
    public bool IsSelected { get; set; }
    public string Name => Sample.Name;

    public SampleBaseExtension(ISampleBase sample)
    {
        Sample = sample;
    }
}
