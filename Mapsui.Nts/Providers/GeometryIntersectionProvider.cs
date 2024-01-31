using Mapsui.Features;
using Mapsui.Layers;
using Mapsui.NTS.Extensions;
using Mapsui.NTS.Features;
using Mapsui.Providers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mapsui.NTS.Providers;

public class GeometryIntersectionProvider : IAsyncProvider, IProviderExtended
{
    private readonly IAsyncProvider _provider;
    private FeatureKeyCreator<(long, MRect)>? _featureKeyCreator;

    public GeometryIntersectionProvider(IAsyncProvider provider)
    {
        _provider = provider;
    }

    public int Id { get; } = BaseLayer.NextId();

    public string? CRS
    {
        get => _provider.CRS;
        set => _provider.CRS = value;
    }

    public FeatureKeyCreator<(long, MRect)> FeatureKeyCreator
    {
        get => _featureKeyCreator ??= new FeatureKeyCreator<(long, MRect)>();
        set => _featureKeyCreator = value;
    }

    public async Task<IEnumerable<IFeature>> GetFeaturesAsync(FetchInfo fetchInfo)
    {
        return IterateFeatures(fetchInfo, await _provider.GetFeaturesAsync(fetchInfo));
    }

    private IEnumerable<IFeature> IterateFeatures(FetchInfo fetchInfo, IEnumerable<IFeature> features)
    {
        var rectangle = fetchInfo.Extent.Grow(fetchInfo.Resolution);
        var polygon = rectangle.ToPolygon();

        foreach (var feature in features)
            if (feature is GeometryFeature geometryFeature)
            {
                var copied = new GeometryFeature(geometryFeature, FeatureId.CreateId(Id, (geometryFeature.Id, rectangle), FeatureKeyCreator.GetKey));
                if (geometryFeature.Geometry != null)
                {
                    copied.Geometry = polygon.Intersection(geometryFeature.Geometry);
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
