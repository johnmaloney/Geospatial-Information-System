using System;
using System.Collections.Generic;
using System.Text;
using TileFactory.Interfaces;

namespace TileFactory.Tests.Mocks
{
    public class TileContextMock : ITileContext
    {
        public IEnumerable<Feature> TileFeatures
        {
            get;
            set;
        }
    }
}
