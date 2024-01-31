using Mapsui.Providers;

namespace Mapsui.Layers;

/// <summary>
/// Interface for layers, that have a provider as DataSource
/// </summary>
/// <typeparam name="T">Object implementing IProvider</typeparam>
public interface IDataSourceLayer<out T> where T : IAsyncProvider
{
    /// <summary>
    /// Provider used as data source
    /// </summary>
    public T? DataSource { get; }
}
