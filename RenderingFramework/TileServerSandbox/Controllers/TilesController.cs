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

namespace TileServerSandbox.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    public class TilesController : ControllerBase
    {
        private readonly Generator generator;
        private readonly IFileProvider files;

        // GET api/values/5
        [HttpGet("{z}/{x}/{y}.{fileExtension}.{fileSerializer?}")]
        [EnableCors]
        public async Task<IActionResult> Get(int z, int x, int y, string fileExtension, string fileSerializer, [FromQuery(Name="access_token")]string accessToken)
        {
            var tile = generator.GenerateTile(z, x, y);

            var path = files.GetFileInfo("sample.vector.pbf");
            
            var memStream = new MemoryStream();
            using (var stream = new FileStream(path.PhysicalPath, FileMode.Open))
            {
                await stream.CopyToAsync(memStream);
            }
            memStream.Position = 0;
            return File(memStream, "application/octet-stream", Path.GetFileName(path.PhysicalPath));
        }

        public TilesController(ITileCacheStorage<ITile> tileCache, ITileContext tileContext, IFileProvider files)
        {
            generator = new Generator(tileCache, tileContext);
            this.files = files;
        }
    }
}
