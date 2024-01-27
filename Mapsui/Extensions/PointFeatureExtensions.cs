using Mapsui.Layers;
using Mapsui.Styles;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using Color = Mapsui.Styles.Color;

namespace Mapsui.Extensions;

/// <summary>
/// Extensions for PointFeature
/// </summary>
/// <remarks>
/// This are all extension functions to make it easy to create a <see cref="PointFeature"/>
/// that behaves like a marker or a symbol. They add two styles to each PointFeature for the
/// symbol itself and a possible callout.
/// 
/// If there are special extensions to get or set a value, then use them. They do more than
/// one thing (e.g. setting color creates a new marker or setting subtitle change the callout 
/// type).
/// </remarks>
public static class PointFeatureExtensions
{
    // Const for using to access feature fields
    public const string MarkerKey = "Marker";
    public const string MarkerColorKey = MarkerKey + ".Color";
    public const string SymbolKey = "Symbol";
    public const string IconSymbolKey = "IconSymbol";
    public const string SymbolStyleKey = "SymbolStyle";
    public const string CalloutStyleKey = "CalloutStyle";
    public const string TouchedKey = MarkerKey+".Touched";
    public const string InvalidateKey = MarkerKey + ".Invalidate";

    private const string markerImage = "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"39\" height=\"59\">" +
                          "  <g stroke-width=\"2.935\" stroke-linejoin=\"round\">" +
                          "    <path d=\"M34.719 29.415c4.783-7.514 3.355-17.576-3.413-23.503a17.89 17.89 0 0 0-13.221-4.387c-3.83.308-7.463 1.845-10.367 4.387C.95 11.839-.52 21.858 4.28 29.393c6.1 9.304 11.227 6.878 15.234 28.14 3.986-21.223 9.123-18.837 15.206-28.119z\" fill=\"#color\" stroke=\"#color\"/>" +
                          "    <path d=\"M34.719 29.415c4.783-7.514 3.355-17.576-3.413-23.503a17.89 17.89 0 0 0-13.221-4.387c-3.83.308-7.463 1.845-10.367 4.387C.95 11.839-.52 21.858 4.28 29.393c6.1 9.304 11.227 6.878 15.234 28.14 3.986-21.223 9.123-18.837 15.206-28.119z\" fill=\"none\" stroke=\"#000\" stroke-opacity=\".2\"/>" +
                          "  </g>" +
                          "  <circle cx=\"19.5\" cy=\"19.5\" r=\"7\" fill-opacity=\".4\"/>" +
                          "</svg>";
    private const double markerImageHeight = 59.0;

    private static readonly Regex extractWidth = new Regex("width=\\\"(\\d+)\\\"", RegexOptions.Compiled);
    private static readonly Regex extractHeight = new Regex("height=\\\"(\\d+)\\\"", RegexOptions.Compiled);

    /// <summary>
    /// Init a PointFeature, so that it is a marker
    /// </summary>
    /// <param name="marker">PointFeature to use</param>
    /// <param name="invalidate">Action to call when something is changed via extensions</param>
    /// <param name="color">Color for this marker</param>
    /// <param name="opacity">Opacity for this marker</param>
    /// <param name="scale">Scale for this marker</param>
    /// <param name="title">Title of callout</param>
    /// <param name="subtitle">Subtitle for callout</param>
    /// <param name="touched">Action to call, when this marker is touched</param>
    public static void InitMarker(this PointFeature marker, Action invalidate, Color? color = null, double opacity = 1.0, double scale = 1.0, string? title = null, string? subtitle = null, Action<ILayer, IFeature, MapInfoEventArgs>? touched = null)
    {
        marker[MarkerKey] = true;

        color = color ?? Color.Red;

        Init(marker, invalidate, opacity, scale, title, subtitle, touched);

        SetSymbolValue(marker, (symbolStyle) => symbolStyle.SymbolType = SymbolType.Image);
        SetSymbolValue(marker, (symbolStyle) => symbolStyle.BitmapId = GetPinWithColor(color));
        SetSymbolValue(marker, (symbolStyle) => symbolStyle.SymbolOffset = new RelativeOffset(0.0, 0.5));

        SetCalloutValue(marker, (calloutStyle) => calloutStyle.SymbolOffset = new Offset(0.0, markerImageHeight * scale));

        marker[MarkerColorKey] = color;
    }

