using Mapsui.Features;
using Mapsui.Providers;
using Mapsui.Styles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mapsui.Layers;

/// <summary>
/// Layer getting data from a data source
/// </summary>
public class Layer : BaseLayer, IDataSourceLayer<IProvider>
{
    private IProvider? _dataSource;

    /// <summary>
    /// Create a new layer
    /// </summary>
    public Layer() : this("Layer") { }

    /// <summary>
    /// Create layer with name
    /// </summary>
    /// <param name="layerName">Name to use for layer</param>
    public Layer(string layerName) : base(layerName) { }

    /// <summary>
    /// TODO: Not clear, what this property is used for
    /// </summary>
    public SymbolStyle? SymbolStyle { get; set; }

    /// <summary>
    /// Animations belonging to this layer
    /// </summary>
    public List<Func<bool>> Animations { get; } = [];

    /// <summary>
    /// Data source for this layer
    /// </summary>
    public virtual IProvider? DataSource
    {
        get => _dataSource;
        set
        {
            if (_dataSource == value) return;

            if (_dataSource is IDataChangedProvider oldProvider)
                oldProvider.DataChanged -= HandleDataChanged;

            _dataSource = value;

            if (_dataSource is IDataChangedProvider newProvider)
                newProvider.DataChanged += HandleDataChanged;

            OnPropertyChanged(nameof(DataSource));
            OnPropertyChanged(nameof(Extent));
        }
    }

    /// <summary>
    /// Returns the extent of the layer
    /// </summary>
    /// <returns>Bounding box corresponding to the extent of the features in the layer</returns>
    public override MRect? Extent => DataSource?.GetExtent();

    /// <inheritdoc />
    public override IEnumerable<IFeature> GetFeatures(MRect extent, double resolution)
    {
        return _dataSource?.GetFeatures(new FetchInfo(new MSection(extent, resolution))) ?? Enumerable.Empty<IFeature>();
    }

    public override bool UpdateAnimations()
    {
        var areAnimationsRunning = false;
        foreach (var animation in Animations)
        {
            if (animation())
                areAnimationsRunning = true;
        }
        return areAnimationsRunning;
    }

    private void HandleDataChanged(object? sender, DataChangedEventArgs e)
    {
        DataHasChanged();
    }
}
