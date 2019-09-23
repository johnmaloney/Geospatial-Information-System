using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TileFactory;
using TileFactory.Interfaces;
using TileFactory.Layers;
using TileFactory.Serialization;
using TileFactory.Tests.Mocks;
using TileFactory.Tests.Utility;
using TileFactory.Transforms;

namespace TileFactory.Tests
{
    [TestClass]
    /// These tests are a direct representation of the Mapbox Tile specification.
    /// Specification: https://github.com/mapbox/vector-tile-spec/tree/master/2.1#vector-tile-specification
    public class TileSerializationTests : ATest
    {
        [TestMethod]
        [Description("Encoding the Polygon Geometry of a Tile")]
        public void using_polygon_transformed_tile_generate_serialized_tile()
        {
            // the goal of this test is to take a TransformedTile, which is the coordinates of a geometry feature //
            // and turn it into a group of Commands with parameters. //
            var tile = Container.GetService<IConfigurationStrategy>().Into<TransformedTile>("transformed_tile");
            var factory = new ProtoBufSerializationFactory();
            factory.BuildFrom(tile, new MockTileContext());
            var stream = factory.SerializeTile();

            Assert.IsNotNull(stream);
            Assert.AreEqual(47, stream.Length);
        }

        [TestMethod]
        [Description("Encoding the Point Geometry of a Tile")]
        public async Task using_points_tile_generate_serialized_tile()
        {
            Container.GetService<MockContextRepository>().TryGetAs<MockTileContext>("simple_points", out MockTileContext context);

            var accessor = new LayerTileCacheAccessor(()=> new MockTransformedCacheStorage(), () => new MockRawCacheStorage());
            var generator = new Generator(context, accessor,
                new LayerInitializationFileService(Container.GetService<IFileProvider>()));

            var retriever = new TileRetrieverService(accessor, context, generator);

            var tile = await retriever.GetTile(0, 0, 0);
            var factory = new ProtoBufSerializationFactory();
            factory.BuildFrom(tile, new MockTileContext());
            var stream = factory.SerializeTile();

            Assert.IsNotNull(stream);
            Assert.AreEqual(114, stream.Length);
        }
    }
}
