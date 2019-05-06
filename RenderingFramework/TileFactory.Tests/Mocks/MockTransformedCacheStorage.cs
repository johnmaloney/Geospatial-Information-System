using System;
using System.Collections.Generic;
using System.Text;
using TileFactory.Interfaces;
using TileFactory.Serialization;
using TileFactory.Transforms;

namespace TileFactory.Tests.Mocks
{
    public class MockTransformedCacheStorage : ITileCacheStorage<Tile>
    {
        #region Fields

        private readonly ProtobufTransform transform;

        #endregion

        #region Properties



        #endregion

        #region Methods

        public MockTransformedCacheStorage()
        {
            
        }

        public Tile GetBy(int id)
        {
            throw new NotImplementedException();
        }

        public void StoreBy(int id, Tile tile)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
