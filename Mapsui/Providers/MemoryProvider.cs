using Mapsui.Features;
using Mapsui.Layers;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace Mapsui.Providers;

public class MemoryProvider : IProvider
{
    private readonly object _sync = new object();
    private MRect? _extent;

    /// <summary>
    /// Initializes a new instance of the MemoryProvider
    /// </summary>
    public MemoryProvider()
    {
        _features = new List<IFeature>();
        _extent = null;
    }

    /// <summary>
    /// Initializes a new instance of the MemoryProvider
    /// </summary>
    /// <param name="feature">Feature to be in this dataSource</param>
    public MemoryProvider(IFeature feature)
    {
        _features = new List<IFeature> { feature };
        _extent = GetExtent(Features);
    }

    /// <summary>
    /// Initializes a new instance of the MemoryProvider
    /// </summary>
    /// <param name="features">Features to be included in this dataSource</param>
    public MemoryProvider(IEnumerable<IFeature> features)
    {
        _features = features.ToList();
        _extent = GetExtent(Features);
    }

    private List<IFeature> _features = new List<IFeature>();

    /// <summary>
    /// Gets or sets the geometries this data source contains
    /// </summary>
    public IReadOnlyList<IFeature> Features { get => _features.ToImmutableList(); }

    public double SymbolSize { get; set; } = 64;

    /// <summary>
    /// Spatial reference ID (CRS)
    /// </summary>
    public string? CRS { get; set; }

    /// <summary>
    /// Get all features contained in FetchInfos extend
    /// </summary>
    /// <param name="fetchInfo">FetchInfo to use</param>
    /// <returns>Task to get list of features</returns>
    public virtual Task<IEnumerable<IFeature>> GetFeaturesAsync(FetchInfo fetchInfo)
    {
        if (fetchInfo == null || fetchInfo.Extent == null)
            return Task.FromResult(Enumerable.Empty<IFeature>()); 

        var features = _features.ToImmutableArray();

        fetchInfo = new FetchInfo(fetchInfo);

        // Use a larger extent so that symbols partially outside of the extent are included
        var biggerBox = fetchInfo.Extent?.Grow(fetchInfo.Resolution * SymbolSize * 0.5);
        var result = features.Where(f => f != null && (f.Extent?.Intersects(biggerBox) ?? false)).ToList();

        return Task.FromResult((IEnumerable<IFeature>)result);
    }

    /// <summary>
    /// Search for a feature
    /// </summary>
    /// <param name="value">Value to search for</param>
    /// <param name="fieldName">Name of the field to search in. This is the key of the T dictionary</param>
    /// <returns></returns>
    public IFeature? Find(object? value, string fieldName)
    {
        IFeature? result = null;

        if (value == null)
            return result;

        lock (_sync)
        {
            result = _features.FirstOrDefault(f => f[fieldName] == value);
        }

        return result;
    }

    /// <summary>
    /// Get extend of all features provided by this provider
    /// </summary>
    /// <returns>Extent of all features</returns>
    public MRect? GetExtent()
    {
        return _extent;
    }

    /// <summary>
    /// Clear list of features
    /// </summary>
    public void Clear()
    {
        lock (_sync)
        {
            _features = new List<IFeature>();
        }
    }

    /// <summary>
    /// Add a feature to list
    /// </summary>
    /// <param name="feature">Feature to add</param>
    public void Add(IFeature feature)
    {
        lock (_sync)
        {
            _features.Add(feature);

            _extent = GetExtent(_features);
        }
    }

    /// <summary>
    /// Add features to list
    /// </summary>
    /// <param name="features">Features to add</param>
    public void Add(IEnumerable<IFeature> features)
    {
        lock (_sync)
        {
            _features.AddRange(features);

            _extent = GetExtent(_features);
        }
    }

    /// <summary>
    /// Remove feature from list
    /// </summary>
    /// <param name="feature">Feature to remove</param>
    public void Remove(IFeature feature)
    {
        lock (_sync)
        {
            _features.Remove(feature);

            _extent = GetExtent(_features);
        }
    }

    /// <summary>
    /// Remove feature from list
    /// </summary>
    /// <param name="feature">Feature to remove</param>
    public void Remove(IEnumerable<IFeature> features)
    {
        lock (_sync)
        {
            foreach (var feature in features)
                _features.Remove(feature);

            _extent = GetExtent(_features);
        }

    }

    internal static MRect? GetExtent(IReadOnlyList<IFeature> features)
    {
        MRect? extent = null;

        foreach (var feature in features)
        {
            if (feature.Extent == null) 
                continue;

            extent = extent == null
                ? feature.Extent
                : extent.Join(feature.Extent);
        }

        return extent;
    }
}
