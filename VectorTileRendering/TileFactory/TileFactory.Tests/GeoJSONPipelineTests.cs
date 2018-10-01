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
        public void parse_geospatial_to_geometric_through_the_pipeline_expect_rendered_data()
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
                .ExtendWith(new ParseGeoJsonToFeatures((geoItem) => new WebMercatorProcessor(geoItem)));

            pipeline.Process(context);

            Assert.AreEqual(2, context.TileFeatures.Count());

            var feature1 = context.TileFeatures.FirstOrDefault(f => f.Id == "400001");
            Assert.AreEqual("400001", feature1.Id);
            Assert.AreEqual("400001", feature1.Tags["stop_id"].ToString());
            Assert.AreEqual("4 AV/E 9 ST", feature1.Tags["stop_name"].ToString());
            Assert.AreEqual(0.294471d, feature1.Geometry[0]);
            Assert.AreEqual(0.375915411794357d, feature1.Geometry[1]);
            Assert.AreEqual(0, feature1.Geometry[2]);


            var feature2 = context.TileFeatures.FirstOrDefault(f => f.Id == "400002");
            Assert.AreEqual("400002", feature2.Id);
            Assert.AreEqual("400002", feature2.Tags["stop_id"].ToString());
            Assert.AreEqual("4 AV/E 12 ST", feature2.Tags["stop_name"].ToString());
            Assert.AreEqual(0.29447233333333334d, feature2.Geometry[0]);
            Assert.AreEqual(0.37590819401923736d, feature2.Geometry[1]);
            Assert.AreEqual(0, feature2.Geometry[2]);
        }

        [TestMethod]
        public void parse_and_wrap_geometry_through_the_pipeline_expect_rendered_data()
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
                .ExtendWith(new ParseGeoJsonToFeatures(
                    (geoItem) => new WebMercatorProcessor(geoItem))
                        .IterateWith(new WrapTileFeatures())
                );

            pipeline.Process(context);
        }
    }
}
