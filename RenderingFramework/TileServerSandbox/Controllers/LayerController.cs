using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using TileFactory;

namespace TileServerSandbox.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LayerController : ControllerBase
    {
        private readonly ILayerInitializationService layerService;
        
        [HttpGet("{group}/{name?}")]
        public IActionResult Get(string group, string name)
        {
            return new ObjectResult(layerService.Models);
        }

        public LayerController(ILayerInitializationService layers)
        {
            layerService = layers;
        }
    }
}