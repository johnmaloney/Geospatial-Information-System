using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using TileFactory.Tests.Utility;
using TileFactory.DataPipeline;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using TileFactory.DataPipeline.GeoJson;
using System.Linq;
using TileFactory.Utility;
using TileFactory.Tests.Mocks;
using Microsoft.Extensions.FileProviders;
using TileFactory.Layers;

namespace TileFactory.Tests
{
    [TestClass]
    // Processing a GeoJSON file into a set of tiles.
    public class RawTileTests : ATest
    {
        [TestMethod]
        public async Task given_feature_in_projected_coords_process_into_basic_tile()
        {
            Container.GetService<MockContextRepository>().TryGetAs<MockTileContext>("base", out MockTileContext context);
            var accessor = new LayerTileCacheAccessor(()=> null, () => new MockRawCacheStorage());

            var generator = new Generator(context, accessor, new LayerInitializationFileService(Container.GetService<IFileProvider>()));
            await generator.GenerateTile(0, 0, 0);

            var tile = accessor.GetRawTile(context.Identifier, 0, 0, 0);
            Assert.AreEqual(0d, tile.X);
            Assert.AreEqual(0d, tile.Y);
            Assert.AreEqual(1d, tile.ZoomSquared);
            Assert.AreEqual(386, tile.NumberOfPoints);
            Assert.AreEqual(6, tile.NumberOfSimplifiedPoints);
            Assert.AreEqual(1, tile.NumberOfFeatures);
        }

        [TestMethod]
        public void given_feature_in_projected_coords_process_into_second_level_tile()
        {
            Container.GetService<MockContextRepository>().TryGetAs<MockTileContext>("base", out MockTileContext context);

            var coloradoFeature = Container.GetService<IConfigurationStrategy>().Into<List<Feature>>("colorado_outline_projected");
            context.TileFeatures = coloradoFeature;

            var accessor = new LayerTileCacheAccessor(()=> null, () => new MockRawCacheStorage());
            var generator = new Generator(context, accessor, new LayerInitializationFileService(Container.GetService<IFileProvider>()));

            generator.SplitTile(context.TileFeatures.ToArray(), zoom: 0, x: 0, y: 0, currentZoom: 1, currentX: 0, currentY: 0);
            generator.SplitTile(context.TileFeatures.ToArray(), zoom: 1, x: 0, y: 0, currentZoom: 2, currentX: 0, currentY: 1);
            generator.SplitTile(context.TileFeatures.ToArray(), zoom: 2, x: 0, y: 1, currentZoom: 3, currentX: 1, currentY: 3);
            var tile = accessor.GetRawTile(context.Identifier, 0, 0, 0);
            Assert.AreEqual(0d, tile.X);
            Assert.AreEqual(0d, tile.Y);
            Assert.AreEqual(1d, tile.ZoomSquared);
            Assert.AreEqual(386, tile.NumberOfPoints);
            Assert.AreEqual(6, tile.NumberOfSimplifiedPoints);
            Assert.AreEqual(1, tile.NumberOfFeatures);
        }

        [TestMethod]
        //[ExpectedException(typeof(NotSupportedException))]
        public async Task using_multilinestring_to_add_feature_expect_rewind()
        {
            var multiLinestring = Container.GetService<IConfigurationStrategy>().Into<Feature[]>("multi_linestring_sample_projected");
            Container.GetService<MockContextRepository>().TryGetAs<MockTileContext>("base", out MockTileContext context);

            context.TileFeatures = multiLinestring;
            var accessor = new LayerTileCacheAccessor(()=> new MockTransformedCacheStorage(), () => new MockRawCacheStorage());
            var generator = new Generator(context, accessor, new LayerInitializationFileService(Container.GetService<IFileProvider>()));

            var retriever = new TileRetrieverService(accessor, context, generator);

            var tile = await retriever.GetTile(1, 0, 0);

            // This is not supported yet so the tile is null //
            Assert.IsNull(tile);
            //Assert.AreEqual(2, tile.TransformedFeatures.Count());
        }

        [TestMethod]
        public void given_feature_ring_polygon_expect_rewind()
        {
            var ringJSON = @"[
                { 'item1' : 0.20502623888888888, 'item2' : 0.3749348317775085,  'item3' : 1},
                { 'item1' : 0.21652329444444446, 'item2' : 0.3749172753574649,  'item3' : 0.00008042533215241402 }, 
                { 'item1' : 0.21654933333333332, 'item2' : 0.38925412744493454, 'item3' : 0.00033782393398675166 },
                { 'item1' : 0.1970966027777778,  'item2' : 0.38923325611415305, 'item3' : 0.00022927873054644834 },
                { 'item1' : 0.19708312222222218, 'item2' : 0.37492359875955333, 'item3' : 0.000048328257273705895},
                { 'item1' : 0.20502623888888888, 'item2' : 0.3749348317775085,  'item3' : 1} ]";
            var ring = Container.GetService<IConfigurationStrategy>().FromInto<List<(double X, double Y, double Z)>>(ringJSON);

            var accessor = new LayerTileCacheAccessor(()=> null, () => new MockRawCacheStorage());
            var generator = new Generator(new MockTileContext(),accessor,new LayerInitializationFileService(Container.GetService<IFileProvider>()));
            generator.Rewind(ring, true);

            Assert.AreEqual(0.20502623888888888d, ring[0].X);
            Assert.AreEqual(0.37493483177750853d, ring[0].Y);
            Assert.AreEqual(1d, ring[0].Z);

            Assert.AreEqual(0.21652329444444446d, ring[1].X);
            Assert.AreEqual(0.37491727535746489d, ring[1].Y);
            Assert.AreEqual(8.0425332152414016E-05d, ring[1].Z);

            Assert.AreEqual(0.21654933333333332d, ring[2].X);
            Assert.AreEqual(0.38925412744493454d, ring[2].Y);
            Assert.AreEqual(0.00033782393398675166d, ring[2].Z);

            Assert.AreEqual(0.1970966027777778d, ring[3].X);
            Assert.AreEqual(0.38923325611415305d, ring[3].Y);
            Assert.AreEqual(0.00022927873054644834d, ring[3].Z);

            Assert.AreEqual(0.19708312222222218d, ring[4].X);
            Assert.AreEqual(0.37492359875955333d, ring[4].Y);
            Assert.AreEqual(4.8328257273705895E-05d, ring[4].Z);

            Assert.AreEqual(0.20502623888888888d, ring[5].X);
            Assert.AreEqual(0.37493483177750853d, ring[5].Y);
            Assert.AreEqual(1d, ring[5].Z);
        }
    }
}
