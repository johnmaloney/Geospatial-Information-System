using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using TileFactory.Utility;
using TileFactory.Interfaces;

namespace TileFactory.Tests
{
    [TestClass]
    public class ProjectionTests : ATest
    {
        [TestMethod]
        public void process_point_into_web_mercator_projection_expect_x_and_y()
        {
            var point = new MockPoint(GeometryType.Point,40.73064d, -73.99044d, 0d);
            var webMercatorProcessor = new WebMercatorProcessor(point);
            Assert.AreEqual(0.294471d, webMercatorProcessor.ProjectedX);
            Assert.AreEqual(0.375915411794357d, webMercatorProcessor.ProjectedY);
        }

        public class MockPoint : IGeospatialItem
        {
            public GeometryType Type { get; private set; }

            public double? Altitude { get; private set; }

            public double Latitude { get; private set; }

            public double Longitude { get; private set; }

            public MockPoint(GeometryType type, double latitude, double longitude, double altitude)
            {
                Type = type;
                Latitude = latitude;
                Longitude = longitude;
                Altitude = altitude;
            }
        }
    }
}
