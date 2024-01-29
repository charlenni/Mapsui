using Mapsui.NTS;

namespace Mapsui.UI.Objects;

public interface IFeatureProvider
{
    GeometryFeature? Feature { get; }
}
