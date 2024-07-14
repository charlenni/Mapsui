using Mapsui.Layers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mapsui.Providers;

/// <summary>
/// A Provider that keeps all its features in memory
/// </summary>
public class MemoryProvider : IProvider, IList<IFeature>
{
    private readonly object _sync = new object();
    private MRect? _boundingBox;

    /// <summary>
    /// Initializes a new instance of the MemoryProvider without any features
    /// </summary>
    public MemoryProvider()
    {
        lock (_sync)
            _boundingBox = null;
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

    private List<IFeature> _features = [];

    /// <summary>
    /// Readonly list of features
    /// </summary>
    public IReadOnlyList<IFeature> Features => _features.AsReadOnly();

    /// <summary>
    /// Default SymbolSize for increasing the bounding box
    /// </summary>
    public double SymbolSize { get; set; } = 64;

    /// <summary>
    /// The spatial reference ID (CRS)
    /// </summary>
    public string? CRS { get; set; }

    public int Count
    {
        get
        {
            lock (_sync)
                return _features.Count;
        }
    }

    public bool IsReadOnly => false;

    public IFeature this[int index]
    {
        get
        {
            lock (_sync)
                return _features[index];
        }
        set
        {
            lock (_sync)
                _features[index] = value;
        }
    }

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
            // An Array is faster than a List
            features = [.. _features];

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
            result = _features.FirstOrDefault(f => value != null && f[fieldName] == value);

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
    /// <returns>True, if feature was removed</returns>
    public bool Remove(IFeature? feature)
    {
        if (feature == null)
            return false;

        lock (_sync)
        {
            var result = _features.Remove(feature);
            _boundingBox = GetExtent(_features);

            return result;
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
        Clear();
    }

    /// <summary>
    /// Move a feature to a new position
    /// </summary>
    /// <param name="oldIndex">Old position of feature in list of features</param>
    /// <param name="newIndex">New position of feature in list of features</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void Move(int oldIndex, int newIndex)
    {
        lock (_sync)
        {
            if (newIndex < 0 || newIndex >= _features.Count)
                throw new ArgumentOutOfRangeException(nameof(newIndex));

            var feature = _features[oldIndex];

            _features.RemoveAt(oldIndex);

            // Actual index could have shifted due to the removal
            if (newIndex > oldIndex)
                newIndex--;

            _features.Insert(newIndex, feature);
        }
    }

    /// <summary>
    /// Move given feature to a new position
    /// </summary>
    /// <param name="feature">Feature to move</param>
    /// <param name="newIndex">New position of feature in list of features</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void Move(IFeature feature, int newIndex)
    {
        ArgumentNullException.ThrowIfNull(feature);

        lock (_sync)
        {
            if (newIndex < 0 || newIndex >= _features.Count)
                throw new ArgumentOutOfRangeException(nameof(newIndex));

            var oldIndex = _features.IndexOf(feature);

            if (oldIndex > -1)
            {
                _features.RemoveAt(oldIndex);

                // Actual index could have shifted due to the removal
                if (newIndex > oldIndex)
                    newIndex--;

                _features.Insert(newIndex, feature);
            }
        }

    }
    public int IndexOf(IFeature item)
    {
        lock (_sync)
            return _features.IndexOf(item);
    }

    public void Insert(int index, IFeature item)
    {
        lock (_sync)
        {
            _features.Insert(index, item);
            _boundingBox = GetExtent(_features);
        }
    }

    public void RemoveAt(int index)
    {
        lock (_sync)
        {
            _features.RemoveAt(index);
            _boundingBox = GetExtent(_features);
        }
    }

    public void Clear()
    {
        lock (_sync)
        {
            _features.Clear();
            _boundingBox = null;
        }
    }

    public bool Contains(IFeature item)
    {
        lock (_sync)
            return _features.Contains(item);
    }

    public void CopyTo(IFeature[] array, int arrayIndex)
    {
        lock (_sync)
            _features.CopyTo(array, arrayIndex);
    }

    public IEnumerator<IFeature> GetEnumerator()
    {
        return Features.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Features.GetEnumerator();
    }
}
