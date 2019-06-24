using System;
using System.Collections.Generic;
using System.Text;
using TileFactory.Interfaces;

namespace TileFactory.Transforms
{
    public class TransformedTile : ITransformedTile
    {
        public IList<ITransformedFeature> TransformedFeatures
        {
            get;
            private set;
        }

        public TransformedTile(IList<ITransformedFeature> transformedFeatures)
        {
            this.TransformedFeatures = transformedFeatures;
        }
    }

    public class TransformedFeature : ITransformedFeature
    {
        public (int X, int Y)[] Coordinates { get; set; }

        public int GeometryType { get; private set; }

        public TransformedFeature(int geometryType, (int X, int Y)[] features)
        {
            Coordinates = features;
            GeometryType = geometryType;
        }
    }
}
