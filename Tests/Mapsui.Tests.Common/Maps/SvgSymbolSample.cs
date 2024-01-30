using Mapsui.Features;
using Mapsui.Layers;
using Mapsui.Providers;
using Mapsui.Samples.Common;
using Mapsui.Styles;
using Mapsui.UI;
using Mapsui.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;

#pragma warning disable IDISP001 // Dispose created

namespace Mapsui.Tests.Common.Maps;

public class SvgSymbolSample : ISample
{
    public string Name => "Svg Symbol";
    public string Category => "Tests";

    public Task<Map> CreateMapAsync() => Task.FromResult(CreateMap());


    public static Map CreateMap()
    {
        var layer = new Layer
        {
            DataSource = new MemoryProvider(CreateFeatures()),
            Name = "Points with Svg",
            Style = null,
        };

        var map = new Map
        {
            BackColor = Color.FromString("WhiteSmoke"),
        };

        map.Navigator.ZoomToBox(layer.Extent!.Grow(layer.Extent.Width * 2));

        map.Layers.Add(layer);

        return map;
    }

    public static IEnumerable<IFeature> CreateFeatures()
    {
        var pinId = typeof(SvgSymbolSample).LoadSvgId("Resources.Images.Pin.svg");

        return new List<IFeature>
        {
            new PointFeature(new MPoint(50, 50)) {
                Styles = new[] {new SymbolStyle { BitmapId = pinId } }
            },
            new PointFeature(new MPoint(50, 100)) {
                Styles = new[] {new SymbolStyle { BitmapId = pinId } }
            },
            new PointFeature(new MPoint(100, 50)) {
                Styles = new[] {new SymbolStyle { BitmapId = pinId } }
            },
            new PointFeature(new MPoint(100, 100)) {
                Styles = new[] {new SymbolStyle { BitmapId = pinId } }
            }
        };
    }
}
