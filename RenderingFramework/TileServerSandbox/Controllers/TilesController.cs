using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace TileServerSandbox.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    public class TilesController : ControllerBase
    {     
        // GET api/values/5
        [HttpGet("{z}/{x}/{y}.{fileExtension}.{fileSerializer?}")]
        [EnableCors]
        public async Task<ActionResult<string>> Get(int z, int x, int y, string fileExtension, string fileSerializer, [FromQuery(Name="access_token")]string accessToken)
        {
            return "Found Server";
        }
    }
}
