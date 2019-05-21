using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using TileFactory.Models;
using TileFactory.Tests.Utility;
using Microsoft.Extensions.DependencyInjection;
using TileFactory.Utility;
using TileFactory.Transforms;
using TileFactory.Tests.Mocks;

namespace TileFactory.Tests
{
    [TestClass]
    public class TransformTileTests : ATest
    {
        [TestMethod]
        public void given_projd_tile_transform_expect_converted_points()
        {
            var tile = Container.GetService<IConfigurationStrategy>().Into<DynamicTile>("colorado_outline_tile");

            var transform = new Transform(4096, 64);
            var transformed = transform.ProcessTile(tile);

            // These values represent the screen conversion from web mercator //
            // based on the extent provided, they should not change //
            Assert.AreEqual((uint)840, transformed.TransformedFeatures[0].X);
            Assert.AreEqual((uint)1536, transformed.TransformedFeatures[0].Y);
                            
            Assert.AreEqual((uint)887, transformed.TransformedFeatures[1].X);
            Assert.AreEqual((uint)1536, transformed.TransformedFeatures[1].Y);
                            
            Assert.AreEqual((uint)887, transformed.TransformedFeatures[2].X);
            Assert.AreEqual((uint)1594, transformed.TransformedFeatures[2].Y);
                            
            Assert.AreEqual((uint)807, transformed.TransformedFeatures[3].X);
            Assert.AreEqual((uint)1594, transformed.TransformedFeatures[3].Y);
                            
            Assert.AreEqual((uint)807, transformed.TransformedFeatures[4].X);
            Assert.AreEqual((uint)1536, transformed.TransformedFeatures[4].Y);
                            
            Assert.AreEqual((uint)840, transformed.TransformedFeatures[5].X);
            Assert.AreEqual((uint)1536, transformed.TransformedFeatures[5].Y);
        }

        [TestMethod]
        public void given_projected_outline_retrieve_points_transformed()
        {
            Container.GetService<MockContextRepository>().TryGetAs<MockTileContext>("base", out MockTileContext context);

            var coloradoFeature = Container.GetService<IConfigurationStrategy>().Into<List<Feature>>("colorado_outline_projected");
            context.TileFeatures = coloradoFeature;

            var raw = new MockRawCacheStorage();
            var transformed = new MockTransformedCacheStorage();
            var retriever = new TileRetriever(transformed, raw, context);

            var tile = retriever.GetTile(0, 0, 0);
        }
    }
}
