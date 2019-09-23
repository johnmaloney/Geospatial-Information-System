using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdminManagementApp.Data;
using AdminManagementApp.Models;
using AdminManagementApp.Services;
using Messaging;
using Messaging.Models;
using Microsoft.AspNetCore.Mvc;
using Universal.Contracts.Messaging;

namespace AdminManagementApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly MessagingService service;
        private readonly FileService fileService;

        public MessageController(MessagingService service, FileService fileService)
        {
            this.service = service;
            this.fileService = fileService;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<IMessage>> Get()
        {
            return new ActionResult<IEnumerable<IMessage>>(service.CurrentMessages());
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<IMessage> Get(string id)
        {
            return new ActionResult<IMessage>(service.GetMessage(Guid.Parse(id)));
        }

        // POST api/values
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] JobRequest request)
        {
            // FIRST upload and verify the file is in the CLOUD //
            var fileCreated = await fileService.CreateDirectoryAndFile(request);

            if (fileCreated)
                await service.Generate(request);

            return CreatedAtAction("POST", new { message = "Document uploaded and Job Queueud" });
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromForm] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
