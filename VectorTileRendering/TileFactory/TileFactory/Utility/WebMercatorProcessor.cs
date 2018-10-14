using System;
using System.Collections.Generic;
using System.Text;
using TileFactory.Interfaces;

namespace TileFactory.Utility
{
    public class WebMercatorProcessor : IProjectionProcessor
    {
        #region Fields
        
        #endregion

        #region Properties

        public double ProjectedX { get; private set; }

        public double ProjectedY { get; private set; }

        #endregion

        #region Methods

        public WebMercatorProcessor(IGeospatialItem geospatialItem, double? tolerance = null)
        {
            /// X is the Horizontal value or Longitude
            /// Y is the Vertical value or Latitude
            this.ProjectedX = projectX(geospatialItem.Longitude);
            this.ProjectedY = projectY(geospatialItem.Latitude);
        }

        public WebMercatorProcessor(double latitude, double longitude)
        {
            ProjectedX = projectX(longitude);
            ProjectedY = projectY(latitude);
        }

        private double projectX(double longitude)
        {
            return longitude / 360 + 0.5;
        }

        private double projectY(double latitude)
        {
            var sin = Math.Sin(latitude * Math.PI / 180);
            var y2 = 0.5 - 0.25 * Math.Log((1 + sin) / (1 - sin)) / Math.PI;
            return y2 < 0 ? 0
                : y2 > 1 ? 1
                : y2;
        }
            #endregion
        }
}
