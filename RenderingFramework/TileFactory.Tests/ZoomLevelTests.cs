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

namespace TileFactory.Tests
{
    [TestClass]
    public class ZoomLevelTests : ATest
    {
        private static MockTileContext TileContext;
        private static IServiceCollection Registrations;
        private static ServiceProvider Services;
        private static ITileCacheStorage<ITransformedTile> TransformedCache;
        private static ITileCacheStorage<ITile> RawCache;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            Registrations = new ServiceCollection();
            Registrations.AddSingleton<MockContextRepository>(new MockContextRepository());
            Registrations.AddSingleton<IConfigurationStrategy>(new ConfigurationStrategy());
            Services = Registrations.BuildServiceProvider();

            // Setup the singleton objects to replicate a server // 
            Services.GetService<MockContextRepository>().TryGetAs("base", out TileContext);
            var coloradoFeature = Services.GetService<IConfigurationStrategy>().Into<List<Feature>>("colorado_outline_projected");
            TileContext.TileFeatures = coloradoFeature;
            
            RawCache = new MockRawCacheStorage();
            TransformedCache = new MockTransformedCacheStorage();
        }

        [TestMethod]
        [Ignore]
        public void at_zoom_ZERO_level_expect_proper_extent_coodinates()
        {
            var retriever = new TileRetriever(TransformedCache, RawCache, TileContext);
            var transformed = retriever.GetTile(0, 0, 0);

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
        [Ignore]
        public void at_zoom_ONE_level_expect_proper_extent_coodinates()
        {
            var retriever = new TileRetriever(TransformedCache, RawCache, TileContext);
            var transformed = retriever.GetTile(1, 0, 0);

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
        [Ignore]
        public void at_zoom_TWO_level_expect_proper_extent_coodinates()
        {
            var retriever = new TileRetriever(TransformedCache, RawCache, TileContext);
            var transformed = retriever.GetTile(2, 0, 0);

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
        [Ignore]
        public void at_zoom_THREE_level_expect_proper_extent_coodinates()
        {
            var retriever = new TileRetriever(TransformedCache, RawCache, TileContext);
            retriever.GetTile(3, 0, 0);
        }
    }
}
