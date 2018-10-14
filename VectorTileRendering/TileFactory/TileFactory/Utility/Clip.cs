using System;
using System.Collections.Generic;
using System.Text;
using TileFactory.Interfaces;
using static TileFactory.Constants;

namespace TileFactory.Utility.Obsolete
{
    internal interface IClipContext
    {
        double Scale { get; }
        double K1 { get; }
        double K2 { get; }
        int Axis { get; }
        Int16 MinAll { get; }
        Int16 MaxAll { get; }
        ClipType Type { get; }
    }

    internal class ClipContext : IClipContext
    {
        #region Fields
        #endregion

        #region Properties
        public double Scale { get; private set; }

        public double K1 { get; private set; }

        public double K2 { get; private set; }

        public int Axis { get; private set; }

        public short MinAll { get; private set; }

        public short MaxAll { get; private set; }

        public ClipType Type { get; private set; }

        public bool ShouldClipFeatures { get; private set; }

        #endregion

        #region Methods

        public ClipContext(ClipType clipType, double buffer)
        {
            Type = clipType;
            if (clipType == ClipType.Left)
            {
                Scale = 1;
                K1 = -1 - buffer;
                K2 = buffer;
                Axis = 0;
                MinAll = -1;
                MaxAll = 2;
            }
            else if (clipType == ClipType.Right)
            {
                Scale = 1;
                K1 = 1 - buffer;
                K2 = 2 + buffer;
                Axis = 0;
                MinAll = -1;
                MaxAll = 2;
            }
            else
                throw new NotSupportedException($"The clip type of {clipType} is not supported.");

            K1 /= Scale;
            K2 /= Scale;

            // Trivial changes needed accept current Features as-is //
            if (MinAll >= K1 && MaxAll <= K2)
                Type = ClipType.None;
            // Trivial changes reject  all //
            else if (MinAll > K2 || MaxAll < K1)
                Type = ClipType.Rejected;
        }

        #endregion
    }
    
    internal class Clip
    {
        #region Fields



        #endregion

        #region Properties

        public ClipType Type { get; private set; }

        public IGeometryItem ClippedFeature { get; private set; }

        #endregion

        #region Methods

        public Clip(IGeometryItem feature, IClipContext clipContext)
        {
            var min = feature.MinGeometry.X;
            var max = feature.MaxGeometry.X;

            // clip would be trivial accept the feature //
            if (min >= clipContext.K1 && max <= clipContext.K2)
            {
                ClippedFeature = feature;
                return;
            }
            else if (min > clipContext.K2 || max < clipContext.K1)
                return;

            // This is a rare occurrance //
            if (feature.Type == GeometryType.Point)
            {
                //ClippedFeature = ClipPoints(feature, clipContext.K1, clipContext.K2, clipContext.Axis);
            }
            else // Covers all the other types //
            {

            }
        }

        public void ClipPoints(IGeometryItem feature, double k1, double k2)
        {
            var slice = new List<(double X, double Y, double Z)>();

            for (int i = 0; i < feature.Geometry.Length; i++)
            {
                var a = feature.Geometry[i][0];
                
                if (a.X >= k1 && a.X <= k2)
                {
                }
            }
        }

        #endregion 
    }
}
