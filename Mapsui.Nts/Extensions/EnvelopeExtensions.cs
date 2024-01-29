using NetTopologySuite.Geometries;

namespace Mapsui.NTS.Extensions;

public static class EnvelopeExtensions
{
    public static MRect ToMRect(this Envelope envelope)
    {
        return new MRect(envelope.MinX, envelope.MinY, envelope.MaxX, envelope.MaxY);
    }
}

