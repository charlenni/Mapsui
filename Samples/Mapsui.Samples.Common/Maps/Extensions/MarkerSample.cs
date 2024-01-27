using BruTile.Wms;
using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Projections;
using Mapsui.Samples.Common.Maps.Styles;
using Mapsui.Styles;
using Mapsui.Tiling;
using Mapsui.Widgets;
using Mapsui.Widgets.ScaleBar;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Mapsui.Samples.Common.Maps.Extensions;

public class MarkerSample : ISample
{
    public string Name => "Marker";
    public string Category => "Extensions";

    private static Timer? _timer;

    public Task<Map> CreateMapAsync()
    {
        return Task.FromResult(CreateMap());
    }

    public static Map CreateMap()
    {
        var map = new Map
        {
            CRS = "EPSG:3857"
        };

        // Add a OSM map
        map.Layers.Add(OpenStreetMap.CreateTileLayer());
        // Add a scalebar
        map.Widgets.Add(new ScaleBarWidget(map) { TextAlignment = Alignment.Center, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Top });
        // Create layer for markers
        using var layer = map.AddCalloutLayer("Marker")
            // Create marker for NYC
            .AddMarker(SphericalMercator.FromLonLat(-73.935242, 40.730610),
                Color.Red,
                scale: 1.0,
                title: "New York City")
            // Create marker for Boston
            .AddMarker(SphericalMercator.FromLonLat(-71.057083, 42.361145),
                Color.LightGreen,
                scale: 0.8,
                title: "Boston",
                subtitle: "MA")
            // Create marker for Washington DC
            .AddMarker(SphericalMercator.FromLonLat(-77.03637, 38.89511),
                color: DemoColor(),
                opacity: 0.7,
                scale: 1.5,
                title: "Washington DC",
                touched: MarkerTouched)
            // Create symbol for Baltimore
            .AddSymbol(SphericalMercator.FromLonLat(-76.609383, 39.299236),
                color: DemoColor(),
                title: "Baltimore")
            // Create symbol for Toms River
            .AddSymbol(SphericalMercator.FromLonLat(-74.198456, 39.954639),
                symbolType: SymbolType.Rectangle,
                color: DemoColor(),
                title: "Toms River",
                subtitle: "NJ");

        // Marker with changable values
        var titleCity = "Philadelphia";
        var marker = layer.CreateMarker(SphericalMercator.FromLonLat(-75.165222, 39.952583), title: titleCity);

        // Create symbol for Albany
        var symbol = layer.CreateSymbol(SphericalMercator.FromLonLat(-73.756233, 42.652580),
                symbolType: SymbolType.Triangle,
                color: DemoColor(),
                scale: 1.0,
                title: "Albany");

        _timer?.Dispose();
        _timer = new Timer((t) => {
            marker.SetColor(DemoColor());
            marker.SetScale(marker.GetTitle().Length >= titleCity.Length ? 0.5 : marker.GetScale() + 0.1);
            marker.SetTitle(marker.GetTitle().Length >= titleCity.Length ? titleCity.Substring(0, 1) : titleCity.Substring(0, marker.GetTitle().Length + 1));
            symbol.SetColor(DemoColor());
        }, null, 1000, 1000);

        // Change a value direct in style
        marker.SetSymbolValue((s) => s.Opacity = 0.8f);

        // Another possibility to change a value direct in style
        if (marker.GetSymbolStyle() is SymbolStyle style) 
            style.Opacity = 0.8f;

        // Show callout for this marker
        marker.ShowCallout(layer);

        var iconSymbol = layer.CreateIconSymbol(SphericalMercator.FromLonLat(-75, 41),
                svg: "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"40\" height=\"40\"><path d=\"M2.945 20.131l-2.667-5.98h2.99l2.829 4.31 15.463-.485L14.232.197h3.987l11.314 17.429 2.613-.027q2.317 0 4.202.593l1.724.566q1.913.647 1.913 1.347 0 .754-1.913 1.401l-1.724.566q-1.886.593-4.283.593l-2.532-.054-11.314 17.456h-3.987l7.327-17.779-15.463-.485-2.829 4.31H.278z\"/></svg>",
                offset: new RelativeOffset(0.5, -0.5),
                scale: 1.0,
                title: "Airplan");

        // Zoom map, so that all markers are visible
        map.Navigator.ZoomToBox(layer.Extent?.Grow(50000));

        return map;
    }

    private static Random _rand = new(1);

    /// <summary>
    /// Create a random color
    /// </summary>
    /// <returns>Color created</returns>
    private static Color DemoColor()
    {
        return new Color(_rand.Next(128, 256), _rand.Next(128, 256), _rand.Next(128, 256));
    }

    /// <summary>
    /// Function called when marker is touched/tapped/clicked
    /// </summary>
    /// <param name="layer">Layer feature belonging too</param>
    /// <param name="feature">Feature that is hit</param>
    /// <param name="args">Parameters from touch event</param>
    private static void MarkerTouched(ILayer layer, IFeature feature, MapInfoEventArgs args)
    {
        if (feature is not PointFeature marker || layer is not MemoryLayer)
            return;

        // Change color of marker
        marker.SetColor(DemoColor())
            // Increase subtitle by one
            .SetSubtitle(String.IsNullOrEmpty(marker.GetSubtitle()) ? "0" : (int.Parse(marker.GetSubtitle()) + 1).ToString())
            // Make callout visible
            .ShowCallout(layer);

        // We handled this event, so there isn't the default handling (show callout) needed
        args.Handled = true;
    }
}
