using Mapsui.Features;
using Mapsui.Layers;
using Mapsui.NTS.Extensions;
using Mapsui.NTS.Features;
using Mapsui.Styles;
using NetTopologySuite.Geometries;
using System.Collections.Generic;
using System.Linq;

namespace Mapsui.NTS.Layers;

public class VertexOnlyLayer : BaseLayer
{
    private FeatureKeyCreator<(long, int)>? _featureKeyCreator;
    public override MRect? Extent => Source.Extent;
    public Layer Source { get; }

    public VertexOnlyLayer(Layer source)
    {
        Source = source;
        Source.DataChanged += (_, args) => OnDataChanged(args);
        Style = new SymbolStyle { SymbolScale = 0.5 };
    }

    public FeatureKeyCreator<(long, int)> FeatureKeyCreator
    {
        get => _featureKeyCreator ??= new FeatureKeyCreator<(long, int)>();
        set => _featureKeyCreator = value;
    }

    public override IEnumerable<IFeature> GetFeatures(MRect box, double resolution)
    {
        var features = Source.GetFeatures(box, resolution).Cast<GeometryFeature>().ToList();
        foreach (var feature in features)
        {
            if (feature.Geometry is Point || feature.Geometry is MultiPoint) continue; // Points with a vertex on top confuse me
            if (feature.Geometry != null)
            {
                int count = 0;
                foreach (var vertex in feature.Geometry.MainCoordinates())
                {
                    yield return new GeometryFeature(FeatureId.CreateId(Id, (feature.Id, count), FeatureKeyCreator.GetKey)) { Geometry = new Point(vertex) };
                    count++;
                }
            }
        }
    }
}
