using System;
using System.Collections.Generic;
using System.Text;

namespace TileFactory.DataPipeline
{
    public abstract class ADataPipelineContext
    {
        public int MaxZoom { get; set; }
        public int MaxZoomIndex { get; set; }
        public bool SolidChildren { get; set; }
        public int Tolerance { get; set; }
        public int Extent { get; set; }
        public int Buffer { get; set; }
        public int LogLevel { get; set; }

        public IEnumerable<TileFactory.Feature> TileFeatures { get; set; }
    }
}
