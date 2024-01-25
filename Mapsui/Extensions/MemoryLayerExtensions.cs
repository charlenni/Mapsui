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
    /// Create a new marker
    /// </summary>
    /// <remarks>
    /// A marker always belongs to a <see cref="MemoryLayer"/>. The difference between 
    /// CreateMarker and <see cref="AddMarker"/> is, that CreateMarker returns the marker
    /// while AddMarker returns the MemoryLayer.
    /// </remarks>
    /// <param name="layer">Layer to use</param>
    /// <param name="x">X position</param>
    /// <param name="y">Y position</param>
    /// <param name="color">Color of marker</param>
    /// <param name="opacity">Opacity of marker</param>
    /// <param name="scale">Scale of marker</param>
    /// <param name="title">Title for callout</param>
    /// <param name="subtitle">Subtitle for callout</param>
    /// <param name="touched">Action called when marker is touched</param>
    /// <returns>Marker as PointFeature</returns>
    public static PointFeature CreateMarker(this MemoryLayer layer, double x, double y, Color? color = null, double opacity = 1.0, double scale = 1.0, string? title = null, string? subtitle = null, Action<ILayer, IFeature, MapInfoEventArgs>? touched = null)
    {
        var marker = new PointFeature(x, y);

        marker.InitMarker(() => layer.DataHasChanged(), color, opacity, scale, title, subtitle, touched);

        ((ConcurrentBag<IFeature>)layer.Features).Add(marker);

        return marker;
    }

    /// <summary>
    /// Create a new marker
    /// </summary>
    /// <remarks>
    /// A marker always belongs to a <see cref="MemoryLayer"/>. The difference between 
    /// CreateMarker and <see cref="AddMarker"/> is, that CreateMarker returns the marker
    /// while AddMarker returns the MemoryLayer.
    /// </remarks>
    /// <param name="layer">Layer to use</param>
    /// <param name="x">X position</param>
    /// <param name="y">Y position</param>
    /// <param name="color">Color of marker</param>
    /// <param name="opacity">Opacity of marker</param>
    /// <param name="scale">Scale of marker</param>
    /// <param name="title">Title for callout</param>
    /// <param name="subtitle">Subtitle for callout</param>
    /// <param name="touched">Action called when marker is touched</param>
    /// <returns>Marker as PointFeature</returns>
    public static PointFeature CreateMarker(this MemoryLayer layer, (double x, double y) position, Color? color = null, double opacity = 1.0, double scale = 1.0, string? title = null, string? subtitle = null, Action<ILayer, IFeature, MapInfoEventArgs>? touched = null)
    {
        return CreateMarker(layer, position.x, position.y, color, opacity, scale, title, subtitle, touched);
    }

    /// <summary>
    /// Create a new marker
    /// </summary>
    /// <remarks>
    /// A marker always belongs to a <see cref="MemoryLayer"/>. The difference between 
    /// CreateMarker and <see cref="AddMarker"/> is, that CreateMarker returns the marker
    /// while AddMarker returns the MemoryLayer.
    /// </remarks>
    /// <param name="layer">Layer to use</param>
    /// <param name="position">MPoint for position</param>
    /// <param name="color">Color of marker</param>
    /// <param name="opacity">Opacity of marker</param>
    /// <param name="scale">Scale of marker</param>
    /// <param name="title">Title for callout</param>
    /// <param name="subtitle">Subtitle for callout</param>
    /// <param name="touched">Action called when marker is touched</param>
    /// <returns>Marker as PointFeature</returns>
    public static PointFeature CreateMarker(this MemoryLayer layer, MPoint position, Color? color = null, double opacity = 1.0, double scale = 1.0, string? title = null, string? subtitle = null, Action<ILayer, IFeature, MapInfoEventArgs>? touched = null)
    {
        return CreateMarker(layer, position.X, position.Y, color, opacity, scale, title, subtitle, touched);
    }


    /// <summary>
    /// Add a marker to the layer
    /// </summary>
    /// <remarks>
    /// A marker always belongs to a <see cref="MemoryLayer"/>. The difference between 
    /// <see cref="CreateMarker"/> and AddMarker is, that CreateMarker returns the marker
    /// while AddMarker returns the MemoryLayer.
    /// </remarks>
    /// <param name="layer">Layer to use</param>
    /// <param name="x">X position</param>
    /// <param name="y">Y position</param>
    /// <param name="color">Color of marker</param>
    /// <param name="opacity">Opacity of marker</param>
    /// <param name="scale">Scale of marker</param>
    /// <param name="title">Title for callout</param>
    /// <param name="subtitle">Subtitle for callout</param>
    /// <param name="touched">Action called when marker is touched</param>
    /// <returns>MemoryLayer</returns>
    public static MemoryLayer AddMarker(this MemoryLayer layer, double x, double y, Color? color = null, double opacity = 1.0, double scale = 1.0, string? title = null, string? subtitle = null, Action<ILayer, IFeature, MapInfoEventArgs>? touched = null)
    {
        var marker = CreateMarker(layer, x, y, color, opacity, scale, title, subtitle, touched);

        ((ConcurrentBag<IFeature>)layer.Features).Add(marker);

        return layer;
    }

    /// <summary>
    /// Add a marker to layer
    /// </summary>
    /// <remarks>
    /// A marker always belongs to a <see cref="MemoryLayer"/>. The difference between 
    /// <see cref="CreateMarker"/> and AddMarker is, that CreateMarker returns the marker
    /// while AddMarker returns the MemoryLayer.
    /// </remarks>
    /// <param name="layer">Layer to use</param>
    /// <param name="x">X position</param>
    /// <param name="y">Y position</param>
    /// <param name="color">Color of marker</param>
    /// <param name="opacity">Opacity of marker</param>
    /// <param name="scale">Scale of marker</param>
    /// <param name="title">Title for callout</param>
    /// <param name="subtitle">Subtitle for callout</param>
    /// <param name="touched">Action called when marker is touched</param>
    /// <returns>MemoryLayer</returns>
    public static MemoryLayer AddMarker(this MemoryLayer layer, (double x, double y) position, Color? color = null, double opacity = 1.0, double scale = 1.0, string? title = null, string? subtitle = null, Action<ILayer, IFeature, MapInfoEventArgs>? touched = null)
    {
        return AddMarker(layer, position.x, position.y, color, opacity, scale, title, subtitle, touched);
    }

    /// <summary>
    /// Add a marker to the layer
    /// </summary>
    /// <remarks>
    /// A marker always belongs to a <see cref="MemoryLayer"/>. The difference between 
    /// <see cref="CreateMarker"/> and AddMarker is, that CreateMarker returns the marker
    /// while AddMarker returns the MemoryLayer.
    /// </remarks>
    /// <param name="layer">Layer to use</param>
    /// <param name="position">MPoint for position</param>
    /// <param name="color">Color of marker</param>
    /// <param name="opacity">Opacity of marker</param>
    /// <param name="scale">Scale of marker</param>
    /// <param name="title">Title for callout</param>
    /// <param name="subtitle">Subtitle for callout</param>
    /// <param name="touched">Action called when marker is touched</param>
    /// <returns>MemoryLayer</returns>
    public static MemoryLayer AddMarker(this MemoryLayer layer, MPoint position, Color? color = null, double opacity = 1.0, double scale = 1.0, string? title = null, string? subtitle = null, Action<ILayer, IFeature, MapInfoEventArgs>? touched = null)
    {
        return AddMarker(layer, position.X, position.Y, color, opacity, scale, title, subtitle, touched);
    }

    /// <summary>
    /// Hide all callouts on this layer
    /// </summary>
    /// <param name="layer"></param>
    public static void HideAllCallouts(this MemoryLayer layer)
    {
        foreach (var m in layer.Features.Where(f => f.Fields.Contains(PointFeatureExtensions.MarkerKey) && ((PointFeature)f).HasCallout()))
            ((PointFeature)m).HideCallout();
    }
}
