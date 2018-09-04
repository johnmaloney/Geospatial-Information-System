using System;
using System.Collections.Generic;
using System.Text;

namespace TileFactory
{
    /// <summary>
    /// Represents an individual command of action when looking for a Schema item.
    /// Command Spec found here - https://github.com/mapbox/vector-tile-spec/tree/master/2.1
    /// Command      Id  Parameters  Parameter Count 
    /// -----------------------------------------//
    /// MoveTo       1   dX, dY      2
    /// LineTo       2   dX, dY      2   
    /// ClosePath    7   None        0
    /// </summary>
    public enum CommandType : int
    {
        MoveTo = 1,
        LineTo = 2,
        ClosePath = 7
    }
}
