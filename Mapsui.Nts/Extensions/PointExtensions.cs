using NetTopologySuite.Geometries;

namespace Mapsui.NTS.Extensions;

public static class PointExtensions
{
    public static MPoint ToMPoint(this Point point)
    {
        return new MPoint(point.X, point.Y);
    }
}
