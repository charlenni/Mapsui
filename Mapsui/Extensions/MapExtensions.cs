using Mapsui.Layers;
using Mapsui.Styles;
using System;
using System.Linq;

namespace Mapsui.Extensions;

/// <summary>
/// Extensions for Map
/// </summary>
public static  class MapExtensions
{
    /// <summary>
    /// Add a layer for symbols/markers which could show a callout
    /// </summary>
    /// <remarks>
    /// This layer should be the topmost <see cref="Layer"> in a <see cref="Map">, so that the <see cref="CalloutStyle">
    /// are always on top.
    /// </remarks>
    /// <param name="map">Map to add this layer too</param>
    /// <param name="name">Name of layer</param>
    /// <returns>Created MemoryLayer</returns>
    public static MemoryLayer AddCalloutLayer(this Map map, string name)
    {
        // Create layer
        var layer = new MemoryLayer(name)
        {
            Style = null,
            IsMapInfoLayer = true
        };

        // Set function for sort order
        layer.SortFeatures = (features) => features.OrderBy((f) => ((PointFeature)f).HasCallout()).ThenBy((f) => f.ZOrder).ThenBy((f) => f.Id);

        // Add handling of touches
        map.Info += (object? sender, MapInfoEventArgs args) =>
        {
            if (args.MapInfo?.Feature == null || args.MapInfo.Feature is not PointFeature || !(((PointFeature)args.MapInfo.Feature).IsSpecial())) 
                return;

            // Has the marker an own action to call when it is touched?
            var feature = (PointFeature)args.MapInfo.Feature;
            var action = (Action<ILayer, IFeature, MapInfoEventArgs>?)feature[PointFeatureExtensions.TouchedKey];

            if (action != null)
            {
                action(layer, feature, args);

                // When action handled 
                if (args.Handled)
                {
                    layer.DataHasChanged();

                    return;
                }
            }

            var callout = feature.Styles.Where(s => s is CalloutStyle).First();

            if (callout == null)
                return;

            var hasCallout = callout.Enabled;

            layer.HideAllCallouts();

            if (!hasCallout)
                callout.Enabled = true;

            args.Handled = true;

            layer.DataHasChanged();
        };

        // Add layer to map
        map.Layers.Add(layer);

        return layer;
    }
}
