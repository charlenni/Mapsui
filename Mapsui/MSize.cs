namespace Mapsui;

/// <summary>
/// Class for a size (width/height) in Mapsui
/// </summary>
public class MSize
{
    public double Width { get; set; }
    public double Height { get; set; }

    public MSize() { }

    public MSize(double width, double height)
    {
        Width = width;
        Height = height;
    }

    public MSize(MSize size)
    {
        Width = size.Width;
        Height = size.Height;
    }

    public bool Equals(MSize? size)
    {
        if (size == null)
            return false;

        return Width.Equals(size.Width) && Height.Equals(size.Height);
    }
    public static bool operator ==(MSize? left, MSize? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(MSize? left, MSize? right)
    {
        return !Equals(left, right);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != GetType())
        {
            return false;
        }

        return Equals((MSize)obj);
    }

    public override int GetHashCode()
    {
        return (Width.GetHashCode() * 569) ^ Height.GetHashCode();
    }
}
