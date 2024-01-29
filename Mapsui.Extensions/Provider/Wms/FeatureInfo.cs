using System.Collections.Generic;

namespace Mapsui.Extensions.Providers.Wms;

public class FeatureInfo
{
    public string? LayerName { get; set; }
    public List<Dictionary<string, string>>? FeatureInfos { get; set; }
}
