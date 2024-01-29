using System;
using System.Collections.Generic;
using Mapsui.Features;

namespace Mapsui;

[Obsolete("Use Info and ILayerFeatureInfo", true)]
public class FeatureInfoEventArgs : EventArgs
{
    public IDictionary<string, IEnumerable<IFeature>>? FeatureInfo { get; set; }
}
