﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using TileFactory;
using TileFactory.Layers;
using TileFactory.Serialization;
using Universal.Contracts.Layers;
using Universal.Contracts.Tiles;

namespace TileProcessingApp.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    public class TilesController : ControllerBase
    {
        private readonly TileRetrieverService tileRetrieverService;
        private readonly ITileContext tileContext;
        private readonly IFileProvider files;

        // GET api/values/5
        [HttpGet("{layerId}/{z}/{x}/{y}.{fileExtension}.{fileSerializer?}")]
        [EnableCors]
        public async Task<IActionResult> Get(string layerId, int z, int x, int y, string fileExtension, string fileSerializer, [FromQuery(Name = "access_token")]string accessToken)
        {
            tileContext.Identifier = layerId;
            var tile = await tileRetrieverService.GetTile(z, x, y);

            if (tile != null)
            {
                var factory = new ProtoBufSerializationFactory();
                factory.BuildFrom(tile, tileContext);

                var memStream = new MemoryStream();
                using (var serialStream = factory.SerializeTile())
                {
                    await serialStream.CopyToAsync(memStream);
                }
                memStream.Position = 0;
                return File(memStream, "application/octet-stream", "tile.pbf");
            }
            else
                return new EmptyResult();
        }

        public TilesController(LayerTileCacheAccessor cacheAccessor,
            ILayerInitializationService layerService,
            ITileContext tileContext)
        {
            this.tileContext = tileContext;

            var generator = new Generator(tileContext, cacheAccessor, layerService);
            tileRetrieverService = new TileRetrieverService(cacheAccessor, tileContext, generator);
        }
    }
}