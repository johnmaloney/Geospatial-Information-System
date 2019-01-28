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
using TileFactory.Interfaces;

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
            Assert.AreEqual("FeatureCollection", type);
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
                .ExtendWith(new ParseGeoJsonToFeatures()
                    .IterateWith(new ProjectGeoJSONToGeometric(
                        (geoItem) => new WebMercatorProcessor(geoItem))));

            pipeline.Process(context);

            Assert.AreEqual(2, context.TileFeatures.Count());

            var feature1 = context.TileFeatures.FirstOrDefault(f => f.Id == "400001");
            Assert.AreEqual("400001", feature1.Id);
            Assert.AreEqual("400001", feature1.Tags["stop_id"].ToString());
            Assert.AreEqual("4 AV/E 9 ST", feature1.Tags["stop_name"].ToString());
            Assert.AreEqual(0.294471d, feature1.Geometry[0][0].X);
            Assert.AreEqual(0.375915411794357d, feature1.Geometry[0][0].Y);
            Assert.AreEqual(0, feature1.Geometry[0][0].Z);


            var feature2 = context.TileFeatures.FirstOrDefault(f => f.Id == "400002");
            Assert.AreEqual("400002", feature2.Id);
            Assert.AreEqual("400002", feature2.Tags["stop_id"].ToString());
            Assert.AreEqual("4 AV/E 12 ST", feature2.Tags["stop_name"].ToString());
            Assert.AreEqual(0.29447233333333334d, feature2.Geometry[0][0].X);
            Assert.AreEqual(0.37590819401923736d, feature2.Geometry[0][0].Y);
            Assert.AreEqual(0, feature2.Geometry[0][0].Z);
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
                .ExtendWith(new ParseGeoJsonToFeatures()
                    .IterateWith(new ProjectGeoJSONToGeometric(
                        (geoItem) => new WebMercatorProcessor(geoItem))));

            pipeline.Process(context);
        }

        [TestMethod]
        public void parse_colorado_polygon_through_pipeline_expect_translation_to_geometric_object()
        {
            var geoJSON = Container.GetService<IConfigurationStrategy>().GetJson("colorado_outline");

            var context = new GeoJsonContext(geoJSON)
            {
                MaxZoom = 14,
                Buffer = 64,
                Extent = 4096,
                Tolerance = 3
            };

            var pipeline = new DetermineCollectionsTypePipeline()
                .ExtendWith(new ParseGeoJsonToFeatures()
                    .IterateWith(new ProjectGeoJSONToGeometric(
                        (geoItem) => new WebMercatorProcessor(geoItem)))
                .ExtendWith(new GeometricSimplification()));

            pipeline.Process(context);

            var feature = context.TileFeatures.Single();
            Assert.IsNotNull(feature);

            Assert.AreEqual(386, feature.Geometry[0].Length);
            Assert.AreEqual(0.00027851809900100721d, feature.Area);
            Assert.AreEqual(0.067996893428153737d, feature.Distance);
            Assert.AreEqual(0.21655132222222223d, feature.MaxGeometry.X);
            Assert.AreEqual(0.38925641237479158d, feature.MaxGeometry.Y);
            Assert.AreEqual(0.19705485277777779d, feature.MinGeometry.X);
            Assert.AreEqual(0.374913347992747d, feature.MinGeometry.Y);
            Assert.AreEqual(Interfaces.GeometryType.Polygon, feature.Type);
            Assert.AreEqual(5, feature.Tags.Count);
        }

        [TestMethod]
        public void parse_alabama_polygon_through_pipeline_expect_translation_to_geometric_object()
        {
            var geoJSON = Container.GetService<IConfigurationStrategy>().GetJson("alabama_outline");

            var context = new GeoJsonContext(geoJSON)
            {
                MaxZoom = 14,
                Buffer = 64,
                Extent = 4096,
                Tolerance = 3
            };

            var pipeline = new DetermineCollectionsTypePipeline()
                .ExtendWith(new ParseGeoJsonToFeatures()
                    .IterateWith(new ProjectGeoJSONToGeometric(
                        (geoItem) => new WebMercatorProcessor(geoItem)))
                .ExtendWith(new GeometricSimplification()));

            pipeline.Process(context);

            var feature = context.TileFeatures.Single();
            Assert.IsNotNull(feature);
        }

        [TestMethod]
        public async Task parse_multi_linestring_through_pipeline_expect_features()
        {
            var geoJSON = Container.GetService<IConfigurationStrategy>().GetJson("multi_linestring_sample");

            var context = new GeoJsonContext(geoJSON)
            {
                MaxZoom = 14,
                Buffer = 64,
                Extent = 4096,
                Tolerance = 3
            };

            var pipeline = new DetermineCollectionsTypePipeline()
                .ExtendWith(new ParseGeoJsonToFeatures()
                    .IterateWith(new ProjectGeoJSONToGeometric(
                        (geoItem) => new WebMercatorProcessor(geoItem)))
                .ExtendWith(new GeometricSimplification()));

            await pipeline.Process(context);

            var feature = context.TileFeatures.First();
            var geometry = feature.Geometry.First();
            Assert.AreEqual(5.2325351872911652E-07, feature.Area[0]);
            Assert.AreEqual(5.066394805852692E-06, feature.Distance[0]);
            Assert.AreEqual(GeometryType.MultiLineString, feature.Type);
            Assert.AreEqual(6, geometry.Length);

            geometry = feature.Geometry.Skip(1).First();
            Assert.AreEqual(3.6111155667095662E-07, feature.Area[1]);
            Assert.AreEqual(3.0994415283758237E-06, feature.Distance[1]);
            Assert.AreEqual(GeometryType.MultiLineString, feature.Type);
            Assert.AreEqual(3, geometry.Length);

            geometry = feature.Geometry.Skip(2).First();
            Assert.AreEqual(5.9605864598938352E-07, feature.Area[2]);
            Assert.AreEqual(3.993511200006683E-06, feature.Distance[2]);
            Assert.AreEqual(GeometryType.MultiLineString, feature.Type);
            Assert.AreEqual(5, geometry.Length);

            geometry = feature.Geometry.Skip(3).First();
            Assert.AreEqual(6.8980112467170729E-07, feature.Area[3]);
            Assert.AreEqual(4.4107437134344174E-06, feature.Distance[3]);
            Assert.AreEqual(GeometryType.MultiLineString, feature.Type);
            Assert.AreEqual(4, geometry.Length);
        }
    }
}
