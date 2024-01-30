using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Providers;
using Mapsui.Styles;

namespace Mapsui.Samples.Common.DataBuilders;

public class GeodanOfficesLayerBuilder
{
    public static ILayer Create()
    {
        var geodanAmsterdam = new MPoint(122698, 483922);
        var geodanDenBosch = new MPoint(148949, 411446);
        var location = typeof(GeodanOfficesLayerBuilder).LoadBitmapId("Images.location.png");

        var layer = new Layer
        {
            DataSource = new MemoryProvider(new[] { geodanAmsterdam, geodanDenBosch }.ToFeatures()),
            Style = new SymbolStyle
            {
                BitmapId = location,
                SymbolOffset = new Offset { Y = 64 },
                SymbolScale = 0.25
            },
            Name = "Geodan Offices"
        };
        return layer;
    }
}
