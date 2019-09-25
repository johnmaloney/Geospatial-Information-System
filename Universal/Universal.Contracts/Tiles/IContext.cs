using System;
using System.Collections.Generic;
using System.Text;

namespace Universal.Contracts.Tiles
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
}
