using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using TileFactory;
using TileFactory.Interfaces;
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
        [Description("Encoding the Geometry of a Tile")]
        public void using_the_transformed_tile_generate_serialized_tile()
        {
            // the goal of this test is to take a TransformedTile, which is the coordinates of a geometry feature //
            // and turn it into a group of Commands with parameters. //
            var tile = Container.GetService<IConfigurationStrategy>().Into<TransformedTile>("transformed_tile");
            var factory = new ProtoBufSerializationFactory();
            factory.BuildFrom(tile, new MockTileContext());
            factory.SerializeTile();

        }       
    }
}
