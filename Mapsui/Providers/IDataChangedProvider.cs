using Mapsui.Fetcher;

namespace Mapsui.Providers;

public interface IDataChangedProvider
{
    /// <summary>
    /// Event called when the data of provider has changed allowing
    /// listeners to react to this.
    /// </summary>
    event DataChangedEventHandler DataChanged;

    /// <summary>
    /// To indicate the data withing the class has changed. This triggers a DataChanged event.
    /// This is necessary for situations where the class itself can not know about changes to it's data.
    /// </summary>
    void DataHasChanged();
}
