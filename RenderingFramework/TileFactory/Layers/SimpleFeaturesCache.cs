using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Universal.Contracts.Tiles;

namespace TileFactory.Layers
{
    internal class SimpleFeaturesCache : ICacheStorage<Guid, IEnumerable<IGeometryItem>>
    {
        #region Fields

        private ConcurrentDictionary<Guid, IEnumerable<IGeometryItem>> featuresCache = new ConcurrentDictionary<Guid, IEnumerable<IGeometryItem>>();

        #endregion

        #region Properties



        #endregion

        #region Methods

        public SimpleFeaturesCache()
        {

        }

        public IEnumerable<IGeometryItem> GetBy(Guid id)
        {
            if (featuresCache.TryGetValue(id, out IEnumerable<IGeometryItem> features))
                return features;

            return null;
        }

        public void StoreBy(Guid id, IEnumerable<IGeometryItem> features)
        {
            //var carbon = tile.CarbonCopy(); 
            featuresCache.AddOrUpdate(id, features, (currentId, currentFeatures) => currentFeatures = features);
        }

        public void Clear()
        {
            featuresCache.Clear();
        }

        #endregion
    }
}
