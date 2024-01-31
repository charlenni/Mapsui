using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapsui.Features;
using Mapsui.Layers;

namespace Mapsui.Providers;

/// <summary>
/// Provider, that filters results of another provider by a given function
/// </summary>
public class FilteringProvider : IAsyncProvider
{
    private readonly IAsyncProvider _provider;
    private readonly Func<IFeature, bool> _filter;

    public FilteringProvider(IAsyncProvider provider, Func<IFeature, bool> filter)
    {
        _provider = provider;
        _filter = filter;
    }

    public string? CRS
    {
        get => _provider.CRS;
        set => _provider.CRS = value;
    }

    public MRect? GetExtent()
    {
        return _provider.GetExtent();
    }

    /// <summary>
    /// Get all features contained in FetchInfos extend selected by a filter
    /// </summary>
    /// <param name="fetchInfo">FetchInfo to use</param>
    /// <returns>Task to get list of features</returns>
    public async Task<IEnumerable<IFeature>> GetFeaturesAsync(FetchInfo fetchInfo)
    {
        var features = await _provider.GetFeaturesAsync(fetchInfo);
        return features.Where(f => _filter(f));
    }
}
