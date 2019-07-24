using System;
using System.Collections.Generic;
using System.Text;
using TileFactory.DataPipeline;

namespace TileFactory.Interfaces
{
    public interface ITileContext : IContext
    {
        string Identifier { get; set; }
        IEnumerable<TileFactory.Feature> TileFeatures { get; set; }
    }
}
