using GeoJSON.Net.Feature;
using System;
using System.Collections.Generic;
using System.Text;
using TileFactory.Interfaces;

namespace TileFactory.DataPipeline.GeoJson
{
    public class GeoJsonContext : ADataPipelineContext, IPipeContext
    {
        public string OriginalData { get; }

        public FeatureCollection Features { get; set; }

        public GeoJsonContext(string rawData)
        {
            this.OriginalData = rawData;
        }
    }
}
