﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TileFactory.Interfaces;
using TileFactory.Models;
using TileFactory.Transforms;
using TileFactory.Utility;

namespace TileFactory
{
    public class TileRetriever
    {
        #region Fields

        private readonly ITileCacheStorage<ITransformedTile> transformedCache;
        private readonly Generator tileGenerator;
        private readonly ITileContext tileContext;
        private readonly Transform tileTransform;

        #endregion

        #region Properties
        #endregion

        #region Methods

        public TileRetriever(ITileCacheStorage<ITransformedTile> transformedCache, 
            ITileContext context, Generator tileGenerator)
        {
            this.transformedCache = transformedCache;
            this.tileGenerator = tileGenerator;
            this.tileContext = context;
            this.tileTransform = new Transform(context.Extent, context.Buffer);

            if (tileContext == null)
                throw new NotSupportedException("The TileContext must have a value.");            
        }

        public ITransformedTile GetTile(int zoomLevel=0, double x=0, double y=0)
        {
            var zoomSqr = 1 << zoomLevel;
            var xDenom = ((x % zoomSqr) + zoomSqr) % zoomSqr;

            var id = Identifier.ToId(zoomLevel, (int)xDenom, (int)y);
            var transformedTile = transformedCache.GetBy(id);
            if (transformedTile != null)
                return transformedTile;
                        
            var geoTile = tileGenerator.GenerateTile(zoomLevel, xDenom, y);

            if (geoTile == null)
                throw new NotSupportedException($"The tile with id:{id} was not in the base geo tiles collection.");

            transformedTile = tileTransform.ProcessTile(geoTile);
            transformedCache.StoreBy(id, transformedTile);
            return transformedTile;
        }

        #endregion
    }
}
