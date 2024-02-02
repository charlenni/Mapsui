using Mapsui.Extensions;
using Mapsui.Features;
using Mapsui.Layers;
using Mapsui.Providers;
using Mapsui.Styles;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Mapsui.Tests.Layers;

[TestFixture]
public class ImageLayerTests
{
    private const string ExceptionMessage = "This exception should return on OnDataChange";

    private class FakeProvider : IAsyncProvider
    {
        public string? CRS { get; set; }

        public IEnumerable<IFeature> GetFeatures(FetchInfo fetchInfo)
        {
            return GetFeaturesAsync(fetchInfo).Result;
        }

        public Task<IEnumerable<IFeature>> GetFeaturesAsync(FetchInfo fetchInfo)
        {
            throw new Exception(ExceptionMessage);
        }

        public MRect GetExtent()
        {
            return new MRect(-1, -1, 0, 0);
        }
    }

    [Test]
    public void TestExceptionOnProvider()
    {
        // arrange
        var provider = new FakeProvider();
        using var layer = new AsyncLayer("imageLayer") { DataSource = provider, Style = new RasterStyle() };
        using var map = new Map();
        map.Layers.Add(layer);
        using var waitHandle = new AutoResetEvent(false);
        Exception? exception = null;

        layer.DataChanged += (_, args) =>
        {
            exception = args.Error;
            waitHandle.Go();
        };

        var fetchInfo = new FetchInfo(new MSection(new MRect(-1, -1, 0, 0), 1), null, ChangeType.Discrete);

        // act
        map.RefreshData(fetchInfo);

        // assert
        waitHandle.WaitOne();
        ClassicAssert.AreEqual(ExceptionMessage, exception?.Message);
    }
}
