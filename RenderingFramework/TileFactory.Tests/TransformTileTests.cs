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
using Microsoft.Extensions.FileProviders;
using System.Threading.Tasks;

namespace TileFactory.Tests
{
    [TestClass]
    public class TransformTileTests : ATest
    {
        [TestMethod]
        public void given_projected_tile_transform_expect_converted_points()
        {
            var tile = Container.GetService<IConfigurationStrategy>().Into<GeoTile>("colorado_outline_tile");

            var transform = new Transform(4096, 64);
            var transformed = transform.ProcessTile(tile);

            // These values represent the screen conversion from web mercator //
            // based on the extent provided, they should not change //
            Assert.AreEqual(840, transformed.TransformedFeatures[0].Coordinates[0].X);
            Assert.AreEqual(1536, transformed.TransformedFeatures[0].Coordinates[0].Y);
                            
            Assert.AreEqual(887, transformed.TransformedFeatures[0].Coordinates[1].X);
            Assert.AreEqual(1536, transformed.TransformedFeatures[0].Coordinates[1].Y);
                            
            Assert.AreEqual(887, transformed.TransformedFeatures[0].Coordinates[2].X);
            Assert.AreEqual(1594, transformed.TransformedFeatures[0].Coordinates[2].Y);
                            
            Assert.AreEqual(807, transformed.TransformedFeatures[0].Coordinates[3].X);
            Assert.AreEqual(1594, transformed.TransformedFeatures[0].Coordinates[3].Y);
                            
            Assert.AreEqual(807, transformed.TransformedFeatures[0].Coordinates[4].X);
            Assert.AreEqual(1536, transformed.TransformedFeatures[0].Coordinates[4].Y);
                            
            Assert.AreEqual(840, transformed.TransformedFeatures[0].Coordinates[5].X);
            Assert.AreEqual(1536, transformed.TransformedFeatures[0].Coordinates[5].Y);
        }

        [TestMethod]
        public async Task given_projected_outline_retrieve_points_transformed()
        {
            Container.GetService<MockContextRepository>().TryGetAs<MockTileContext>("base", out MockTileContext context);
                        
            var raw = new MockRawCacheStorage();
            var generator = new Generator(context, raw, 
                new LayerInitializationFileService(Container.GetService<IFileProvider>()));

            var transformed = new MockTransformedCacheStorage();
            var retriever = new TileRetrieverService(transformed, context, generator);

            var tile = await retriever.GetTile(0, 0, 0);
            Assert.IsNotNull(tile);
        }
    }
}