    /// <summary>
    /// Init a PointFeature, so that it is a symbol
    /// </summary>
    /// <param name="symbol">PointFeature to use</param>
    /// <param name="invalidate">Action to call when something is changed via extensions</param>
    /// <param name="color">Color for this marker</param>
    /// <param name="opacity">Opacity for this marker</param>
    /// <param name="scale">Scale for this marker</param>
    /// <param name="title">Title of callout</param>
    /// <param name="subtitle">Subtitle for callout</param>
    /// <param name="touched">Action to call, when this marker is touched</param>
    public static void InitSymbol(this PointFeature symbol, Action invalidate, SymbolType symbolType = SymbolType.Ellipse, Color? color = null, double opacity = 1.0, double scale = 1.0, string? title = null, string? subtitle = null, Action<ILayer, IFeature, MapInfoEventArgs>? touched = null)
    {
        // Only ellipse, rectangle or triangle are allowed for symbols
        if (symbolType != SymbolType.Ellipse && symbolType != SymbolType.Rectangle && symbolType != SymbolType.Triangle)
            return;

        symbol[SymbolKey] = true;

        color = color ?? Color.White;

        Init(symbol, invalidate, opacity, scale, title, subtitle, touched);

        SetSymbolValue(symbol, (symbolStyle) => symbolStyle.SymbolType = symbolType);
        SetSymbolValue(symbol, (symbolStyle) => symbolStyle.SymbolOffset = new RelativeOffset(0.0, 0.0));
        SetSymbolValue(symbol, (symbolStyle) => symbolStyle.SymbolScale = 0.5);
        SetSymbolValue(symbol, (symbolStyle) => { if (symbolStyle.Fill != null) symbolStyle.Fill.Color = color; });
        SetSymbolValue(symbol, (symbolStyle) => { if (symbolStyle.Outline != null) symbolStyle.Outline.Color = Color.Black; });
        SetSymbolValue(symbol, (symbolStyle) => { if (symbolStyle.Outline != null) symbolStyle.Outline.Width = 5.0; });

        SetCalloutValue(symbol, (calloutStyle) => calloutStyle.SymbolOffset = new Offset(0.0, 0.0));
    }

    /// <summary>
    /// Init a PointFeature, so that it is a icon symbol
    /// </summary>
    /// <param name="marker">PointFeature to use</param>
    /// <param name="invalidate">Action to call when something is changed via extensions</param>
    /// <param name="color">Color for this icon symbol</param>
    /// <param name="opacity">Opacity for this icon symbol</param>
    /// <param name="scale">Scale for this icon symbol</param>
    /// <param name="title">Title of callout</param>
    /// <param name="subtitle">Subtitle for callout</param>
    /// <param name="touched">Action to call, when this icon symbol is touched</param>
    public static void InitIconSymbol(this PointFeature marker, Action invalidate, string svg, Offset offset, double opacity = 1.0, double scale = 1.0, string? title = null, string? subtitle = null, Action<ILayer, IFeature, MapInfoEventArgs>? touched = null)
    {
        marker[IconSymbolKey] = true;

        Init(marker, invalidate, opacity, scale, title, subtitle, touched);

        SetSymbolValue(marker, (symbolStyle) => symbolStyle.SymbolType = SymbolType.Image);
        SetSymbolValue(marker, (symbolStyle) => symbolStyle.BitmapId = BitmapRegistry.Instance.Register(svg));
        SetSymbolValue(marker, (symbolStyle) => symbolStyle.SymbolOffset = offset);

        // Try to get width and height of svg
        (var width, var height) = ExtractSizeFromSVG(svg);

        var calloutOffset = offset.CalcOffset(width, height);

        SetCalloutValue(marker, (calloutStyle) => calloutStyle.SymbolOffset = new Offset(-calloutOffset.X, (height * 0.5 + calloutOffset.Y) * scale));
    }

