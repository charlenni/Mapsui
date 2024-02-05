namespace Mapsui.Styles;

/// <summary>
/// Offset of images/text from the center of the image/text
/// </summary>
public class Offset
{
    public Offset() { }

    public Offset(double x, double y)
    {
        X = x;
        Y = y;
    }

    public Offset(Offset offset)
    {
        X = offset.X;
        Y = offset.Y;
    }

    public Offset(MPoint point)
    {
        X = point.X;
        Y = point.Y;
    }

    public double X { get; set; }

    public double Y { get; set; }

    public MPoint ToPoint()
    {
        return new MPoint(X, Y);
    }

    /// <summary>
    /// Calculate the real offset respecting width and height
    /// </summary>
    /// <param name="width">Width of the symbol</param>
    /// <param name="height">Height of the symbol</param>
    /// <returns>Calculated offset</returns>
    public virtual Offset CalcOffset(double width, double height)
    {
        return this;
    }

    public override bool Equals(object? obj)
    {
        if (!(obj is Offset offset))
            return false;
        return Equals(offset);
    }

    public bool Equals(Offset? offset)
    {
        if (offset == null)
            return false;

        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (X != offset.X) return false;
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (Y != offset.Y) return false;
        return true;
    }

    public static bool operator ==(Offset? offset1, Offset? offset2)
    {
        return Equals(offset1, offset2);
    }

    public static bool operator !=(Offset? offset1, Offset? offset2)
    {
        return !Equals(offset1, offset2);
    }

    public override int GetHashCode()
    {
        return X.GetHashCode() ^ Y.GetHashCode();
    }
}
