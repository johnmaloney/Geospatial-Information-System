using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using TileFactory.Tests.Utility;
using Microsoft.Extensions.DependencyInjection;

namespace TileFactory.Tests
{
    [TestClass]
    public class FeatureTests : ATest
    {
        [TestMethod]
        public void initialize_single_geometric_feature_expect_bounding_box_calc()
        {
            var feature = new Feature(Interfaces.GeometryType.Point)
            {
                Geometry = Container.GetService<IConfigurationStrategy>().FromInto<(double X, double Y, double Z)[][]>(simplifiedPointData()),
                Id = "777",
                Tags = new Dictionary<string, object>()
            };

            Assert.AreEqual(0.20630438611111107d, feature.MinGeometry.X);
            Assert.AreEqual(0.37493706948566913d, feature.MinGeometry.Y);
            Assert.AreEqual(0.2053856083333333d, feature.MaxGeometry.X);
            Assert.AreEqual(0.37493706948566913d, feature.MaxGeometry.Y);
        }

        [TestMethod]
        public void initialize_single_geometric_feature_expect_area()
        {
            var feature = new Feature(Interfaces.GeometryType.Point)
            {
                Geometry = Container.GetService<IConfigurationStrategy>().FromInto<(double X, double Y, double Z)[][]>(simplifiedPointData()),
                Id = "777",
                Tags = new Dictionary<string, object>()
            };

            Assert.AreEqual(5.6323187919149476E-09d, feature.Area);
        }

        [TestMethod]
        public void initialize_single_geometric_feature_expect_distance()
        {
            var feature = new Feature(Interfaces.GeometryType.Point)
            {
                Geometry = Container.GetService<IConfigurationStrategy>().FromInto<(double X, double Y, double Z)[][]>(simplifiedPointData()),
                Id = "777",
                Tags = new Dictionary<string, object>()
            };

            Assert.AreEqual(0.0050840068211578382, feature.Distance);
        }

        private string simplifiedPointData()
        {
            return @"[
                      [
                        {
                          'item1': 0.20502623888888888,
                          'item2': 0.37493483177750853,
                          'item3': 0.0
                        },
                        {
                          'item1': 0.2053856083333333,
                          'item2': 0.37493706948566913,
                          'item3': 0.0
                        },
                        {
                          'item1': 0.20630438611111107,
                          'item2': 0.37493748537323368,
                          'item3': 0.0
                        },
                        {
                          'item1': 0.20631998888888886,
                          'item2': 0.37493739704319112,
                          'item3': 0.0
                        },
                        {
                          'item1': 0.20756350555555558,
                          'item2': 0.37493274862932147,
                          'item3': 0.0
                        },
                        {
                          'item1': 0.20502623888888888,
                          'item2': 0.37493483177750853,
                          'item3': 0.0
                        }
                      ]
                    ]";
        }
    }
}
