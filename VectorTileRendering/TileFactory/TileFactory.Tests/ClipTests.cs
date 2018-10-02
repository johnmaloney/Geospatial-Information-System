using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TileFactory.Interfaces;
using TileFactory.Utility;
using TileFactory.Utility.Obsolete;

namespace TileFactory.Tests
{
    [TestClass]
    public class ClipTests
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
            var features = this.getFeatures();
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
            var features = this.getFeatures();
            var clipper = new Clipper();
            double buffer = 0.015625d;

            var left = clipper.Clip(features, 1, -1 - buffer, buffer, -1, 2, Axis.X);
            var right = clipper.Clip(features, 1, 1 - buffer, 2 + buffer, -1, 2, Axis.X);

            Assert.AreEqual(0, left.Count());
            Assert.AreEqual(0, right.Count());
        }

        [TestMethod]
        public void process_points_clip_of_feature_expect_clipped_feature()
        {
            var features = this.getFeatures();
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

            var topLeft = clipper.Clip(features, z2, y-k1, y+ k3, minY, maxY, Axis.Y);
            var bottomLeft = clipper.Clip(features, z2,y + k2, y + k4, minY, maxY, Axis.Y);

            // The Top Left will have both points //
            Assert.AreEqual(2, topLeft.Count());
            // The Bottom Left will only have one of the points //
            Assert.AreEqual(1, bottomLeft.Count());

            var single = bottomLeft.Single() as Feature;
            Assert.AreEqual("777", single.Id);
        }

        private IEnumerable<Feature> getFeatures()
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
    }
}
