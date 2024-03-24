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
        // todo: find proper way to load assembly
        Mapsui.Tests.Common.Utilities.LoadAssembly();
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
    ISampleBase _selectedSample;

    [ObservableProperty]
    Map? _map;

    public ObservableCollection<ISampleBase> Samples { get; set; } = new();
    public ObservableCollection<ILayer> Layers { get; set; } = new();
    public ObservableCollection<string> Categories { get; } = new();

    // MapControl is needed in the samples. Mapsui's design should change so this is not needed anymore.
    public MapControl? MapControl { get; set; }

    public void PopulateSamples()
    {
        var samples = AllSamples.GetSamples().OfType<ISampleBase>().Where(s => s.Category == SelectedCategory);
        SelectedSample = samples?.FirstOrDefault();
        Samples.Clear();
        foreach (var sample in samples)
            Samples.Add(sample);
        SelectSample(Samples.First());
    }

    [SuppressMessage("Usage", "VSTHRD100:Avoid async void methods")]
    public async void SelectSample(ISampleBase selectedSample)
    {
        try
        {
            if (selectedSample is null)
                return;

            SelectedSample = selectedSample;

            if (SelectedSample is ISample sample)
            {
                if (Map != null)
                    Map.Layers.Changed -= PopulateLayers;
                Map = await sample.CreateMapAsync();
                Map.Layers.Changed += PopulateLayers;
                PopulateLayers(null, null);
                if (MapControl != null)
                    MapControl.Map = Map;
            }
            else if (SelectedSample is IMapControlSample mapControlSample && MapControl != null)
                mapControlSample.Setup(MapControl);
        }
        catch (Exception ex)
        {
            Logger.Log(LogLevel.Error, ex.Message, ex);
        }
    }

    private void PopulateLayers(object sender, LayerCollectionChangedEventArgs args)
    {
        Layers.Clear();
        foreach (var layer in Map?.Layers)
            Layers.Add(layer);
    }
}