    /// <summary>
    /// Check, if feature is a marker
    /// </summary>
    /// <param name="feature">Feature to check</param>
    /// <returns>True, if the feature is a marker</returns>
    public static bool IsMarker(this PointFeature feature)
    {
        return IsOfType(feature, MarkerKey);
    }

    /// <summary>
    /// Check, if feature is a symbol
    /// </summary>
    /// <param name="feature">Feature to check</param>
    /// <returns>True, if the feature is a symbol</returns>
    public static bool IsSymbol(this PointFeature feature)
    {
        return IsOfType(feature, SymbolKey);
    }

    /// <summary>
    /// Check, if feature is a icon symbol
    /// </summary>
    /// <param name="feature">Feature to check</param>
    /// <returns>True, if the feature is a icon symbol</returns>
    public static bool IsIconSymbol(this PointFeature feature)
    {
        return IsOfType(feature, IconSymbolKey);
    }

    /// <summary>
    /// Check, if feature is one of the special features
    /// </summary>
    /// <param name="feature">Feature to check</param>
    /// <returns>True, if the feature is a special one</returns>
    public static bool IsSpecial(this PointFeature feature)
    {
        return IsMarker(feature) || IsSymbol(feature) || IsIconSymbol(feature);
    }

    /// <summary>
    /// Get SymbolStyle used by this special feature
    /// </summary>
    /// <param name="feature">Feature for which to get SymbolStyle</param>
    /// <returns>SymbolStyle used or null</returns>
    public static SymbolStyle? GetSymbolStyle(this PointFeature feature) 
    {
        if (!IsSpecial(feature))
            return null;

        return feature.Get<SymbolStyle>(SymbolStyleKey);
    }

    /// <summary>
    /// Get CalloutStyle used by this special feature
    /// </summary>
    /// <param name="feature">Feature for which to get CalloutStyle</param>
    /// <returns>CalloutStyle used or null</returns>
    public static CalloutStyle? GetCalloutStyle(this PointFeature feature)
    {
        if (!IsSpecial(feature))
            return null;

        return feature.Get<CalloutStyle>(CalloutStyleKey);
    }

    /// <summary>
    /// Get color of this feature
    /// </summary>
    /// <param name="feature">Feature to use</param>
    /// <returns>Color of feature</returns>
    public static Color? GetColor(this PointFeature feature)
    {
        if (IsMarker(feature))
            return feature.Get<Color>(MarkerColorKey);

        if (IsSymbol(feature))
            return feature.Get<SymbolStyle>(SymbolStyleKey)?.Fill?.Color;

        return null;
    }

    /// <summary>
    /// Set color for feature
    /// </summary>
    /// <param name="feature">Feature to use</param>
    /// <param name="color">Color to set</param>
    /// <returns>Feature</returns>
    public static PointFeature SetColor(this PointFeature feature, Color color)
    {
        if (IsMarker(feature))
        {
            SetSymbolValue(feature, (symbolStyle) => symbolStyle.BitmapId = GetPinWithColor(color));
            feature[MarkerColorKey] = color;
        }

        if (IsSymbol(feature))
            SetSymbolValue(feature, (symbolStyle) => { if (symbolStyle.Fill != null) symbolStyle.Fill.Color = color; });

        return feature;
    }

    /// <summary>
    /// Get scale of this feature
    /// </summary>
    /// <param name="feature">Feature to use</param>
    /// <returns>Scale of feature</returns>
    public static double GetScale(this PointFeature feature)
    {
        if (!IsSpecial(feature))
            return 1.0;

        var symbol = feature.Get<SymbolStyle>(SymbolStyleKey);

        if (symbol != null)
        {
            return symbol.SymbolScale;
        }

        return 1.0;
    }

    /// <summary>
    /// Set scale of this feature
    /// </summary>
    /// <param name="feature">Feature to use</param>
    /// <param name="scale">Scale to set</param>
    /// <returns>feature</returns>
    public static PointFeature SetScale(this PointFeature feature, double scale)
    {
        SetSymbolValue(feature, (symbol) => symbol.SymbolScale = scale);
        // When setting scale, also SymbolOffset of CalloutStyle has to be adjusted
        SetCalloutValue(feature, (callout) => callout.SymbolOffset = new Offset(0.0, markerImageHeight * scale));

        return feature;
    }

