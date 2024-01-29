using Mapsui.Layers;
using Mapsui.Providers;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Mapsui.Tests.Providers;

[TestFixture]
public static class MemoryProviderTests
{
    [Test]
    public static void MemoryProviderEmptyCreator()
    {
        var provider = new MemoryProvider();

        ClassicAssert.True(provider.Features.Count == 0);
    }

    [Test]
    public static void MemoryProviderSingleCreator()
    {
        var feature = new PointFeature(new MPoint(1, 1));
        var provider = new MemoryProvider(feature);

        ClassicAssert.True(provider.Features.Count == 1);
        ClassicAssert.True(provider.Features[0] == feature);
    }

    [Test]
    public static void MemoryProviderMultiCreator()
    {
        var feature1 = new PointFeature(new MPoint(1, 1));
        var feature2 = new PointFeature(new MPoint(2, 2));
        var feature3 = new PointFeature(new MPoint(3, 3));
        var provider = new MemoryProvider(new[] { feature1, feature2, feature3 });

        ClassicAssert.True(provider.Features.Count == 3);
        ClassicAssert.True(provider.Features[0] == feature1);
        ClassicAssert.True(provider.Features[1] == feature2);
        ClassicAssert.True(provider.Features[2] == feature3);
    }

    [Test]
    public static void MemoryProviderClear()
    {
        var feature1 = new PointFeature(new MPoint(1, 1));
        var feature2 = new PointFeature(new MPoint(2, 2));
        var feature3 = new PointFeature(new MPoint(3, 3));
        var provider = new MemoryProvider(new[] { feature1, feature2, feature3 });

        provider.Clear();

        ClassicAssert.True(provider.Features.Count == 0);
    }

    [Test]
    public static void MemoryProviderAddSingle()
    {
        var feature1 = new PointFeature(new MPoint(1, 1));
        var feature2 = new PointFeature(new MPoint(2, 2));
        var feature3 = new PointFeature(new MPoint(3, 3));
        var provider = new MemoryProvider(new[] { feature1, feature2 });

        provider.Add(feature3);

        ClassicAssert.True(provider.Features.Count == 3);
        ClassicAssert.True(provider.Features[0] == feature1);
        ClassicAssert.True(provider.Features[1] == feature2);
        ClassicAssert.True(provider.Features[2] == feature3);
    }

    [Test]
    public static void MemoryProviderAddMultiple()
    {
        var feature1 = new PointFeature(new MPoint(1, 1));
        var feature2 = new PointFeature(new MPoint(2, 2));
        var feature3 = new PointFeature(new MPoint(3, 3));
        var provider = new MemoryProvider(new[] { feature1 });

        provider.Add(feature2);
        provider.Add(feature3);

        ClassicAssert.True(provider.Features.Count == 3);
        ClassicAssert.True(provider.Features[0] == feature1);
        ClassicAssert.True(provider.Features[1] == feature2);
        ClassicAssert.True(provider.Features[2] == feature3);
    }

    [Test]
    public static void MemoryProviderRemoveSingle()
    {
        var feature1 = new PointFeature(new MPoint(1, 1));
        var feature2 = new PointFeature(new MPoint(2, 2));
        var feature3 = new PointFeature(new MPoint(3, 3));
        var provider = new MemoryProvider(new[] { feature1, feature2, feature3 });

        provider.Remove(feature2);

        ClassicAssert.True(provider.Features.Count == 2);
        ClassicAssert.True(provider.Features[0] == feature1);
        ClassicAssert.True(provider.Features[1] == feature3);
    }

    [Test]
    public static void MemoryProviderRemoveMultiple()
    {
        var feature1 = new PointFeature(new MPoint(1, 1));
        var feature2 = new PointFeature(new MPoint(2, 2));
        var feature3 = new PointFeature(new MPoint(3, 3));
        var provider = new MemoryProvider(new[] { feature1, feature2, feature3 });

        provider.Remove(new[] { feature1, feature2 });

        ClassicAssert.True(provider.Features.Count == 1);
        ClassicAssert.True(provider.Features[0] == feature3);
    }
}
