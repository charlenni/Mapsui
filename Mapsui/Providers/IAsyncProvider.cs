using System.Collections.Generic;
using System.Threading.Tasks;
using Mapsui.Features;
using Mapsui.Layers;

namespace Mapsui.Providers;

/// <summary>
/// Interface for async data providers
/// </summary>
public interface IAsyncProvider : IProvider
{
    /// <summary>
    /// Get all features async contained in FetchInfos extend
    /// </summary>
    /// <param name="fetchInfo">FetchInfo to use</param>
    /// <returns>Task to get list of features</returns>
    Task<IEnumerable<IFeature>> GetFeaturesAsync(FetchInfo fetchInfo);
}
