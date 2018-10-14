using System;
using System.Collections.Generic;
using System.Text;
using TileFactory.Interfaces;

namespace TileFactory.DataPipeline
{
    internal class GeoJSONIterativeContext : IPipeContext
    {
        public GeoJSON.Net.Feature.Feature OriginalFeature { get; private set; }

        public IGeometryItem Feature { get; private set; }

        public double Buffer { get; private set; }

        public GeoJSONIterativeContext(GeoJSON.Net.Feature.Feature originalFeature, IGeometryItem feature, double buffer)
        {
            this.OriginalFeature = originalFeature;
            this.Feature = feature;
            this.Buffer = buffer;
        }
    }
}
