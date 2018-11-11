using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace TileFactory.Tests
{
    [TestClass]
    public class FeatureTests
    {
        [TestMethod]
        public void initialize_single_geometric_feature_expect_bounding_box_calc()
        {
            var feature = new Feature(Interfaces.GeometryType.Point)
            {
                Geometry = new(double X, double Y, double Z)[][] 
                {
                    new(double X, double Y, double Z)[] 
                    {
                        (X: 0.294471d, Y: 0.375915411794357d, Z: 0d)
                    }
                },
                Id = "777",
                Tags = new Dictionary<string, object>()
            };

            Assert.AreEqual(0.294471d, feature.MinGeometry.X);
            Assert.AreEqual(0.375915411794357d, feature.MinGeometry.Y);
            Assert.AreEqual(0.294471d, feature.MaxGeometry.X);
            Assert.AreEqual(0.375915411794357d, feature.MaxGeometry.Y);
        }
    }
}
