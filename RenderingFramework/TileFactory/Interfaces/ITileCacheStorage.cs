using System;
using System.Collections.Generic;
using System.Text;

namespace TileFactory.Interfaces
{
    public interface ITileCacheStorage
    {
        ITile GetBy(int id);
        void StoreBy(int id, ITile tile);
    }
}
