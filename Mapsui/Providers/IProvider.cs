using Mapsui.Features;
using Mapsui.Layers;
using System.Collections.Generic;

namespace Mapsui.Providers;

/// <summary>
/// Interface for data providers
/// </summary>
public interface IProvider
{
    /// <summary>
    /// The spatial reference ID (CRS)
    /// </summary>
    string? CRS { get; set; }

    /// <summary>
    /// Get extend of all features provided by this provider
    /// </summary>
    /// <returns>Extent of all features</returns>
    MRect? GetExtent();

    /// <summary>
    /// Get all features contained in FetchInfos extend
    /// </summary>
    /// <param name="fetchInfo">FetchInfo to use</param>
    /// <returns>Task to get list of features</returns>
    IEnumerable<IFeature> GetFeatures(FetchInfo fetchInfo);
}
