using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;

namespace TileServerSandbox.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LayerController : ControllerBase
    {
        private readonly IFileProvider files;
        
        [HttpGet("{group}/{name?}")]
        public async Task<IActionResult> Get(string group, string name)
        {
            return null;
        }

        public LayerController(IFileProvider files)
        {
            this.files = files;
        }
    }
}