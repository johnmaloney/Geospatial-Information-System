using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TileFactory;
using Universal.Contracts.Layers;
using Universal.Contracts.Models;

namespace TileProcessingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LayerController : ControllerBase
    {
        private readonly ILayerInitializationService layerService;

        [HttpGet]
        [EnableCors]
        public ActionResult<IEnumerable<LayerInformationModel>> Get()
        {

            return new ObjectResult(layerService.Models);
        }

        public LayerController(ILayerInitializationService layerService)
        {
            this.layerService = layerService;
        }
    }
}