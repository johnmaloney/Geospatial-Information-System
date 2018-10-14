using System;
using System.Collections.Generic;
using System.Text;
using TileFactory.Interfaces;

namespace TileFactory.Utility
{
    public class Clipper
    {
        #region Fields



        #endregion

        #region Properties



        #endregion

        #region Methods

        public Clipper()
        {

        }

        public IEnumerable<IGeometryItem> Clip(IEnumerable<IGeometryItem> features, 
            double scale, double k1, double k2, double minAll, double maxAll, Axis axis)
        {
            k1 /= scale;
            k2 /= scale;

            // clip would be trivial accept the feature //
            if (minAll >= k1 && maxAll <= k2)
                return features;
            else if (minAll > k2 || maxAll < k1)
                return new List<IGeometryItem>();

            var clipped = new List<IGeometryItem>();
            foreach(var feature in features)
            {
                // Get the axis, whether we are working with X or Y //
                var min = axis == Axis.X 
                    ? feature.MinGeometry.X 
                    : feature.MinGeometry.Y;
                var max = axis == Axis.X 
                    ? feature.MaxGeometry.X 
                    : feature.MaxGeometry.Y;

                // Accept this as clipped //
                if (min >= k1 && max <= k2)
                {
                    clipped.Add(feature);
                    continue;
                }
                // Reject this as not clipped //
                else if (min > k2 || max < k1)
                    continue;
                
                // This is a rare occurrance //
                if (feature.Type == GeometryType.Point)
                {
                    var clip = ClipPoints(feature, k1, k2);
                }
                else // Covers all the other types //
                {

                }
            }
            return clipped;
        }

        public IGeometryItem ClipPoints(IGeometryItem feature, double k1, double k2)
        {
            var slice = new List<(double X, double Y, double Z)>();

            for (int i = 0; i < feature.Geometry.Length; i++)
            {
                var a = feature.Geometry[i][0];

                if (a.X >= k1 && a.X <= k2)
                {
                }
            }
            throw new NotImplementedException("Multi points are not yet supported");
        }

        public IEnumerable<IGeometryItem> ClipGeometry()
        {
            return null;
        }

        #endregion
    }
}
