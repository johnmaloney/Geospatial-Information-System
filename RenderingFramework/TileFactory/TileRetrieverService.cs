using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TileFactory.Interfaces;
using TileFactory.Layers;
using TileFactory.Models;
using TileFactory.Transforms;
using TileFactory.Utility;

namespace TileFactory
{
    /// <summary>
    /// Service that brings together the many disparate items needed
    /// to initialize and retrieve a tile. This is a service and therefore 
    /// should act as a broker only to the workers that know the initialization 
    /// and retrieval processes.
    /// </summary>
    public class TileRetrieverService
    {
        #region Fields

        private readonly Generator tileGenerator;
        private readonly LayerTileCacheAccessor cacheAccessor;
        private readonly ITileContext tileContext;
        private readonly Transform tileTransform;

        #endregion

        #region Properties
        #endregion

        #region Methods

        public TileRetrieverService(
            LayerTileCacheAccessor cacheAccessor,
            ITileContext context, Generator tileGenerator)
        {
            this.tileGenerator = tileGenerator;
            this.cacheAccessor = cacheAccessor;
            this.tileContext = context;
            this.tileTransform = new Transform(context.Extent, context.Buffer);

            if (tileContext == null)
                throw new NotSupportedException("The TileContext must have a value.");            
        }

        public async Task<ITransformedTile> GetTile(int zoomLevel=0, double x=0, double y=0)
        {
            var zoomSqr = 1 << zoomLevel;
            var xDenom = ((x % zoomSqr) + zoomSqr) % zoomSqr;

            var transformedTile = cacheAccessor.GetTransformedTile(
                tileContext.Identifier, zoomLevel, (int)xDenom, (int)y);
            if (transformedTile != null)
                return transformedTile;
            
            // This scenario means that this could be an initialization request //
            // Need to handle this by initializing the tiles into memory //

            var geoTile = await tileGenerator.GenerateTile(zoomLevel, xDenom, y);

            if (geoTile != null)
            {
                transformedTile = tileTransform.ProcessTile(geoTile);
                cacheAccessor.StoreTransformedTile(
                    tileContext.Identifier, zoomLevel, (int)xDenom, (int)y, transformedTile);
                return transformedTile;
            }
            return null;
        }


        /// <summary>
        /// Initializes a set of features into the cache system.
        /// To be used when features are already loaded into the context. 
        /// This might occur when projecting data from a file on the server.
        /// </summary>
        /// <param name="tileContext"></param>
        /// <returns></returns>
        public async Task InitializeTile(ITileContext tileContext)
        {
            var generator = new Generator(tileContext, cacheAccessor, null);

            // This is only called at the beginning //
            generator.SplitTile(tileContext.TileFeatures.ToArray(),
                zoom: 0, x: 0, y: 0, currentZoom: null, currentX: null, currentY: null);

            var geoTile = await generator.GenerateTile();

            if (geoTile == null)
                throw new NotSupportedException($"The tile with id: {tileContext.Identifier} could not be initialized.");
        }

        #endregion
    }
}
