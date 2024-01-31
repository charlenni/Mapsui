using System;

namespace Mapsui.Providers;

/// <summary>
/// Data delivered when a provider raise a an event that its data changed
/// </summary>
public class DataChangedEventArgs : EventArgs
{
    public DataChangedEventArgs() : this(null, false, null) { }

    public DataChangedEventArgs(Exception? error, bool cancelled, object? info)
        : this(error, cancelled, info, string.Empty) { }

    public DataChangedEventArgs(Exception? error, bool cancelled, object? info, string layerName)
    {
        Error = error;
        Cancelled = cancelled;
        Info = info;
        LayerName = layerName;
    }

    /// <summary>
    /// Exception that was raised when data changed
    /// </summary>
    public Exception? Error { get; }

    /// <summary>
    /// True, when fetching data is cancelled
    /// </summary>
    public bool Cancelled { get; }

    /// <summary>
    /// Additional informations
    /// </summary>
    public object? Info { get; }

    /// <summary>
    /// Name of layer that this provider belongs too 
    /// </summary>
    public string LayerName { get; }
}
