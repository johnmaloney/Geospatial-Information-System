using System;
using System.Collections.Generic;
using System.Text;
using TileFactory.Interfaces;

namespace TileFactory.Transforms
{
    public class TransformedTile : ITransformedTile
    {
        public (uint X, uint Y)[] TransformedFeatures
        {
            get;
            private set;
        }

        public TransformedTile((uint X, uint Y)[] transformedFeatures)
        {
            this.TransformedFeatures = transformedFeatures;
        }
    }
}
