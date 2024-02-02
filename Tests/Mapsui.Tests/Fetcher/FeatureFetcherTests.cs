using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Mapsui.Extensions.Provider;
using Mapsui.Features;
using Mapsui.Layers;
using Mapsui.Providers;
using Mapsui.Tests.Utilities;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Mapsui.Tests.Fetcher;

[TestFixture]
public class FeatureFetcherTests
{
    [Test]
    public async Task TestFeatureFetcherDelayAsync()
    {
        /* TODO: Needs an async provider to test
        // arrange
        var location = Path.Combine(AssemblyInfo.AssemblyDirectory, "Resources", "example.tif");
        using var geoTiffProvider = new GeoTiffProvider(location);
        //var extent = new MRect(0, 0, 10, 10);
        using var layer = new AsyncLayer
        {
            DataSource = geoTiffProvider //new MemoryProvider(GenerateRandomPoints(extent, 25))
        };

        var notifications = new List<bool>();
        layer.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(AsyncLayer.Busy))
            {
                notifications.Add(layer.Busy);
            }
        };
        var fetchInfo = new FetchInfo(new MSection(geoTiffProvider.GetExtent(), 1), null, ChangeType.Discrete);

        // act

        // assert
        await Task.Run(() =>
        {
            while (notifications.Count < 2)
            {
                // just wait until we have two
            }
        });
        ClassicAssert.IsTrue(notifications[0]);
        ClassicAssert.IsFalse(notifications[1]);
        */
    }

    private static IEnumerable<IFeature> GenerateRandomPoints(MRect envelope, int count)
    {
        var random = new Random(0);
        var result = new List<PointFeature>();

        for (var i = 0; i < count; i++)
        {
            result.Add(new PointFeature(new MPoint(
                random.NextDouble() * envelope.Width + envelope.Left,
                random.NextDouble() * envelope.Height + envelope.Bottom)));
        }

        return result;
    }
}