    /// <summary>
    /// Get title of callout for this feature
    /// </summary>
    /// <param name="feature">Feature to use</param>
    /// <returns>Title from callout of feature</returns>
    public static string GetTitle(this PointFeature feature)
    {
        var callout = feature.Get<CalloutStyle>(CalloutStyleKey);

        return callout?.Title ?? string.Empty;
    }

    /// <summary>
    /// Set title of callout of this feature
    /// </summary>
    /// <param name="feature">Feature to use</param>
    /// <param name="text">Title to set</param>
    /// <returns>Feature</returns>
    public static PointFeature SetTitle(this PointFeature feature, string text)
    {
        SetCalloutValue(feature, (callout) => callout.Title = text);

        return feature;
    }

    /// <summary>
    /// Get subtitle of callout for this feature
    /// </summary>
    /// <param name="feature">Feature to use</param>
    /// <returns>Subtitle from callout of feature</returns>
    public static string GetSubtitle(this PointFeature feature)
    {
        var callout = feature.Get<CalloutStyle>(CalloutStyleKey);

        return callout?.Subtitle ?? string.Empty;
    }

    /// <summary>
    /// Set subtitle of callout of this feature
    /// </summary>
    /// <param name="feature">Feature to use</param>
    /// <param name="text">Subtitle to set</param>
    /// <returns>Feature</returns>
    public static PointFeature SetSubtitle(this PointFeature feature, string text)
    {
        SetCalloutValue(feature, (callout) => { 
            callout.Subtitle = text; 
            callout.Type = String.IsNullOrEmpty(text) ? CalloutType.Single : CalloutType.Detail; 
        });

        return feature;
    }

    /// <summary>
    /// Set a value in SymbolStyle
    /// </summary>
    /// <param name="feature">Feature to use</param>
    /// <param name="action">Action to set value</param>
    public static void SetSymbolValue(this PointFeature feature, Action<SymbolStyle> action)
    {
        var symbol = feature.Get<SymbolStyle>(SymbolStyleKey);

        if (symbol != null)
            action(symbol);

        feature.Get<Action>(InvalidateKey)?.Invoke();
    }

    /// <summary>
    /// Set a value in CalloutStyle
    /// </summary>
    /// <param name="feature">Feature to use</param>
    /// <param name="action">Action to set value</param>
    public static void SetCalloutValue(this PointFeature feature, Action<CalloutStyle> action)
    {
        var callout = feature.Get<CalloutStyle>(CalloutStyleKey);

        if (callout != null)
            action(callout);

        feature.Get<Action>(InvalidateKey)?.Invoke();
    }

    /// <summary>
    /// Show callout of this feature
    /// </summary>
    /// <param name="feature">Feature to use</param>
    /// <param name="layer">Layer this feature belongs to</param>
    /// <returns>Feature</returns>
    public static PointFeature ShowCallout(this PointFeature feature, ILayer layer)
    {
        if (layer is MemoryLayer memoryLayer)
        {
            memoryLayer.HideAllCallouts();
        }

        ChangeCalloutEnabled(feature, true);

        return feature;
    }

    /// <summary>
    /// Hide callout of this feature
    /// </summary>
    /// <param name="feature">Feature to use</param>
    /// <returns>Feature</returns>
    public static PointFeature HideCallout(this PointFeature feature)
    {
        ChangeCalloutEnabled(feature, false);

        return feature;
    }

    /// <summary>
    /// Check, if callout of this feature is visible
    /// </summary>
    /// <param name="feature">Feature to use</param>
    /// <returns>True, if callout of feature is visible</returns>
    public static bool HasCallout(this PointFeature feature)
    {
        var callout = feature.Get<CalloutStyle>(CalloutStyleKey);

        return callout?.Enabled ?? false;
    }

