using System;
using System.Collections.Generic;
using System.Text;
using Universal.Contracts.Tiles;

namespace TileFactory.DataPipeline
{  
    public abstract class AContext : IContext
    {
        private double? zoomSq;

        public int MaxZoom { get; set; }
        public int MaxZoomIndex { get; set; }
        public int MaxAllowablePoints { get; set; }
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

        public string Identifier { get; set; }

        public IEnumerable<IGeometryItem> TileFeatures { get; set; }
    }
}
