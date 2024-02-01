using Mapsui.Features;
using Mapsui.Layers;
using Mapsui.NTS.Features;
using Mapsui.Providers;
using NetTopologySuite.Geometries;
using NetTopologySuite.Simplify;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mapsui.NTS.Providers;

public class GeometrySimplifyProvider : IAsyncProvider, IProviderExtended
{
    private readonly IAsyncProvider _provider;
    private readonly Func<Geometry, double, Geometry> _simplify;
    private readonly double? _distanceTolerance;
    private FeatureKeyCreator<(long, double)>? _featureKeyCreator;

    public GeometrySimplifyProvider(IAsyncProvider provider, Func<Geometry, double, Geometry>? simplify = null, double? distanceTolerance = null)
    {
        _provider = provider;
        _simplify = simplify ?? TopologyPreservingSimplifier.Simplify;
        _distanceTolerance = distanceTolerance;
    }

    public int Id { get; } = BaseLayer.NextId();

    public string? CRS
    {
        get => _provider.CRS;
        set => _provider.CRS = value;
    }

    public FeatureKeyCreator<(long, double)> FeatureKeyCreator
    {
        get => _featureKeyCreator ??= new FeatureKeyCreator<(long, double)>();
        set => _featureKeyCreator = value;
    }

    /// <summary>
    /// Get all features contained in FetchInfos extent
    /// </summary>
    /// <param name="fetchInfo"></param>
    /// <returns></returns>
    public IEnumerable<IFeature> GetFeatures(FetchInfo fetchInfo)
    {
#pragma warning disable VSTHRD002 // Avoid problematic synchronous waits
        return GetFeaturesAsync(fetchInfo).Result;
#pragma warning restore VSTHRD002 // Avoid problematic synchronous waits
    }

    public async Task<IEnumerable<IFeature>> GetFeaturesAsync(FetchInfo fetchInfo)
    {
        return IterateFeatures(fetchInfo, await _provider.GetFeaturesAsync(fetchInfo));
    }

    private IEnumerable<IFeature> IterateFeatures(FetchInfo fetchInfo, IEnumerable<IFeature> features)
    {
        var resolution = _distanceTolerance ?? fetchInfo.Resolution;
        foreach (var feature in features)
            if (feature is GeometryFeature geometryFeature)
            {
                var copied = new GeometryFeature(geometryFeature, FeatureId.CreateId(Id, (feature.Id, resolution), FeatureKeyCreator.GetKey));
                if (geometryFeature.Geometry != null)
                {
                    copied.Geometry = _simplify(geometryFeature.Geometry, resolution);
                }

                yield return copied;
            }
            else
                yield return feature;
    }

    public MRect? GetExtent()
    {
        return _provider.GetExtent();
    }
}
