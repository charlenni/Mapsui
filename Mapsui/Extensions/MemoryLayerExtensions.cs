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
    #region Marker

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

    #endregion

    #region Symbol

    /// <summary>
    /// Create a new symbol
    /// </summary>
    /// <remarks>
    /// A symbol always belongs to a <see cref="MemoryLayer"/>. The difference between 
    /// CreateSymbol and <see cref="AddSymbol"/> is, that CreateSymbol returns the symbol
    /// while AddSymbol returns the MemoryLayer.
    /// </remarks>
    /// <param name="layer">Layer to use</param>
    /// <param name="x">X position</param>
    /// <param name="y">Y position</param>
    /// <param name="color">Color of symbol</param>
    /// <param name="opacity">Opacity of symbol</param>
    /// <param name="scale">Scale of symbol</param>
    /// <param name="title">Title for callout</param>
    /// <param name="subtitle">Subtitle for callout</param>
    /// <param name="touched">Action called when symbol is touched</param>
    /// <returns>Symbol as PointFeature</returns>
    public static PointFeature CreateSymbol(this MemoryLayer layer, double x, double y, SymbolType symbolType = SymbolType.Ellipse, Color? color = null, double opacity = 1.0, double scale = 1.0, string? title = null, string? subtitle = null, Action<ILayer, IFeature, MapInfoEventArgs>? touched = null)
    {
        var symbol = new PointFeature(x, y);

        symbol.InitSymbol(() => layer.DataHasChanged(), symbolType, color, opacity, scale, title, subtitle, touched);

        ((ConcurrentBag<IFeature>)layer.Features).Add(symbol);

        return symbol;
    }

    /// <summary>
    /// Create a new symbol
    /// </summary>
    /// <remarks>
    /// A symbol always belongs to a <see cref="MemoryLayer"/>. The difference between 
    /// CreateSymbol and <see cref="AddSymbol"/> is, that CreateSymbol returns the symbol
    /// while AddSymbol returns the MemoryLayer.
    /// </remarks>
    /// <param name="layer">Layer to use</param>
    /// <param name="x">X position</param>
    /// <param name="y">Y position</param>
    /// <param name="color">Color of symbol</param>
    /// <param name="opacity">Opacity of symbol</param>
    /// <param name="scale">Scale of symbol</param>
    /// <param name="title">Title for callout</param>
    /// <param name="subtitle">Subtitle for callout</param>
    /// <param name="touched">Action called when symbol is touched</param>
    /// <returns>Symbol as PointFeature</returns>
    public static PointFeature CreateSymbol(this MemoryLayer layer, (double x, double y) position, SymbolType symbolType = SymbolType.Ellipse, Color? color = null, double opacity = 1.0, double scale = 1.0, string? title = null, string? subtitle = null, Action<ILayer, IFeature, MapInfoEventArgs>? touched = null)
    {
        return CreateSymbol(layer, position.x, position.y, symbolType, color, opacity, scale, title, subtitle, touched);
    }

    /// <summary>
    /// Create a new symbol
    /// </summary>
    /// <remarks>
    /// A symbol always belongs to a <see cref="MemoryLayer"/>. The difference between 
    /// CreateSymbol and <see cref="AddSymbol"/> is, that CreateSymbol returns the symbol
    /// while AddSymbol returns the MemoryLayer.
    /// </remarks>
    /// <param name="layer">Layer to use</param>
    /// <param name="position">MPoint for position</param>
    /// <param name="color">Color of symbol</param>
    /// <param name="opacity">Opacity of symbol</param>
    /// <param name="scale">Scale of symbol</param>
    /// <param name="title">Title for callout</param>
    /// <param name="subtitle">Subtitle for callout</param>
    /// <param name="touched">Action called when symbol is touched</param>
    /// <returns>Symbol as PointFeature</returns>
    public static PointFeature CreateSymbol(this MemoryLayer layer, MPoint position, SymbolType symbolType = SymbolType.Ellipse, Color? color = null, double opacity = 1.0, double scale = 1.0, string? title = null, string? subtitle = null, Action<ILayer, IFeature, MapInfoEventArgs>? touched = null)
    {
        return CreateSymbol(layer, position.X, position.Y, symbolType, color, opacity, scale, title, subtitle, touched);
    }

    /// <summary>
    /// Add a symbol to the layer
    /// </summary>
    /// <remarks>
    /// A symbol always belongs to a <see cref="MemoryLayer"/>. The difference between 
    /// <see cref="CreateSymbol"/> and AddSymbol is, that CreateSymbol returns the symbol
    /// while AddSymbol returns the MemoryLayer.
    /// </remarks>
    /// <param name="layer">Layer to use</param>
    /// <param name="x">X position</param>
    /// <param name="y">Y position</param>
    /// <param name="color">Color of symbol</param>
    /// <param name="opacity">Opacity of symbol</param>
    /// <param name="scale">Scale of symbol</param>
    /// <param name="title">Title for callout</param>
    /// <param name="subtitle">Subtitle for callout</param>
    /// <param name="touched">Action called when symbol is touched</param>
    /// <returns>MemoryLayer</returns>
    public static MemoryLayer AddSymbol(this MemoryLayer layer, double x, double y, SymbolType symbolType = SymbolType.Ellipse, Color? color = null, double opacity = 1.0, double scale = 1.0, string? title = null, string? subtitle = null, Action<ILayer, IFeature, MapInfoEventArgs>? touched = null)
    {
        var Symbol = CreateSymbol(layer, x, y, symbolType, color, opacity, scale, title, subtitle, touched);

        ((ConcurrentBag<IFeature>)layer.Features).Add(Symbol);

        return layer;
    }

    /// <summary>
    /// Add a symbol to the layer
    /// </summary>
    /// <remarks>
    /// A symbol always belongs to a <see cref="MemoryLayer"/>. The difference between 
    /// <see cref="CreateSymbol"/> and AddSymbol is, that CreateSymbol returns the symbol
    /// while AddSymbol returns the MemoryLayer.
    /// </remarks>
    /// <param name="layer">Layer to use</param>
    /// <param name="x">X position</param>
    /// <param name="y">Y position</param>
    /// <param name="color">Color of symbol</param>
    /// <param name="opacity">Opacity of symbol</param>
    /// <param name="scale">Scale of symbol</param>
    /// <param name="title">Title for callout</param>
    /// <param name="subtitle">Subtitle for callout</param>
    /// <param name="touched">Action called when symbol is touched</param>
    /// <returns>MemoryLayer</returns>
    public static MemoryLayer AddSymbol(this MemoryLayer layer, (double x, double y) position, SymbolType symbolType = SymbolType.Ellipse, Color? color = null, double opacity = 1.0, double scale = 1.0, string? title = null, string? subtitle = null, Action<ILayer, IFeature, MapInfoEventArgs>? touched = null)
    {
        return AddSymbol(layer, position.x, position.y, symbolType, color, opacity, scale, title, subtitle, touched);
    }

    /// <summary>
    /// Add a symbol to the layer
    /// </summary>
    /// <remarks>
    /// A symbol always belongs to a <see cref="MemoryLayer"/>. The difference between 
    /// <see cref="CreateSymbol"/> and AddSymbol is, that CreateSymbol returns the symbol
    /// while AddSymbol returns the MemoryLayer.
    /// </remarks>
    /// <param name="layer">Layer to use</param>
    /// <param name="position">MPoint for position</param>
    /// <param name="color">Color of symbol</param>
    /// <param name="opacity">Opacity of symbol</param>
    /// <param name="scale">Scale of symbol</param>
    /// <param name="title">Title for callout</param>
    /// <param name="subtitle">Subtitle for callout</param>
    /// <param name="touched">Action called when symbol is touched</param>
    /// <returns>MemoryLayer</returns>
    public static MemoryLayer AddSymbol(this MemoryLayer layer, MPoint position, SymbolType symbolType = SymbolType.Ellipse, Color? color = null, double opacity = 1.0, double scale = 1.0, string? title = null, string? subtitle = null, Action<ILayer, IFeature, MapInfoEventArgs>? touched = null)
    {
        return AddSymbol(layer, position.X, position.Y, symbolType, color, opacity, scale, title, subtitle, touched);
    }

    #endregion

    /// <summary>
    /// Hide all callouts on this layer
    /// </summary>
    /// <param name="layer">MemoryLayer for which to hide all callouts</param>
    public static void HideAllCallouts(this MemoryLayer layer)
    {
        foreach (var m in layer.Features.Where(f => ((PointFeature)f).IsSpecial() && ((PointFeature)f).HasCallout()))
            ((PointFeature)m).HideCallout();
    }
}
