using System;
using System.Collections.Generic;
using System.Text;

namespace Universal.Contracts.Tiles
{
    public interface ITransformedTile
    {
        IList<ITransformedFeature> TransformedFeatures { get; }
    }
}
