using System;
using System.Collections.Generic;
using System.Text;

namespace TileFactory.Utility
{
    public class Rewind
    {
        #region Fields



        #endregion

        #region Properties

        public bool IsReversed { get; private set; }

        public double Area { get; private set; }

        #endregion

        #region Methods

        public Rewind(Feature featureSet, bool isClockwise)
        {
            Area = signedArea(featureSet);
        }

        private double signedArea(Feature feature)
        {
            var sum = 0d;
            for (int i = 0, length = feature.Geometry.Length, j = length -1; i < length; j=i++)
            {
                var p1 = feature.Geometry[i][0];
                var p2 = feature.Geometry[j][0];

                sum += (p2.X - p1.X) * (p1.Y + p2.Y);
            }

            return sum;
        }

        #endregion
    }
}
