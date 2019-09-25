using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using System;
using System.Collections.Generic;
using System.Text;
using TileFactory.Interfaces;
using Universal.Contracts.Tiles;

namespace TileFactory.DataPipeline.GeoJson
{
    public class GeoJsonContext : AContext, IPipeContext, ITileContext
    {
        public string OriginalData { get; }

        public FeatureCollection Features { get; set; }

        public IGeometryObject Geometry { get; set; }

        public GeoJsonContext(string rawData)
        {
            this.OriginalData = rawData;
        }
    }
}
