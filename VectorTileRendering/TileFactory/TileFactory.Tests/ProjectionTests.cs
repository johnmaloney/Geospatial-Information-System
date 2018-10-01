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
            var point = new MockPoint { Coordinates = new MockPosition { Latitude = 40.73064d, Longitude = -73.99044d } };
            var webMercatorProcessor = new WebMercatorProcessor(point);
            Assert.AreEqual(0.294471d, webMercatorProcessor.ProjectedX);
            Assert.AreEqual(0.375915411794357d, webMercatorProcessor.ProjectedY);
        }

        public class MockPoint : IPoint
        {
            public GeometryType Type { get { return GeometryType.Point;} }
            public IPosition Coordinates { get; set; }
        }

        public class MockPosition : IPosition
        {
            public double? Altitude { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
        }
    }
}
