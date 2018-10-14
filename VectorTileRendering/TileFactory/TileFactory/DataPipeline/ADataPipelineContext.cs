using System;
using System.Collections.Generic;
using System.Text;

namespace TileFactory.DataPipeline
{
    public abstract class ADataPipelineContext
    {
        private double? zoomSq;

        public int MaxZoom { get; set; }
        public int MaxZoomIndex { get; set; }
        public bool SolidChildren { get; set; }
        public double Extent { get; set; }
        public double Buffer { get; set; }
        public int LogLevel { get; set; }

        public double Tolerance { get; set; }
        public double ExtentTolerance
        {
            get
            {
                if (!zoomSq.HasValue)
                    zoomSq = 1 << this.MaxZoom;
                double tolerance = this.Tolerance / (zoomSq.Value * this.Extent);
                return tolerance;
            }
        }

        public IEnumerable<TileFactory.Feature> TileFeatures { get; set; }
    }
}
