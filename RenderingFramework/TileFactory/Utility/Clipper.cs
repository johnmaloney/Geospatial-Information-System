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

        public IGeometryItem[] Clip(IGeometryItem[] features, 
            double scale, double k1, double k2, Axis axis, double minAll, double maxAll)
        {
            k1 /= scale;
            k2 /= scale;

            // clip would be trivial accept the feature //
            if (minAll >= k1 && maxAll <= k2)
                return features;
            else if (minAll > k2 || maxAll < k1)
                return new IGeometryItem[0];

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
                    var clippedGeometry = this.ClipGeometry(feature, k1, k2, axis);
                    clipped.Add(CreateGeometryFeatureFrom(feature, clippedGeometry));
                }
            }
            return clipped.ToArray();
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

        public (double X, double Y, double Z)[][] ClipGeometry(IGeometryItem geometryItem, double k1, double k2, Axis axis, bool closed = true)
        {
            // Stores the slices from this operation //
            var slices = new List<(double X, double Y, double Z)[]> ();

            Feature rootFeature = geometryItem as Feature;
            for (int i = 0; i < rootFeature.Geometry.Length; i++)
            {
                double? ak = null;
                double? bk = null;
                var points = rootFeature.Geometry[i];
                var area = rootFeature.Area;
                var distance = rootFeature.Distance;
                var outer = rootFeature.IsOuter;
                var length = points.Length;
                (double X, double Y, double Z) a = points[0];
                (double X, double Y, double Z) b = points[1];
                (double X, double Y, double Z) last = points[points.Length -1];

                var slice = new List<(double X, double Y, double Z)>();
                for (int j = 0; j < length - 1; j++)
                {
                    // a equals b if b has been assigned in the prior iteration //
                    a = j > 0 ? b : points[j];
                    b = points[j + 1];

                    if (bk.HasValue)
                        ak = bk;
                    else
                        ak = axis == Axis.X ? a.X : a.Y;

                    bk = axis == Axis.X ? b.X : b.Y;

                    if (ak < k1)
                    {
                        if (bk > k2) // ---/---/---> 
                        {
                            slice.Add(axis == Axis.X 
                                ? IntersectX(a, b, k2) 
                                : IntersectY(a, b, k2));

                            if (!closed)
                                slice = NewSlice(slices, slice, area[i], distance[i], outer);
                        }
                        else if(bk >= k1)
                        {
                            slice.Add(axis == Axis.X
                                ? IntersectX(a, b, k1)
                                : IntersectY(a, b, k1));
                        }

                    }
                    else if (ak > k2)
                    {
                        if (bk < k1) // <---/---/--- | 
                        {
                            // Adding two //
                            slice.Add(axis == Axis.X
                                ? IntersectX(a, b, k2)
                                : IntersectY(a, b, k2));

                            slice.Add(axis == Axis.X
                                ? IntersectX(a, b, k1)
                                : IntersectY(a, b, k1));

                            if (!closed)
                                slice = NewSlice(slices, slice, area[i], distance[i], outer);
                        }
                        else if (bk <= k2) // | <---/--- 
                        {
                            slice.Add(axis == Axis.X
                                ? IntersectX(a, b, k2)
                                : IntersectY(a, b, k2));
                        }
                    }
                    else
                    {
                        slice.Add(a);

                        if (bk < k1)
                        {
                            slice.Add(axis == Axis.X
                                ? IntersectX(a, b, k1)
                                : IntersectY(a, b, k1));

                            if (!closed)
                                slice = NewSlice(slices, slice, area[i], distance[i], outer);
                        }
                        else if (bk > k2)
                        {
                            slice.Add(axis == Axis.X
                                ? IntersectX(a, b, k2)
                                : IntersectY(a, b, k2));

                            if (!closed)
                                slice = NewSlice(slices, slice, area[i], distance[i], outer);
                        }
                    }
                }

                // Add the last point //
                a = points[length - 1];
                ak = axis == Axis.X ? a.X : a.Y;

                if (ak >= k1 && ak <= k2)
                {
                    // add //
                    slice.Add(a);
                }

                // Close the polygon if the endpoints are not the same after clipping //
                if (slice.Count > 0)
                {
                    last = slice[slice.Count - 1];
                    var firstSlice = slice[0];

                    if (closed && (firstSlice.X != last.X || firstSlice.Y != last.Y))
                        slice.Add(firstSlice);
                }

                NewSlice(slices, slice, area[i], distance[i], outer);
            }
            return slices.ToArray();
        }

        internal (double X, double Y, double Z) IntersectX((double X, double Y, double Z) a, (double X, double Y, double Z) b, double x)
        {
            var y = (x - a.X) * (b.Y - a.Y) / (b.X - a.X) + a.Y;
            return (X: x, Y: y, Z: 1);
        }

        internal (double X, double Y, double Z) IntersectY((double X, double Y, double Z) a, (double X, double Y, double Z) b, double y)
        {
            var x = (y - a.X) * (b.X - a.X) / (b.Y - a.Y) + a.X;
            return (X: x, Y: y, Z: 1);
        }

        internal List<(double X, double Y, double Z)> NewSlice(
            List<(double X, double Y, double Z)[]> slices, 
            List<(double X, double Y, double Z)> slice, 
            double area, double distance, bool outer) 
        {
            if (slice.Count > 0)
            {
                slices.Add(slice.ToArray());
            }

            // reset the collection //
            return new List<(double X, double Y, double Z)>();
        }

        internal IGeometryItem CreateGeometryFeatureFrom(IGeometryItem source, (double X, double Y, double Z)[][] newGeometry)
        {
            if (source is Feature)
            {
                var sourceFeature = source as Feature;
                return new Feature(source.Type)
                {
                    Id = sourceFeature.Id,
                    Tags = sourceFeature.Tags,
                    Geometry = newGeometry
                };
            }

            throw new NotSupportedException($"Could not translate the GeometryItem type of: {source.GetType()} into a new instance.");
        }

        #endregion
    }
}
