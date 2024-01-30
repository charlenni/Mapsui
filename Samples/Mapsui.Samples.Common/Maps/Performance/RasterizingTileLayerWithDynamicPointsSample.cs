using Mapsui.Layers;
using Mapsui.Styles;
using Mapsui.Tiling;
using Mapsui.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Mapsui.Tiling.Layers;
using Mapsui.Providers;

#pragma warning disable IDISP004 // Don't ignore created IDisposable

namespace Mapsui.Samples.Common.Maps.Performance;

public class RasterizingTileLayerWithDynamicPointsSample : IMapControlSample
{
    public string Name => "RasterizingTileLayer with Dynamic Points";
    public string Category => "Performance";

    public void Setup(IMapControl mapControl)
    {
        mapControl.Map = CreateMap(mapControl.PixelDensity);
    }

    public static Map CreateMap(float pixelDensity)
    {
        var map = new Map();
        map.Layers.Add(OpenStreetMap.CreateTileLayer());
        map.Layers.Add(new RasterizingTileLayer(CreateRandomPointLayer(), pixelDensity: pixelDensity));
        var extent = map.Layers[1].Extent!.Grow(map.Layers[1].Extent!.Width * 0.1);
        map.Navigator.ZoomToBox(extent);
        return map;
    }

    private static ILayer CreateRandomPointLayer()
    {
        var rnd = new Random(3462); // Fix the random seed so the features don't move after a refresh

        var layer = new Layer()
        {
            DataSource = new MemoryProvider(),
            Name = "Points",
            Style = new SymbolStyle
            {
                SymbolType = SymbolType.Triangle,
                Fill = new Brush(Color.Red)
            },
        };

        ((MemoryProvider)layer.DataSource).DataChanged += (s, e) => layer.DataHasChanged();

        _ = Task.Run(async () =>
        {
            for (var i = 0; i < 100; i++)
            {
                ((MemoryProvider)layer.DataSource).Add(new PointFeature(new MPoint(rnd.Next(0, 5000000), rnd.Next(0, 5000000))));
                await Task.Delay(100);
            }
        });

        return layer;
    }
}
