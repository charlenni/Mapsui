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
    private readonly object _sync = new object();
    private MRect? _boundingBox;

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
            _boundingBox = GetExtent(Features);
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
            _boundingBox = GetExtent(Features);
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
    /// Get all features contained in FetchInfos extend
    /// </summary>
    /// <param name="fetchInfo">FetchInfo to use</param>
    /// <returns>Task to get list of features</returns>
    public virtual Task<IEnumerable<IFeature>> GetFeaturesAsync(FetchInfo fetchInfo)
    {
        ArgumentNullException.ThrowIfNull(fetchInfo);
        ArgumentNullException.ThrowIfNull(fetchInfo.Extent);

        IFeature[] features;

        lock (_sync)
        {
            features = [.. _features]; // An Array is faster than a List
        }

        fetchInfo = new FetchInfo(fetchInfo);

        // Use a larger extent so that symbols partially outside of the extent are included
        var biggerBox = fetchInfo.Extent?.Grow(fetchInfo.Resolution * SymbolSize * 0.5);

        var result = features.Where(f => f != null && (f.Extent?.Intersects(biggerBox) ?? false)).ToList();

        return Task.FromResult((IEnumerable<IFeature>)result);
    }

    /// <summary>
    /// Get extend of all features provided by this provider
    /// </summary>
    /// <returns>Extend for all features</returns>
    public MRect? GetExtent()
    {
        return _boundingBox;
    }

    internal static MRect? GetExtent(IEnumerable<IFeature> features)
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

    /// <summary>
    /// Search for a feature
    /// </summary>
    /// <param name="value">Value to search for</param>
    /// <param name="fieldName">Name of the field to search in. This is the key of the T dictionary</param>
    /// <returns></returns>
    public IFeature? Find(object? value, string fieldName)
    {
        IFeature? result;

        lock (_sync)
        {
            result = _features.FirstOrDefault(f => value != null && f[fieldName] == value);
        }

        return result;
    }

    /// <summary>
    /// Add a feature to list
    /// </summary>
    /// <param name="feature">Feature to add</param>
    public void Add(IFeature? feature)
    {
        if (feature == null)
            return;

        lock (_sync)
        {
            _features.Add(feature);
            _boundingBox = GetExtent(_features);
        }
    }

    /// <summary>
    /// Add features to list
    /// </summary>
    /// <param name="features">Features to add</param>
    public void AddRange(IEnumerable<IFeature>? features)
    {
        if (features == null)
            return;

        lock (_sync)
        {
            _features.AddRange(features);
            _boundingBox = GetExtent(_features);
        }
    }

    /// <summary>
    /// Remove feature from list
    /// </summary>
    /// <param name="feature">Feature to remove</param>
    public void Remove(IFeature? feature)
    {
        if (feature == null)
            return;

        lock (_sync)
        {
            _features.Remove(feature);
            _boundingBox = GetExtent(_features);
        }
    }

    /// <summary>
    /// Remove features from list
    /// </summary>
    /// <param name="features">Features to remove</param>
    public void RemoveRange(IEnumerable<IFeature>? features)
    {
        if (features == null)
            return;

        lock (_sync)
        {
            foreach (var feature in features)
                _features.Remove(feature);

            _boundingBox = GetExtent(_features);
        }
    }

    /// <summary>
    /// Remove all features from list
    /// </summary>
    public void RemoveAll()
    {
        lock (_sync)
        {
            _features.Clear();
            _boundingBox = null;
        }
    }
}
