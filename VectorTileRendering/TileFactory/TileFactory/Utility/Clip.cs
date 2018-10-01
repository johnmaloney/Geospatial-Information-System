using System;
using System.Collections.Generic;
using System.Text;
using TileFactory.Interfaces;
using static TileFactory.Constants;

namespace TileFactory.Utility
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

        public Feature ClippedFeature { get; private set; }

        #endregion

        #region Methods

        public Clip(IGeometryItem feature, IClipContext clipContext)
        {
            //var min = feature.MinGeometry[clipContext.Axis];
            //var max = feature.MaxGeometry[clipContext.Axis];
        }

        #endregion 
    }
}
