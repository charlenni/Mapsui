using Mapsui.Features;
using Mapsui.Fetcher;
using Mapsui.Providers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Mapsui.Layers;

public class AsyncLayer : Layer, IAsyncDataFetcher, IDataSourceLayer<IAsyncProvider>
{
    private IAsyncProvider? _dataSource;
    private readonly object _syncRoot = new();
    private readonly ConcurrentStack<IFeature> _cache = new();
    private readonly FeatureFetchDispatcher<IFeature> _fetchDispatcher;
    private readonly FetchMachine _fetchMachine;

    /// <summary>
    /// Create a new layer
    /// </summary>
    public AsyncLayer() : this("Layer") { }

    /// <summary>
    /// Create layer with name
    /// </summary>
    /// <param name="layerName">Name to use for layer</param>
    public AsyncLayer(string layerName) : base(layerName)
    {
        _fetchDispatcher = new FeatureFetchDispatcher<IFeature>(_cache);
        _fetchDispatcher.DataChanged += FetchDispatcherOnDataChanged;
        _fetchDispatcher.PropertyChanged += FetchDispatcherOnPropertyChanged;

        _fetchMachine = new FetchMachine(_fetchDispatcher);
    }

    public Delayer Delayer { get; } = new();

    /// <summary>
    /// Time to wait before fetching data
    /// </summary>
    // ReSharper disable once UnusedMember.Global // todo: Create a sample for this field
    public int FetchingPostponedInMilliseconds
    {
        get => Delayer.MillisecondsToWait;
        set => Delayer.MillisecondsToWait = value;
    }
    /// <summary>
    /// Data source for this layer
    /// </summary>
    public new IAsyncProvider? DataSource
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

            ClearCache();

            if (_dataSource != null)
            {
                _fetchDispatcher.DataSource = _dataSource;
            }

            OnPropertyChanged(nameof(DataSource));
            OnPropertyChanged(nameof(Extent));
        }
    }

    private void FetchDispatcherOnPropertyChanged(object? sender, PropertyChangedEventArgs propertyChangedEventArgs)
    {
        if (propertyChangedEventArgs.PropertyName == nameof(Busy))
        {
            if (_fetchDispatcher != null) Busy = _fetchDispatcher.Busy;
        }
    }

    private void FetchDispatcherOnDataChanged(object? sender, DataChangedEventArgs args)
    {
        OnDataChanged(args);
    }

    private void DelayedFetch(FetchInfo fetchInfo)
    {
        _fetchDispatcher.SetViewport(fetchInfo);
        _fetchMachine.Start();
    }

    /// <summary>
    /// Returns the extent of the layer
    /// </summary>
    /// <returns>Bounding box corresponding to the extent of the features in the layer</returns>
    public override MRect? Extent
    {
        get
        {
            lock (_syncRoot)
            {
                return DataSource?.GetExtent();
            }
        }
    }

    /// <inheritdoc />
    public override IEnumerable<IFeature> GetFeatures(MRect extent, double resolution)
    {
        return _cache.ToList();
    }

    /// <inheritdoc />
    public void AbortFetch()
    {
        _fetchMachine.Stop();
    }

    /// <inheritdoc />
    public void ClearCache()
    {
        _cache.Clear();
    }

    /// <inheritdoc />
    public void RefreshData(FetchInfo fetchInfo)
    {
        if (!Enabled) return;
        if (MinVisible > fetchInfo.Resolution) return;
        if (MaxVisible < fetchInfo.Resolution) return;
        if (DataSource == null) return;
        if (fetchInfo.ChangeType == ChangeType.Continuous) return;

        Delayer.ExecuteDelayed(() => DelayedFetch(fetchInfo));
    }

    private void HandleDataChanged(object? sender, DataChangedEventArgs e)
    {
        DataHasChanged();
    }
}