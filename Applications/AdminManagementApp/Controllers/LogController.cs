using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Universal.Contracts.Logging;

namespace AdminManagementApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private readonly ILogger logger;

        public LogController(ILogger logging)
        {
            this.logger = logging;
        }

        // POST api/values
        [HttpPost]
        public async Task Post([FromForm] object value)
        {
            await logger.Log(new LogEntry { Id = 1, Title = "Log Controller post.", Type=LogType.Default.ToString()});
        }
    }
}