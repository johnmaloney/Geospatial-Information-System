using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using TileFactory.Interfaces;

namespace TileFactory
{
    public class Retriever
    {
        #region Fields

        private ConcurrentDictionary<int, object> tiles = new ConcurrentDictionary<int, object>();

        #endregion

        #region Properties



        #endregion

        #region Methods



        public object GetTile(ITileContext tileContext, int zoomLevel, int x, int y)
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// GeoJSONVT.prototype.splitTile
        /// </remarks>
        /// <param name="features"></param>
        /// <param name="zoom"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        internal object SplitTile(IEnumerable<Feature> features, int zoom, int x, int y)
        {
            var zoomSqr = 1 << zoom;
            var id = toIdentifier(zoom, x, y);
            var currentTile = tiles[id];
            var tileTolerance = zoom == 
        }

        internal void createTile(IEnumerable<Feature> features, int zoomSquared, int x, int y, int tileTolerance, )
        {

        }


        private int toIdentifier(int zoom, int x, int y)
        {
            return ((1 << zoom) * y + x) * 32 + zoom;
        }
        #endregion
    }
}
