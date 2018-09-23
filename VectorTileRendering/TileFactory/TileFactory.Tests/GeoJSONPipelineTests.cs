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

namespace TileFactory.Tests
{
    [TestClass]
    // Processing a GeoJSON file into a set of tiles.
    public class GeoJSONPipelineTests : ATest
    {
        [TestMethod]
        public void process_geojson_file_into_raw_components_expect_iteration()
        {
            var rawGeoJSON = Container.GetService<IConfigurationStrategy>().GetJson("simple_stops");

            var geoJSON = JObject.Parse(rawGeoJSON);
            var type = geoJSON["type"].Value<string>();
            Assert.AreEqual("FeaturesCollection", type);
        }

        [TestMethod]
        public async Task process_geojson_file_using_pipeline_expect_collections()
        {
            var rawGeoJSON = Container.GetService<IConfigurationStrategy>().GetJson("simple_stops");

            var context = new GeoJsonContext(rawGeoJSON);

            var pipe = new DetermineCollectionsTypePipeline();
            await pipe.Process(context);

            Assert.AreEqual(2, context.Features.Features.Count);
        }

        [TestMethod]
        public void build_and_process_geojson_file_through_the_pipeline_expect_rendered_data()
        {
            var geoJSON = Container.GetService<IConfigurationStrategy>().GetJson("simple_stops");

            var context = new GeoJsonContext(geoJSON)
            {
                MaxZoom = 14,
                Buffer = 64,
                Extent = 4096,
                Tolerance = 3
            };

            var pipeline = new DetermineCollectionsTypePipeline()
                .ExtendWith(new ParseGeoJsonToFeatures());

            pipeline.Process(context);

            Assert.AreEqual(2, context.TileFeatures.Count());
        }
    }
}
