using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TileFactory.Interfaces;

namespace TileFactory.DataPipeline.GeoJson
{
    /// <summary>
    /// Determine is the GeoJson file contains a Feature or a FeatureCollection
    /// </summary>
    public class DetermineCollectionsTypePipeline : APipe, IPipe
    {
        public override async Task Process(IPipeContext context)
        {
            var dataContext = context as GeoJsonContext;

            if (dataContext == null)
                throw new NotSupportedException("The pipeline context must be of type DataPipelineContext");

            // Save time and space only read the first line to determine if this is a Feature or a //
            // Feature collection //
            string capture = "";
            using (var reader = new StringReader(dataContext.OriginalData))
            {
                bool finishedSearch = false;
                while (!finishedSearch)
                {
                    var character = reader.ReadLine();
                    if (capture.IndexOf(',') < 0)
                        capture += character;
                    else
                        finishedSearch = true;
                }

                if (capture.Contains("FeatureCollection"))
                {
                    dataContext.Features = JsonConvert.DeserializeObject<GeoJSON.Net.Feature.FeatureCollection>(dataContext.OriginalData);

                }
                else if (capture.Contains("Feature"))
                {
                    var feature = JsonConvert.DeserializeObject<GeoJSON.Net.Feature.Feature>(dataContext.OriginalData);
                    dataContext.Features = new GeoJSON.Net.Feature.FeatureCollection(new List<GeoJSON.Net.Feature.Feature> { feature });
                }
                else
                {
                    throw new NotSupportedException("Type of GeoJson data could not be determined.");
                }
            }

            while (this.HasNextPipe)
                await this.NextPipe.Process(context);
        }
    }

    /// <summary>
    /// This a C# implementation of the following Javascript file:
    /// https://github.com/mapbox/geojson-vt
    /// </summary>
    public class ParseGeoJsonToFeatures : APipe, IPipe
    {
        public override async Task Process(IPipeContext context)
        {
            var dataContext = context as GeoJsonContext;

            if (dataContext == null)
                throw new NotSupportedException("The pipeline context must be of type DataPipelineContext");

            if (dataContext.Features == null)
                throw new NotSupportedException("The Features of the context must have a value for the data to be processed.");

            // Get tje MaxZoom = 2^z //
            var zoomSq = 1 << dataContext.MaxZoom;
            dataContext.TileFeatures = ConvertFeatures(dataContext.Features, dataContext.Tolerance);
            
            while (this.HasNextPipe)
                await this.NextPipe.Process(context);
        }

        internal IEnumerable<Feature> ConvertFeatures(FeatureCollection featureCollection, int tolerance)
        {
            var processedFeatures = new List<Feature>();
            foreach (var feature in featureCollection.Features)
            {
                processedFeatures.Add(ConvertFeature(feature, tolerance));
            }
            return processedFeatures;
        }

        internal Feature ConvertFeature(GeoJSON.Net.Feature.Feature geoJsonFeature, int tolerance)
        {
            
            if (geoJsonFeature.Geometry == null)
                throw new NotSupportedException("The Feature must contain Geometry data to be converted");

            var tileFeature = new TileFactory.Feature
            {
                Id = geoJsonFeature.Id, 
                Type = geoJsonFeature.Type.ToString(),
                Tags = geoJsonFeature.Properties
            };

            switch (geoJsonFeature.Geometry.Type)
            {
                case GeoJSON.Net.GeoJSONObjectType.Point:
                    {
                        var geometry = processPoint(geoJsonFeature.Geometry as Point);
                        tileFeature.Geom = geometry;
                        break;
                    }
                default:
                    throw new NotSupportedException($"The Geometry type of {geoJsonFeature.Geometry.Type} is not supported.");
            }
            return tileFeature;
        }

        private double[] processPoint(Point point)
        {
            return new double[] { projectX(point.Coordinates.Latitude), projectY(point.Coordinates.Longitude) };
        }

        private double projectX(double latitude)
        {
            return latitude / 360 + 0.5;
        }

        private double projectY(double longitude)
        {
            var sin = Math.Sin(longitude * Math.PI / 180);
            var y2 = 0.5 - 0.25 * Math.Log((1+ sin) / (1-sin)) / Math.PI;
            return y2 < 0 ? 0 
                : y2 > 1 ? 1 
                : y2; 
        }
    }

    public class WrapTileFeatures : APipe, IPipe
    {
        public override async Task Process(IPipeContext context)
        {
            var dataContext = context as GeoJsonContext;

            if (dataContext == null)
                throw new NotSupportedException("The pipeline context must be of type DataPipelineContext");

            if (dataContext.Features == null)
                throw new NotSupportedException("The Features of the context must have a value for the data to be processed.");

            var wrappedFeatures = WrapFeatures(dataContext.TileFeatures, dataContext.Buffer / dataContext.Extent);

            while (this.HasNextPipe)
                await this.NextPipe.Process(context);
        }

        private IEnumerable<Feature> WrapFeatures(IEnumerable<Feature> unwrappedFeatures, double buffer)
        {
            var left = Clip(unwrappedFeatures, 1, -1, -buffer, 1 + buffer, 0, -1, 2);
        }

        private object Clip(IEnumerable<Feature> features, int scale, int k1, double k2, double axis, )
    }

}
