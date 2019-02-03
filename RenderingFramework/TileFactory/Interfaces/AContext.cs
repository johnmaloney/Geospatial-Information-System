using System;
using System.Collections.Generic;
using System.Text;

namespace TileFactory.DataPipeline
{
    public interface IContext
    {
        /// <summary>
        /// Max number of points per tile in the tile index.
        /// </summary>
        int MaxAllowablePoints { get; }

        /// <summary>
        /// Max zoom to preserve detail on
        /// </summary>
        int MaxZoom { get; }

        /// <summary>
        /// Max zoom in the tile index.
        /// </summary>
        int MaxZoomIndex { get; }

        /// <summary>
        /// Whether to tile solid square tiles further.
        /// </summary>
        bool SolidChildren { get; }

        /// <summary>
        /// Tile extent
        /// </summary>
        double Extent { get; }

        /// <summary>
        /// Tile buffer in each side.
        /// </summary>
        double Buffer { get; }

        /// <summary>
        /// Verbosity of the output log.
        /// </summary>
        int LogLevel { get; }

        /// <summary>
        /// Simplification tolerance (Higher means Simpler).
        /// </summary>
        double Tolerance { get; }

        double ExtentTolerance { get; }
    }

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

        public IEnumerable<TileFactory.Feature> TileFeatures { get; set; }
    }
}
