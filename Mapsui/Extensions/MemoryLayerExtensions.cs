using Mapsui.Features;
using Mapsui.Layers;
using Mapsui.Styles;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Mapsui.Extensions;

/// <summary>
/// Extensions for MemoryLayer
/// </summary>
public static class MemoryLayerExtensions
{
    /// <summary>
    /// Add a <see cref="Marker"/> to the layer
    /// </summary>
    /// <param name="layer">Layer to use</param>
    /// <param name="x">X position</param>
    /// <param name="y">Y position</param>
    /// <param name="color">Color of marker</param>
    /// <param name="scale">Scale of marker</param>
    /// <param name="touched">Action called when marker is touched</param>
    public static MemoryLayer AddMarker(this MemoryLayer layer, double x, double y, MarkerType type = MarkerType.Pin_Circle, Styles.Color? color = null, double scale = 1.0, string? title = null, string? subtitle = null, Action<ILayer, Marker, MapInfoEventArgs>? touched = null)
    {
        var marker = new Marker(x, y, type);

        marker.MarkerType = type;
        marker.Scale = scale;
        marker.Title = title;
        marker.Subtitle = subtitle;
        if (color != null) marker.Color = color;
        if (touched != null) marker.Touched = touched;

        ((ConcurrentBag<IFeature>)layer.Features).Add(marker);

        return layer;
    }

    /// <summary>
    /// Add a <see cref="Marker"/> to the layer
    /// </summary>
    /// <param name="layer">Layer to use</param>
    /// <param name="x">X position</param>
    /// <param name="color">Color of marker</param>
    /// <param name="scale">Scale of marker</param>
    /// <param name="touched">Action called when marker is touched</param>
    /// <param name="y">Y position</param>
    public static MemoryLayer AddMarker(this MemoryLayer layer, (double x, double y) position, MarkerType type = MarkerType.Pin_Circle, Color? color = null, double scale = 1.0, string? title = null, string? subtitle = null, Action<ILayer, Marker, MapInfoEventArgs>? touched = null)
    {
        return AddMarker(layer, position.x, position.y, type, color, scale, title, subtitle, touched);
    }

    /// <summary>
    /// Add a <see cref="Marker"/> to the layer
    /// </summary>
    /// <param name="layer">Layer to use</param>
    /// <param name="point">Point for position</param>
    /// <param name="color">Color of marker</param>
    /// <param name="scale">Scale of marker</param>
    /// <param name="touched">Action called when marker is touched</param>
    public static MemoryLayer AddMarker(this MemoryLayer layer, MPoint position, MarkerType type = MarkerType.Pin_Circle, Color? color = null, double scale = 1.0, string? title = null, string? subtitle = null, Action<ILayer, Marker, MapInfoEventArgs>? touched = null)
    {
        return AddMarker(layer, position.X, position.Y, type, color, scale, title, subtitle, touched);
    }

    /// <summary>
    /// Hide all callouts of <see cref="Marker"/> on this layer
    /// </summary>
    /// <param name="layer"></param>
    public static void HideAllCallouts(this MemoryLayer layer)
    {
        foreach (var m in layer.Features.Where(f => f is Marker && ((Marker)f).HasCallout))
            ((Marker)m).HideCallout();
    }
}
