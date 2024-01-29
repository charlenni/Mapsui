using System.Collections.Generic;
using NetTopologySuite.Geometries;
using NUnit.Framework;
using Mapsui.NTS.Extensions;
using System;

namespace Mapsui.NTS.Tests;

[TestFixture]
public class CoordinateExtensionsTests
{
    [Test]
    public void ToLineStringShouldNotThrowWhenCoordinatesIsEmpty()
    {
        // Arrange
        IEnumerable<Coordinate> coordinates = new List<Coordinate>();

        // Act & Assert
        Assert.DoesNotThrow(() => coordinates.ToLineString());
    }
}

