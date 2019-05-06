using System;
using System.Collections.Generic;
using System.Text;

namespace TileFactory.Interfaces
{
    public interface IVectorLayer
    {
        uint Version { get; set; }
        string Name { get; set; }
        IEnumerable<IVectorFeature> Features { get; set; }
    }

    public interface IVectorFeature
    {
        uint Id { get; set; }
        uint[] Tags { get; set; }
        uint Type { get; set; }
        uint[] Geometry { get; set; }
    }    
}
