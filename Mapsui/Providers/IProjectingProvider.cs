namespace Mapsui.Providers;

public interface IProjectingProvider : IAsyncProvider
{
    /// <summary>
    /// Queries whether a provider supports projection to a certain CRS.
    /// </summary>
    /// <param name="crs">CRS to project to</param>
    /// <returns>True, if it does, false, if it does not, null if it is unknown</returns>
    bool? IsCrsSupported(string crs);
}
