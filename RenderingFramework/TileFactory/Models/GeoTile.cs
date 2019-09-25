using System;
using System.Collections.Generic;
using System.Text;
using TileFactory.Interfaces;
using Universal.Contracts.Tiles;

namespace TileFactory.Models
{
    /// <summary>
    ///
    /// </summary>
    [Serializable]
    public class GeoTile : ITile
    {
        #region Fields



        #endregion

        #region Properties

        public double ZoomSquared { get; set; }

        public IList<IGeometryItem> Features { get; set; }

        public IList<IGeometryItem> Source { get; set; }

        public int NumberOfPoints { get; set; }

        public int NumberOfSimplifiedPoints { get; set; }

        public int NumberOfFeatures { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public bool Transformed { get; set; }

        public (double X, double Y) Min { get; set; }

        public (double X, double Y) Max { get; set; }

        #endregion

        #region Methods

        public override string ToString()
        {
            try
            {
                return $"Zsqr:{ZoomSquared} | X:{X} Y:{Y} | Has Source:{Source != null && Source.Count > 0}";
            }
            catch
            {
                return base.ToString();
            }
        }
        #endregion
    }
}
