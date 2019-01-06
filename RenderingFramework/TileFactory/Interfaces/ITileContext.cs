using System;
using System.Collections.Generic;
using System.Text;
using TileFactory.DataPipeline;

namespace TileFactory.Interfaces
{
    public interface ITileContext : IContext
    {
        IEnumerable<TileFactory.Feature> TileFeatures { get; set; }
    }
}
