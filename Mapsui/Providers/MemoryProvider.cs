using Mapsui.Layers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mapsui.Providers;

/// <summary>
/// A Provider that keepos all its features in memory
/// </summary>
public class MemoryProvider : IProvider
{
    private readonly MRect? _boundingBox;
    private readonly object _sync = new object();

    /// <summary>
    /// Initializes a new instance of the MemoryProvider without any features
    /// </summary>
    public MemoryProvider()
    {
        lock (_sync)
        {
            _boundingBox = null;
        }
    }

    /// <summary>
    /// Initializes a new instance of the MemoryProvider with one Feature
    /// </summary>
    /// <param name="feature">Feature to be in this dataSource</param>
    public MemoryProvider(IFeature feature)
    {
        lock (_sync)
        {
            _features = [feature];
            _boundingBox = GetExtent(_features);
        }
    }

    /// <summary>
    /// Initializes a new instance of the MemoryProvider with a range of Features
    /// </summary>
    /// <param name="features">Features to be included in this dataSource</param>
    public MemoryProvider(IEnumerable<IFeature> features)
    {
        lock (_sync)
        {
            _features.AddRange(features.ToList());
            _boundingBox = GetExtent(_features);
        }
    }

    private List<IFeature> _features = new();

    public IReadOnlyList<IFeature> Features => _features.AsReadOnly();

    /// <summary>
    /// Default ymbolSize for increasing the bounding box
    /// </summary>
    public double SymbolSize { get; set; } = 64;

    /// <summary>
    /// The spatial reference ID (CRS)
    /// </summary>
    public string? CRS { get; set; }

    /// <summary>
    /// Get all features belonging to the FetchInfo
    /// </summary>
    /// <param name="fetchInfo">FetchInfo with Extent</param>
    /// <returns>All Features inside the Extent of FetchInfo increased about SymbolSize</returns>
    public virtual Task<IEnumerable<IFeature>> GetFeaturesAsync(FetchInfo fetchInfo)
    {
        ArgumentNullException.ThrowIfNull(fetchInfo);
        ArgumentNullException.ThrowIfNull(fetchInfo.Extent);

        IFeature[] features;

        lock (_sync)
        {
            features = _features.ToArray(); // An Array is faster than a List
        }

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
        return Features.FirstOrDefault(f => value != null && f[fieldName] == value);
    }

    /// <summary>
    /// BoundingBox of data set
    /// </summary>
    /// <returns>BoundingBox</returns>
    public MRect? GetExtent()
    {
        return _boundingBox;
    }

    internal static MRect? GetExtent(IReadOnlyList<IFeature> features)
    {
        MRect? box = null;
        foreach (var feature in features)
        {
            if (feature.Extent == null) continue;
            box = box == null
                ? feature.Extent
                : box.Join(feature.Extent);
        }
        return box;
    }

    public void RemoveAll()
    {
        lock (_sync)
        {
            _features.Clear();
        }
    }
}
