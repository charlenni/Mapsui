using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace Mapsui.NTS.Editing;

public class AddInfo
{
    public GeometryFeature? Feature { get; set; }
    public IList<Coordinate>? Vertices { get; set; }
    public Coordinate? Vertex { get; set; }
}
