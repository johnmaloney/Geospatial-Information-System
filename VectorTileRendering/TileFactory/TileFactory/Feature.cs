using System;
using System.Collections.Generic;
using System.Text;

namespace TileFactory
{
    public class Feature
    {
        #region Fields



        #endregion

        #region Properties

        public string Id { get; set; }

        public string Type { get; set; }

        public double[] Geom { get; set; }
        
        public Dictionary<string, object> Tags { get; set; }

        #endregion

        #region Methods



        #endregion

    }
}
