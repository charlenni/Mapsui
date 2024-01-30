using Mapsui.Layers;
using Mapsui.NTS;
using Mapsui.Providers;
using Mapsui.Styles;
using Mapsui.Tiling;
using NetTopologySuite.Geometries;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Mapsui.Samples.Common.Maps.Demo;

public class WriteToLayerSample : ISample
{
    public string Name => "Add Pins";
    public string Category => "Demo";

    [SuppressMessage("IDisposableAnalyzers.Correctness", "IDISP001:Dispose created")]
    [SuppressMessage("IDisposableAnalyzers.Correctness", "IDISP004:Don't ignore created IDisposable")]
    public Task<Map> CreateMapAsync()
    {
        var map = new Map();

        map.Layers.Add(OpenStreetMap.CreateTileLayer());

        var layer = new Layer
        {
            DataSource = new MemoryProvider(),
            Style = SymbolStyles.CreatePinStyle(),
        };
        map.Layers.Add(layer);

        // To notify the map that a redraw is needed.
        ((MemoryProvider)layer.DataSource).DataChanged += (s, e) => layer.DataHasChanged();

        map.Info += (s, e) =>
        {
            if (e.MapInfo?.WorldPosition == null) return;

            // Add a point to the layer using the Info position
            ((MemoryProvider)layer.DataSource).Add(new GeometryFeature
            {
                Geometry = new Point(e.MapInfo.WorldPosition.X, e.MapInfo.WorldPosition.Y)
            });
            return;
        };

        return Task.FromResult(map);
    }
}
