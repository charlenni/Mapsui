using System.IO;

namespace Mapsui.Extensions.Providers.Wms;

public interface IGetFeatureInfoParser
{
    FeatureInfo ParseWMSResult(string? layerName, Stream result);
}
