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

namespace TileFactory.Tests
{
    [TestClass]
    // Processing a GeoJSON file into a set of tiles.
    public class TileGenerationTests : ATest
    {
        [TestMethod]
        public void given_feature_in_projected_coords_process_into_basic_tile()
        {
            Container.GetService<MockContextRepository>().TryGetAs<MockTileContext>("base", out MockTileContext context);

            var coloradoFeature = Container.GetService<IConfigurationStrategy>().Into<List<Feature>>("colorado_outline_projected");
            context.TileFeatures = coloradoFeature;

            var retriever = new Retriever();
            var tile = retriever.GetTile(context, 0, 0, 0);
        }
    }
}
