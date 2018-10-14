﻿using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TileFactory.Interfaces;
using TileFactory.Utility;

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
        private GeoJsonContext dataContext;

        public override async Task Process(IPipeContext context)
        {
            dataContext = context as GeoJsonContext;

            if (dataContext == null)
                throw new NotSupportedException("The pipeline context must be of type DataPipelineContext");

            if (dataContext.Features == null)
                throw new NotSupportedException("The Features of the context must have a value for the data to be processed.");

            // var z2 = 1 << options.maxZoom, // 2^z
            // features = convert(data, options.tolerance / (z2 * options.extent));

            dataContext.TileFeatures = await ConvertFeatures(dataContext.Features);
            
            while (this.HasNextPipe)
                await this.NextPipe.Process(context);
        }

        internal async Task<IEnumerable<Feature>> ConvertFeatures(FeatureCollection featureCollection)
        {
            var processedFeatures = new List<Feature>();
            //Parallel.ForEach(featureCollection.Features, feature=>
            foreach (var geospatialFeature in featureCollection.Features)
            {
                var geometricFeature = CreateFeature(geospatialFeature);

                await this.Iterate(
                        new GeoJSONIterativeContext(
                            geospatialFeature,
                            geometricFeature, 
                            dataContext.Buffer / dataContext.Extent));

                processedFeatures.Add(geometricFeature);

            }//);
            
            return processedFeatures;
        }

        internal Feature CreateFeature(GeoJSON.Net.Feature.Feature geoJsonFeature)
        {            
            if (geoJsonFeature.Geometry == null)
                throw new NotSupportedException("The Feature must contain Geometry data to be converted");

            var tileFeature = new TileFactory.Feature(convertToGeometryType(geoJsonFeature.Geometry.Type))
            {
                Id = geoJsonFeature.Id, 
                Tags = geoJsonFeature.Properties
            };
                        
            return tileFeature;
        }

        private GeometryType convertToGeometryType(GeoJSON.Net.GeoJSONObjectType geospatialType)
        {
            switch (geospatialType)
            {
                case GeoJSON.Net.GeoJSONObjectType.Point:
                    return GeometryType.Point;
                case GeoJSON.Net.GeoJSONObjectType.MultiPoint:
                    break;
                case GeoJSON.Net.GeoJSONObjectType.LineString:
                    break;
                case GeoJSON.Net.GeoJSONObjectType.MultiLineString:
                    break;
                case GeoJSON.Net.GeoJSONObjectType.Polygon:
                    return GeometryType.Polygon;
                case GeoJSON.Net.GeoJSONObjectType.MultiPolygon:
                    break;
                case GeoJSON.Net.GeoJSONObjectType.GeometryCollection:
                    break;
                case GeoJSON.Net.GeoJSONObjectType.Feature:
                    return GeometryType.Point;                    
                case GeoJSON.Net.GeoJSONObjectType.FeatureCollection:
                    break;                
            }
            throw new NotSupportedException($"Converting the GeoJSON geospatial type of {geospatialType} to a known geometry type is not supported.");
        }      
    }

    //
    public class ProjectGeoJSONToGeometric : APipe, IPipe
    {
        private GeoJSONIterativeContext dataContext;
        private readonly Func<IGeospatialItem, IProjectionProcessor> projectionProcessor;

        public ProjectGeoJSONToGeometric(Func<IGeospatialItem, IProjectionProcessor> projectionFactory)
        {
            this.projectionProcessor = projectionFactory;
        }

        public override async Task Process(IPipeContext context)
        {
            dataContext = context as GeoJSONIterativeContext;

            if (dataContext == null)
                throw new NotSupportedException($"The pipeline context must be of type IterativeContext");

            if (dataContext.Feature == null)
                throw new NotSupportedException("The Feature of the context must have a value for the data to be processed.");

            dataContext.Feature.Geometry = projectToGeometry(dataContext.OriginalFeature);
            
            while (this.HasNextPipe)
                await this.NextPipe.Process(context);
        }

        private (double X, double Y, double Z)[][] projectToGeometry(GeoJSON.Net.Feature.Feature geoJsonFeature)
        {
            switch (geoJsonFeature.Geometry.Type)
            {
                case GeoJSON.Net.GeoJSONObjectType.Point:
                    {
                        var geometry = geoJsonFeature.Geometry as GeoJSON.Net.Geometry.Point;
                        // Convert the GeoJSON data to a local DTO //
                        var point = new PointData(GeometryType.Point, geometry.Coordinates.Latitude, geometry.Coordinates.Longitude, geometry.Coordinates.Altitude ?? 0);

                        var processor = projectionProcessor(point);

                        return new(double X, double Y, double Z)[][]
                        {
                            new(double X, double Y, double Z)[]
                            {
                                (X:processor.ProjectedX, Y:processor.ProjectedY, Z:0d)
                            }
                        };
                    }
                case GeoJSON.Net.GeoJSONObjectType.Polygon:
                    {
                        var geometry = geoJsonFeature.Geometry as GeoJSON.Net.Geometry.Polygon;
                        (double X, double Y, double Z)[][] shapeData = null;

                        foreach (var polygonGroup in geometry.Coordinates)
                        {
                            var linestring = polygonGroup as GeoJSON.Net.Geometry.LineString;
                            shapeData = new(double X, double Y, double Z)[linestring.Coordinates.Count][];

                            for (int i = 0; i < linestring.Coordinates.Count; i++)
                            {
                                var coordinates = linestring.Coordinates[i];
                                var point = new PointData(GeometryType.Point, coordinates.Latitude, coordinates.Longitude, coordinates.Altitude ?? 0);
                                var projected = projectionProcessor(point);
                                shapeData[i] = new(double X, double Y, double Z)[]
                                {
                                    (X:projected.ProjectedX, Y:projected.ProjectedY, Z:0d)
                                };
                            }
                        }
                        return shapeData;
                    }
                default:
                    throw new NotSupportedException($"The Geometry type of {geoJsonFeature.Geometry.Type} is not supported.");
            }
        }
    }
    
    public class WrapTileFeatures : APipe, IPipe
    {
        public override async Task Process(IPipeContext context)
        {
            var dataContext = context as GeoJSONIterativeContext;

            if (dataContext == null)
                throw new NotSupportedException("The pipeline context must be of type DataPipelineContext");

            if (dataContext.Feature == null)
                throw new NotSupportedException("The Feature of the context must have a value for the data to be processed.");

            var wrappedFeatures = WrapFeatures(dataContext.Feature, dataContext.Buffer);

            while (this.HasNextPipe)
                await this.NextPipe.Process(context);
        }

        private IEnumerable<Feature> WrapFeatures(IGeometryItem unwrappedFeature, double buffer)
        {
            var left = Clip(unwrappedFeature, 1, (-1 - buffer), 1 + buffer, 0, -1, 2);
            //var right = Clip()

            return null;
        }

        private object Clip(IGeometryItem feature, int scale, double k1, double k2, double axis, double minAll, double maxAll)
        {


            return null;
        }
    }
}
