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
using TileFactory.Interfaces;
using Microsoft.Extensions.FileProviders;
using System.Threading.Tasks;

namespace TileFactory.Tests
{
    [TestClass]
    public class ZoomLevelTests : ATest
    { 
        private static MockTileContext TileContext;
        private static ITileCacheStorage<ITransformedTile> TransformedCache = new MockTransformedCacheStorage();
        private static ITileCacheStorage<ITile> RawCache = new MockRawCacheStorage();

        public ZoomLevelTests()
        {
            // Setup the singleton objects to replicate a server // 
            Container.GetService<MockContextRepository>().TryGetAs("base", out TileContext);
            var coloradoFeature = Container.GetService<IConfigurationStrategy>().Into<List<Feature>>("colorado_outline_projected");
            TileContext.Identifier = "colorado_outline_projected";
        }

        [TestMethod]
        public async Task at_zoom_ZERO_level_expect_proper_extent_coodinates()
        {
            var generator = new Generator(TileContext, RawCache, new LayerInitializationFileService(Container.GetService<IFileProvider>()));
            var retriever = new TileRetrieverService(TransformedCache, TileContext, generator);
            
            var transformed = await retriever.GetTile(0, 0, 0);

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
        public async Task at_zoom_ONE_level_expect_proper_extent_coodinates()
        {
            TransformedCache.Clear();
            RawCache.Clear();
            var generator = new Generator(TileContext, RawCache, new LayerInitializationFileService(Container.GetService<IFileProvider>()));;
            var retriever = new TileRetrieverService(TransformedCache, TileContext, generator);

            var transformed = await retriever.GetTile(1, 0, 0);

            // These values represent the screen conversion from web mercator //
            // based on the extent provided, they should not change //
            Assert.AreEqual(1680, transformed.TransformedFeatures[0].Coordinates[0].X);
            Assert.AreEqual(3071, transformed.TransformedFeatures[0].Coordinates[0].Y);

            Assert.AreEqual(1774, transformed.TransformedFeatures[0].Coordinates[1].X);
            Assert.AreEqual(3071, transformed.TransformedFeatures[0].Coordinates[1].Y);

            Assert.AreEqual(1774, transformed.TransformedFeatures[0].Coordinates[2].X);
            Assert.AreEqual(3189, transformed.TransformedFeatures[0].Coordinates[2].Y);

            Assert.AreEqual(1615, transformed.TransformedFeatures[0].Coordinates[3].X);
            Assert.AreEqual(3189, transformed.TransformedFeatures[0].Coordinates[3].Y);

            Assert.AreEqual(1615, transformed.TransformedFeatures[0].Coordinates[4].X);
            Assert.AreEqual(3071, transformed.TransformedFeatures[0].Coordinates[4].Y);

            Assert.AreEqual(1680, transformed.TransformedFeatures[0].Coordinates[5].X);
            Assert.AreEqual(3071, transformed.TransformedFeatures[0].Coordinates[5].Y);
        }

        [TestMethod]
        public async Task at_zoom_TWO_level_expect_proper_extent_coodinates()
        {
            TransformedCache.Clear();
            RawCache.Clear();
            var generator = new Generator(TileContext, RawCache, new LayerInitializationFileService(Container.GetService<IFileProvider>()));;
            var retriever = new TileRetrieverService(TransformedCache, TileContext, generator);

            var transformed = await retriever.GetTile(2, 0, 1);

            // These values represent the screen conversion from web mercator //
            // based on the extent provided, they should not change //
            Assert.AreEqual(3359, transformed.TransformedFeatures[0].Coordinates[0].X);
            Assert.AreEqual(2047, transformed.TransformedFeatures[0].Coordinates[0].Y);

            Assert.AreEqual(3548, transformed.TransformedFeatures[0].Coordinates[1].X);
            Assert.AreEqual(2047, transformed.TransformedFeatures[0].Coordinates[1].Y);

            Assert.AreEqual(3548, transformed.TransformedFeatures[0].Coordinates[2].X);
            Assert.AreEqual(2282, transformed.TransformedFeatures[0].Coordinates[2].Y);

            Assert.AreEqual(3229, transformed.TransformedFeatures[0].Coordinates[3].X);
            Assert.AreEqual(2281, transformed.TransformedFeatures[0].Coordinates[3].Y);

            Assert.AreEqual(3229, transformed.TransformedFeatures[0].Coordinates[4].X);
            Assert.AreEqual(2047, transformed.TransformedFeatures[0].Coordinates[4].Y);

            Assert.AreEqual(3359, transformed.TransformedFeatures[0].Coordinates[5].X);
            Assert.AreEqual(2047, transformed.TransformedFeatures[0].Coordinates[5].Y);
        }

        [TestMethod]
        public async Task at_zoom_THREE_level_expect_proper_extent_coodinates()
        {
            TransformedCache.Clear();
            RawCache.Clear();
            var generator = new Generator(TileContext, RawCache, new LayerInitializationFileService(Container.GetService<IFileProvider>()));;
            var retriever = new TileRetrieverService(TransformedCache, TileContext, generator);
            var transformed = await retriever.GetTile(3, 1, 3);

            // These values represent the screen conversion from web mercator //
            // based on the extent provided, they should not change //
            Assert.AreEqual(2622, transformed.TransformedFeatures[0].Coordinates[0].X);
            Assert.AreEqual(-2, transformed.TransformedFeatures[0].Coordinates[0].Y);

            Assert.AreEqual(2999, transformed.TransformedFeatures[0].Coordinates[1].X);
            Assert.AreEqual(-3, transformed.TransformedFeatures[0].Coordinates[1].Y);

            Assert.AreEqual(3000, transformed.TransformedFeatures[0].Coordinates[2].X);
            Assert.AreEqual(467, transformed.TransformedFeatures[0].Coordinates[2].Y);

            Assert.AreEqual(2362, transformed.TransformedFeatures[0].Coordinates[3].X);
            Assert.AreEqual(466, transformed.TransformedFeatures[0].Coordinates[3].Y);

            Assert.AreEqual(2362, transformed.TransformedFeatures[0].Coordinates[4].X);
            Assert.AreEqual(-3, transformed.TransformedFeatures[0].Coordinates[4].Y);

            Assert.AreEqual(2622, transformed.TransformedFeatures[0].Coordinates[5].X);
            Assert.AreEqual(-2, transformed.TransformedFeatures[0].Coordinates[5].Y);
        }

        [TestMethod]
        public async Task at_zoom_FOUR_level_expect_proper_extent_coodinates()
        {
            TransformedCache.Clear();
            RawCache.Clear();
            var generator = new Generator(TileContext, RawCache, new LayerInitializationFileService(Container.GetService<IFileProvider>()));;
            var retriever = new TileRetrieverService(TransformedCache, TileContext, generator);

            var transformed = await retriever.GetTile(4, 3, 6);

            // These values represent the screen conversion from web mercator //
            // based on the extent provided, they should not change //
            Assert.AreEqual(1149, transformed.TransformedFeatures[0].Coordinates[0].X);
            Assert.AreEqual(-4, transformed.TransformedFeatures[0].Coordinates[0].Y);

            Assert.AreEqual(1902, transformed.TransformedFeatures[0].Coordinates[1].X);
            Assert.AreEqual(-5, transformed.TransformedFeatures[0].Coordinates[1].Y);

            Assert.AreEqual(1904, transformed.TransformedFeatures[0].Coordinates[2].X);
            Assert.AreEqual(934, transformed.TransformedFeatures[0].Coordinates[2].Y);

            Assert.AreEqual(629, transformed.TransformedFeatures[0].Coordinates[3].X);
            Assert.AreEqual(933, transformed.TransformedFeatures[0].Coordinates[3].Y);

            Assert.AreEqual(630, transformed.TransformedFeatures[0].Coordinates[4].X);
            Assert.AreEqual(665, transformed.TransformedFeatures[0].Coordinates[4].Y);

            Assert.AreEqual(628, transformed.TransformedFeatures[0].Coordinates[5].X);
            Assert.AreEqual(-5, transformed.TransformedFeatures[0].Coordinates[5].Y);

            Assert.AreEqual(1149, transformed.TransformedFeatures[0].Coordinates[6].X);
            Assert.AreEqual(-4, transformed.TransformedFeatures[0].Coordinates[6].Y);
        }

        [TestMethod]
        public async Task at_zoom_FIVE_level_expect_proper_extent_coodinates()
        {
            TransformedCache.Clear();
            RawCache.Clear();
            var generator = new Generator(TileContext, RawCache, new LayerInitializationFileService(Container.GetService<IFileProvider>()));;
            var retriever = new TileRetrieverService(TransformedCache, TileContext, generator);
            var transformed = await retriever.GetTile(5, 6, 12);

            // These values represent the screen conversion from web mercator //
            // based on the extent provided, they should not change //
            Assert.AreEqual(2297, transformed.TransformedFeatures[0].Coordinates[0].X);
            Assert.AreEqual(-9, transformed.TransformedFeatures[0].Coordinates[0].Y);

            Assert.AreEqual(3804, transformed.TransformedFeatures[0].Coordinates[1].X);
            Assert.AreEqual(-11, transformed.TransformedFeatures[0].Coordinates[1].Y);

            Assert.AreEqual(3808, transformed.TransformedFeatures[0].Coordinates[2].X);
            Assert.AreEqual(1868, transformed.TransformedFeatures[0].Coordinates[2].Y);

            Assert.AreEqual(3526, transformed.TransformedFeatures[0].Coordinates[3].X);
            Assert.AreEqual(1865, transformed.TransformedFeatures[0].Coordinates[3].Y);

            Assert.AreEqual(2720, transformed.TransformedFeatures[0].Coordinates[4].X);
            Assert.AreEqual(1868, transformed.TransformedFeatures[0].Coordinates[4].Y);

            Assert.AreEqual(2050, transformed.TransformedFeatures[0].Coordinates[5].X);
            Assert.AreEqual(1869, transformed.TransformedFeatures[0].Coordinates[5].Y);

            Assert.AreEqual(1258, transformed.TransformedFeatures[0].Coordinates[6].X);
            Assert.AreEqual(1866, transformed.TransformedFeatures[0].Coordinates[6].Y);
            
            Assert.AreEqual(1259, transformed.TransformedFeatures[0].Coordinates[7].X);
            Assert.AreEqual(1330, transformed.TransformedFeatures[0].Coordinates[7].Y);

            Assert.AreEqual(1252, transformed.TransformedFeatures[0].Coordinates[8].X);
            Assert.AreEqual(1279, transformed.TransformedFeatures[0].Coordinates[8].Y);

            Assert.AreEqual(1256, transformed.TransformedFeatures[0].Coordinates[9].X);
            Assert.AreEqual(-10, transformed.TransformedFeatures[0].Coordinates[9].Y);

            Assert.AreEqual(2297, transformed.TransformedFeatures[0].Coordinates[10].X);
            Assert.AreEqual(-9, transformed.TransformedFeatures[0].Coordinates[10].Y);
        }

        [TestMethod]
        public async Task at_zoom_SIX_level_expect_proper_extent_coodinates()
        {
            TransformedCache.Clear();
            RawCache.Clear();
            var generator = new Generator(TileContext, RawCache, new LayerInitializationFileService(Container.GetService<IFileProvider>()));;
            var retriever = new TileRetrieverService(TransformedCache, TileContext, generator);
            var transformed = await retriever.GetTile(6,13,24);

            // These values represent the screen conversion from web mercator //
            // based on the extent provided, they should not change //
            Assert.AreEqual(498, transformed.TransformedFeatures[0].Coordinates[0].X);
            Assert.AreEqual(-17, transformed.TransformedFeatures[0].Coordinates[0].Y);

            Assert.AreEqual(3512, transformed.TransformedFeatures[0].Coordinates[1].X);
            Assert.AreEqual(-22, transformed.TransformedFeatures[0].Coordinates[1].Y);

            Assert.AreEqual(3519, transformed.TransformedFeatures[0].Coordinates[2].X);
            Assert.AreEqual(3737, transformed.TransformedFeatures[0].Coordinates[2].Y);

            Assert.AreEqual(3041, transformed.TransformedFeatures[0].Coordinates[3].X);
            Assert.AreEqual(3735, transformed.TransformedFeatures[0].Coordinates[3].Y);

            Assert.AreEqual(2957, transformed.TransformedFeatures[0].Coordinates[4].X);
            Assert.AreEqual(3730, transformed.TransformedFeatures[0].Coordinates[4].Y);

            Assert.AreEqual(1344, transformed.TransformedFeatures[0].Coordinates[5].X);
            Assert.AreEqual(3737, transformed.TransformedFeatures[0].Coordinates[5].Y);

            Assert.AreEqual(1040, transformed.TransformedFeatures[0].Coordinates[6].X);
            Assert.AreEqual(3734, transformed.TransformedFeatures[0].Coordinates[6].Y);

            Assert.AreEqual(4, transformed.TransformedFeatures[0].Coordinates[7].X);
            Assert.AreEqual(3737, transformed.TransformedFeatures[0].Coordinates[7].Y);

            Assert.AreEqual(-2, transformed.TransformedFeatures[0].Coordinates[8].X);
            Assert.AreEqual(3730, transformed.TransformedFeatures[0].Coordinates[8].Y);

            Assert.AreEqual(-64, transformed.TransformedFeatures[0].Coordinates[9].X);
            Assert.AreEqual(3730, transformed.TransformedFeatures[0].Coordinates[9].Y);

            Assert.AreEqual(-64, transformed.TransformedFeatures[0].Coordinates[10].X);
            Assert.AreEqual(-23, transformed.TransformedFeatures[0].Coordinates[10].Y);

            Assert.AreEqual(498, transformed.TransformedFeatures[0].Coordinates[11].X);
            Assert.AreEqual(-17, transformed.TransformedFeatures[0].Coordinates[11].Y);
        }

        [TestMethod]
        public async Task at_zoom_SEVEN_level_expect_proper_extent_coodinates()
        {
            TransformedCache.Clear();
            RawCache.Clear();
            var generator = new Generator(TileContext, RawCache, new LayerInitializationFileService(Container.GetService<IFileProvider>()));;
            var retriever = new TileRetrieverService(TransformedCache, TileContext, generator);
            var transformed = await retriever.GetTile(7, 26, 48);

            // These values represent the screen conversion from web mercator //
            // based on the extent provided, they should not change //
            Assert.AreEqual(997, transformed.TransformedFeatures[0].Coordinates[0].X);
            Assert.AreEqual(-34, transformed.TransformedFeatures[0].Coordinates[0].Y);

            Assert.AreEqual(2941, transformed.TransformedFeatures[0].Coordinates[1].X);
            Assert.AreEqual(-35, transformed.TransformedFeatures[0].Coordinates[1].Y);

            Assert.AreEqual(3463, transformed.TransformedFeatures[0].Coordinates[2].X);
            Assert.AreEqual(-42, transformed.TransformedFeatures[0].Coordinates[2].Y);

            Assert.AreEqual(4160, transformed.TransformedFeatures[0].Coordinates[3].X);
            Assert.AreEqual(-42, transformed.TransformedFeatures[0].Coordinates[3].Y);

            Assert.AreEqual(4160, transformed.TransformedFeatures[0].Coordinates[4].X);
            Assert.AreEqual(4160, transformed.TransformedFeatures[0].Coordinates[4].Y);

            Assert.AreEqual(-64, transformed.TransformedFeatures[0].Coordinates[5].X);
            Assert.AreEqual(4160, transformed.TransformedFeatures[0].Coordinates[5].Y);

            Assert.AreEqual(-64, transformed.TransformedFeatures[0].Coordinates[6].X);
            Assert.AreEqual(-45, transformed.TransformedFeatures[0].Coordinates[6].Y);

            Assert.AreEqual(634, transformed.TransformedFeatures[0].Coordinates[7].X);
            Assert.AreEqual(-43, transformed.TransformedFeatures[0].Coordinates[7].Y);

            Assert.AreEqual(997, transformed.TransformedFeatures[0].Coordinates[8].X);
            Assert.AreEqual(-34, transformed.TransformedFeatures[0].Coordinates[8].Y);
        }

        [TestMethod]
        public async Task with_many_tiled_requests_ensure_the_cache_built()
        {
            TransformedCache.Clear();
            RawCache.Clear();
            var generator = new Generator(TileContext, RawCache, 
                new LayerInitializationFileService(Container.GetService<IFileProvider>()));;
            var retriever = new TileRetrieverService(TransformedCache, TileContext, generator);
            var transformed = await retriever.GetTile(2, 3, 1);
            transformed = await retriever.GetTile(2, 0, 1);
            transformed = await retriever.GetTile(2, 1, 1);
            transformed = await retriever.GetTile(2, 2, 1);
            transformed = await retriever.GetTile(2, 2, 1);

        }
    }   
}
