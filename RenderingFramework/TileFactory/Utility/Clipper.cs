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

        public IEnumerable<IGeometryItem> Clip(IEnumerable<IGeometryItem> features, 
            double scale, double k1, double k2, Axis axis, double minAll, double maxAll)
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
                if (feature.Type == GeometryType.Point || feature.Type == GeometryType.MultiPoint)
                {
                    var clip = ClipPoints(feature, k1, k2, axis);
                    //Need to add this to the collection ..//
                }
                else // Covers all the other types //
                {
                    this.ClipGeometry(feature, k1, k2, axis);
                }
            }
            return clipped;
        }

        public List<(double X, double Y, double Z)> ClipPoints(IGeometryItem feature, double k1, double k2, Axis axis)
        {
            var slice = new List<(double X, double Y, double Z)>();

            for (int i = 0; i < feature.Geometry.Length; i++)
            {
                var a = feature.Geometry[i][0];
                var aK = axis == Axis.X ? a.X : a.Y;
                if (aK >= k1 && aK <= k2)
                {
                    slice.Add(a);
                }
            }
            return slice;
        }

        public IEnumerable<IGeometryItem> ClipGeometry(IGeometryItem geometryItem, double k1, double k2, Axis axis)
        {
            //var slices;

            Feature feature = geometryItem as Feature;
            for (int i = 0; i < feature.Geometry.Length; i++)
            {
                var ak = 0d;
                var bk = 0d;
                var points = feature.Geometry[i];
                var area = feature.Area;
                var distance = feature.Distance;
                var outer = feature.IsOuter;
                var length = points.Length;
                (double X, double Y, double Z) a = points[0];
                (double X, double Y, double Z) b = points[1];
                int j, last;


                var slice = new List<(double X, double Y, double Z)>();
                for (j = 0; j < length - 1; j++)
                {
                    a = points[j];
                    b = points[j + 1];

                    if (bk == 0)
                        ak = axis == Axis.X ? a.X : a.Y; 
                    bk = axis == Axis.X ? b.X : b.Y;

                    if (ak < k1)
                    {
                        if (bk > k2) // ---/---/---> //
                        {
                            
                        }
                    }
                }
            }
            return null;
        }

        internal (double X, double Y, double Z) intersectX((double X, double Y, double Z) a, (double X, double Y, double Z) b, double x)
        {

        }

        #endregion
    }
}
