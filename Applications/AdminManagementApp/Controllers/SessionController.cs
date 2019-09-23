using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdminManagementApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Universal.Contracts.Files;

namespace AdminManagementApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionController : ControllerBase
    {
        private readonly IFileRepository files;

        public SessionController(IFileRepository files)
        {
            this.files = files;
        }

        [HttpGet]
        public async Task<ActionResult<Session>> Get()
        {

            var currentDirectories = await files.GetDirectory();
            var session = new Session(currentDirectories);
            
            return new ActionResult<Session>(session);
        }
    }
}