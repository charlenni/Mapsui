using System;

namespace Mapsui.Rendering;

public interface ISymbolCache : IDisposable
{
    MSize? GetSize(int bitmapId); // perhaps use a tuple in C#7

    IBitmapInfo GetOrCreate(int bitmapID);
}
