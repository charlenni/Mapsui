using Mapsui.Providers;
using System;

namespace Mapsui.Layers;

public interface IDataChangedLayer
{
    /// <summary>
    /// Event called when the data within the layer has changed allowing
    /// listeners to react to this.
    /// </summary>
    event EventHandler<DataChangedEventArgs> DataChanged;

    /// <summary>
    /// To indicate the data withing the class has changed. This triggers a DataChanged event.
    /// This is necessary for situations where the class itself can not know about changes to it's data.
    /// </summary>
    void DataHasChanged();
}
