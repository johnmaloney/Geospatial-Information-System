using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using TileFactory;
using TileFactory.Interfaces;
using TileFactory.Serialization;

namespace TileServerSandbox.Controllers
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
            //var path = files.GetFileInfo($"{layerId}.{fileExtension}.{fileSerializer}");
            
            //var memStream = new MemoryStream();
            //using (var stream = new FileStream(path.PhysicalPath, FileMode.Open))
            //{
            //    await stream.CopyToAsync(memStream);
            //}
            //memStream.Position = 0;
            //return File(memStream, "application/octet-stream", "tile.pbf");
        }

        public TilesController(ITileCacheStorage<ITile> tileCache, 
            ITileCacheStorage<ITransformedTile> transformedCache, 
            ITileContext tileContext, 
            ILayerInitializationService layerService)
        {
            this.tileContext = tileContext;
            this.files = files;

            var generator = new Generator(tileContext, tileCache, layerService);
            tileRetrieverService = new TileRetrieverService(transformedCache, tileContext, generator);
        }
    }
}
