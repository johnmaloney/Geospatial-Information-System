﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TileFactory.Interfaces;
using TileFactory.Utility;
using TileFactory.Utility.Obsolete;
using TileFactory.Tests.Utility;

namespace TileFactory.Tests
{
    [TestClass]
    public class ClipTests : ATest
    {
        [TestMethod]
        public void generate_clip_expect_proper_type_discernment()
        {
            IClipContext clip = new ClipContext(Constants.ClipType.Left, 64d/4096d);
            Assert.AreEqual(0, clip.Axis);
            Assert.AreEqual(-1.015625d, clip.K1);
            Assert.AreEqual(0.015625d, clip.K2);
            Assert.AreEqual(-1, clip.MinAll);
            Assert.AreEqual(2, clip.MaxAll);
            Assert.AreEqual(1, clip.Scale);
            Assert.AreEqual(Constants.ClipType.Left, clip.Type);
        }

        [TestMethod]
        public void process_left_clip_of_feature_expect_clipped_feature()
        {
            var features = this.getSimplePointFeatures();
            var context = new ClipContext(Constants.ClipType.Left, 64d / 4096d);
            foreach (var feature in features)
            {
                var clipper = new Clip(feature, context);
                Assert.IsNull(clipper.ClippedFeature);
            }            
        }

        [TestMethod]
        public void process_points_clip_of_feature_expect_no_clipped_feature()
        {
            var features = this.getSimplePointFeatures();
            var clipper = new Clipper();
            double buffer = 0.015625d;

            var left = clipper.Clip(features, scale:1, k1:(-1 - buffer), k2:buffer, minAll:-1, maxAll:2, axis:Axis.X);
            var right = clipper.Clip(features, scale:1, k1:(1 - buffer), k2:(2 + buffer), minAll:-1, maxAll:2, axis:Axis.X);

            Assert.AreEqual(0, left.Count());
            Assert.AreEqual(0, right.Count());
        }

        [TestMethod]
        public void process_points_clip_of_feature_expect_clipped_feature()
        {
            var features = this.getSimplePointFeatures();
            var clipper = new Clipper();
            double buffer = 0.015625d;
            double z2 = 8192;
            double y = 3079;
            double k1 = 0.0078125d;
            double k2 = 0.4921875d;
            double k3 = 0.5078125d;
            double k4 = 1.0078125d;
            double minY = 0.37590819401923736d;
            double maxY = 0.375915411794357d;

            var topLeft = clipper.Clip(features, z2, y-k1, y+ k3, Axis.Y, minY, maxY);
            var bottomLeft = clipper.Clip(features, z2,y + k2, y + k4, Axis.Y, minY, maxY);

            // The Top Left will have both points //
            Assert.AreEqual(2, topLeft.Count());
            // The Bottom Left will only have one of the points //
            Assert.AreEqual(1, bottomLeft.Count());

            var single = bottomLeft.Single() as Feature;
            Assert.AreEqual("777", single.Id);
        }

        private IEnumerable<Feature> getSimplePointFeatures()
        {
            var feature1 = new Feature(Interfaces.GeometryType.Point)
            {
                Geometry = new(double X, double Y, double Z)[][]
               {
                    new(double X, double Y, double Z)[]
                    {
                        (X: 0.294471d, Y: 0.375915411794357d, Z: 0d)
                    }
               },
                Id = "777"
            };

            var feature2 = new Feature(Interfaces.GeometryType.Point)
            {
                Geometry = new(double X, double Y, double Z)[][]
               {
                    new(double X, double Y, double Z)[]
                    {
                        (X: 0.29447233333333334d, Y: 0.37590819401923736d, Z: 0d)
                    }
               },
                Id = "888"
            };

            return new List<Feature> { feature1, feature2 };
        }

        [TestMethod]
        public void process_geometry_for_colorado_expect_clippng()
        {
            var coloradoFeature = Container.GetService<IConfigurationStrategy>().Into<List<Feature>>("colorado_outline_projected");

            var clipper = new Clipper();
            clipper.Clip(coloradoFeature, 32, 5.9921875d, 6.5078125d, Axis.X, 0.1970548527777778d, 0.21655132222222223d);
        }
    }
}
