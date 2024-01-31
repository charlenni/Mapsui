using Mapsui.Providers;

namespace Mapsui.NTS.Providers;

public interface IProviderExtended : IAsyncProvider
{
    int Id { get; }
}
