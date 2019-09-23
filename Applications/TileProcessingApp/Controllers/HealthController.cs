using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TileProcessingApp.Models;

namespace TileProcessingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        private readonly MessageRepository messageRepository;

        [HttpGet]
        public Task<ObjectResult> Get()
        {
            var result = new ObjectResult(messageRepository.SentMessages);
            return Task.FromResult(result);
        }

        public HealthController(MessageRepository messageRepository)
        {
            this.messageRepository = messageRepository;
        }
    }
}