    /// <summary>
    /// Init each type of marker/symbol created with this extensions
    /// </summary>
    /// <param name="feature">PointFeature to use</param>
    /// <param name="invalidate">Action to call when something is changed via extensions</param>
    /// <param name="opacity">Opacity for this marker</param>
    /// <param name="scale">Scale for this marker</param>
    /// <param name="title">Title of callout</param>
    /// <param name="subtitle">Subtitle for callout</param>
    /// <param name="touched">Action to call, when this marker is touched</param>
    private static void Init(this PointFeature feature, Action invalidate, double opacity = 1.0, double scale = 1.0, string? title = null, string? subtitle = null, Action<ILayer, IFeature, MapInfoEventArgs>? touched = null)
    {
        var symbol = new SymbolStyle()
        {
            Enabled = true,
            SymbolScale = scale,
            Opacity = (float)opacity,
        };

        var callout = new CalloutStyle()
        {
            Enabled = false,
            Type = CalloutType.Single,
            ArrowPosition = 0.5f,
            ArrowAlignment = ArrowAlignment.Bottom,
            Padding = new MRect(10, 5),
            Color = Color.Black,
            BackgroundColor = Color.White,
            MaxWidth = 200,
            TitleFontColor = Color.Black,
            TitleTextAlignment = Widgets.Alignment.Center,
            SubtitleFontColor = Color.Black,
            SubtitleTextAlignment = Widgets.Alignment.Center,
        };

        callout.Title = title;
        callout.TitleFont.Size = 16;
        callout.Subtitle = subtitle;
        callout.SubtitleFont.Size = 12;
        callout.Type = String.IsNullOrEmpty(callout.Subtitle) ? CalloutType.Single : CalloutType.Detail;

        feature.Styles.Clear();
        feature.Styles.Add(symbol);
        feature.Styles.Add(callout);

        feature[SymbolStyleKey] = symbol;
        feature[CalloutStyleKey] = callout;

        if (invalidate != null) feature[InvalidateKey] = invalidate;
        if (touched != null) feature[TouchedKey] = touched;
    }

    /// <summary>
    /// Check, if feature is of given type
    /// </summary>
    /// <param name="feature">Feature to check</param>
    /// <returns>True, if the feature is of given type</returns>
    public static bool IsOfType(this PointFeature feature, string key)
    {
        return feature.Fields.Contains(key)
            && feature[SymbolStyleKey] == feature.Styles.First()
            && feature[CalloutStyleKey] == feature.Styles.Skip(1).First();
    }

    /// <summary>
    /// Change the CalloutStyle Enabled flag to a new value
    /// </summary>
    /// <param name="feature">Feature to use</param>
    /// <param name="flag">True, if the callout should be visible, else false</param>
    private static void ChangeCalloutEnabled(PointFeature feature, bool flag)
    {
        SetCalloutValue(feature, (callout) => callout.Enabled = flag);

        feature.Get<Action>(InvalidateKey)?.Invoke();
    }

    /// <summary>
    /// Create a marker image with given color
    /// </summary>
    /// <param name="color">Color to use</param>
    /// <returns>BitmapId for created marker image</returns>
    private static int GetPinWithColor(Color color)
    {
        var colorInHex = $"{color.R:X2}{color.G:X2}{color.B:X2}";

        if (BitmapRegistry.Instance.TryGetBitmapId($"{MarkerKey}_{colorInHex}", out int bitmapId))
            return bitmapId;

        var svg = markerImage.Replace("#color", $"#{colorInHex}");

        return BitmapRegistry.Instance.Register(svg, $"{MarkerKey}_{colorInHex}");
    }

    /// <summary>
    /// Extract width and height from SVG string
    /// </summary>
    /// <param name="svg">String containing SVG icon</param>
    /// <returns>Tupel of width and height, if found in SVG or 0.0</returns>
    private static (double, double) ExtractSizeFromSVG(string svg)
    {
        // Extract width
        var width = 0.0;

        var matches = extractWidth.Matches(svg.ToLower());

        if (matches.Count >= 1)
            width = matches[0].Success ? double.Parse(matches[0].Groups[1].Value ?? "") : 0;

        // Extract height
        var height = 0.0;

        matches = extractHeight.Matches(svg.ToLower());

        if (matches.Count >= 1)
            height = matches[0].Success ? double.Parse(matches[0].Groups[1].Value ?? "") : 0;

        return (width, height);
    }
}
