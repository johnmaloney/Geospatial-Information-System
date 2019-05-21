using Microsoft.VisualStudio.TestTools.UnitTesting;
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

        private Feature[] getSimplePointFeatures()
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

            return new Feature[] { feature1, feature2 };
        }

        [TestMethod]
        public void process_geometry_at_16_scale_colorado_expect_all_features()
        {
            var coloradoFeature = Container.GetService<IConfigurationStrategy>().Into<Feature[]>("colorado_outline_projected");

            var clipper = new Clipper();
            var clippedFeatures = clipper.Clip(coloradoFeature, 
                scale:16, 
                k1:2.9921875d, 
                k2: 3.5078125d, 
                axis:Axis.X,
                minAll:0.1970548527777778d, 
                maxAll:0.21655132222222223d);

            var feature = clippedFeatures.FirstOrDefault();
            Assert.IsNotNull(feature);
            var geometry = feature.Geometry;
            Assert.IsNotNull(geometry);
            var points = geometry.FirstOrDefault();
            Assert.IsNotNull(points);
            Assert.AreEqual(386, points.Count());
        }

        [TestMethod]
        public void process_geometry_at_16_scale_colorado_expect_no_features()
        {
            var coloradoFeature = Container.GetService<IConfigurationStrategy>().Into<Feature[]>("colorado_outline_projected");

            var clipper = new Clipper();
            var clippedFeatures = clipper.Clip(coloradoFeature, 
                scale: 16, 
                k1: 3.4921875d, 
                k2: 4.0078125d, 
                axis: Axis.X, 
                minAll: 0.1970548527777778d, 
                maxAll: 0.21655132222222223d);

            Assert.AreEqual(null, clippedFeatures);
        }

        [TestMethod]
        public void process_geometry_at_32_scale_leftside_expect_features()
        {
            var coloradoFeature = Container.GetService<IConfigurationStrategy>().Into<Feature[]>("colorado_outline_projected");

            var clipper = new Clipper();
            var left = clipper.Clip(coloradoFeature.ToArray(), 
                scale: 32, 
                k1: 5.9921875d, 
                k2: 6.5078125d, 
                axis: Axis.X, 
                minAll: 0.1970548527777778d, 
                maxAll: 0.21655132222222223d);

            Assert.AreEqual(104, left.First().Geometry.First().Count());
        }

        [TestMethod]
        public void given_left_clip_process_for_left_tile_expect_geometry()
        {
            var coloradoFeature = Container.GetService<IConfigurationStrategy>().Into<Feature[]>("colorado_outline_projected");

            var clipper = new Clipper();
            var left = clipper.Clip(coloradoFeature.ToArray(), scale: 32, k1: 5.9921875d, k2: 6.5078125d, axis: Axis.X, minAll: 0.1970548527777778d, maxAll: 0.21655132222222223d);

            var topLeft = clipper.Clip(left, scale: 32, k1: 11.9921875d, k2: 12.5078125d, axis: Axis.Y, minAll: 0.374913347992747d, maxAll: 0.3892564123747916d);
            var bottomLeft = clipper.Clip(left, scale: 32, k1: 12.4921875d, k2: 13.0078125d, axis: Axis.Y, minAll: 0.374913347992747d, maxAll: 0.3892564123747916d);

            Assert.AreEqual(104, topLeft.First().Geometry[0].Length);
            Assert.AreEqual(null, bottomLeft);
        }

        [TestMethod]
        public void process_geometry_at_32_scale_rightside_expect_features()
        {
            var coloradoFeature = Container.GetService<IConfigurationStrategy>().Into<Feature[]>("colorado_outline_projected");

            var clipper = new Clipper();
            var right = clipper.Clip(coloradoFeature.ToArray(),
                scale: 32,
                k1: 6.4921875d,
                k2: 7.0078125d,
                axis: Axis.X,
                minAll: 0.1970548527777778d,
                maxAll: 0.21655132222222223d);

            Assert.AreEqual(290, right.First().Geometry.First().Count());
        }


        [TestMethod]
        public void given_right_clip_process_for_right_tile_expect_geometry()
        {
            var coloradoFeature = Container.GetService<IConfigurationStrategy>().Into<Feature[]>("colorado_outline_projected");

            var clipper = new Clipper();
            var right = clipper.Clip(coloradoFeature.ToArray(), scale: 32, k1: 6.4921875d, k2: 7.0078125d, axis: Axis.X, minAll: 0.1970548527777778d, maxAll: 0.21655132222222223d);

            var topRight = clipper.Clip(right, scale: 32, k1: 11.9921875d, k2: 12.5078125d, axis: Axis.Y, minAll: 0.374913347992747d, maxAll: 0.3892564123747916d);
            var bottomRight = clipper.Clip(right, scale: 32, k1: 12.4921875d, k2: 13.0078125d, axis: Axis.Y, minAll: 0.374913347992747d, maxAll: 0.3892564123747916d);

            Assert.AreEqual(290, topRight.First().Geometry[0].Length);
            Assert.AreEqual(null, bottomRight);
        }


        [TestMethod]
        public void given_line_coordinates_use_intersect_expect_intersect_coordinates()
        {
            var a = (X: 0.2031728527777778d, Y: 0.37491622266800473d, Z: 2.2528049337126496e-12d);
            var b = (X: 0.2042948361111111, Y: 0.3749184531864108d, Z: 4.1856163680663426e-14d);
            var k = 0.203369140625d;
            var clipper = new Clipper();
            var intersect = clipper.IntersectX(a, b, k);

            Assert.AreEqual(0.203369140625d, intersect.X);
            Assert.AreEqual(0.37491661289096356d, intersect.Y);
            Assert.AreEqual(1d,intersect.Z);
        }
    }
}
