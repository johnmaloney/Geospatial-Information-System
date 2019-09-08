using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdminManagementApp.Data;
using Messaging;
using Microsoft.AspNetCore.Mvc;
using Universal.Contracts.Messaging;

namespace AdminManagementApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly MessageRepository repository;

        public MessageController(MessageRepository repository)
        {
            this.repository = repository;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<IMessage>> Get()
        {
            return new ActionResult<IEnumerable<IMessage>>(repository.GetAll());
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<IMessage> Get(string id)
        {
            return new ActionResult<IMessage>(repository.Get(Guid.Parse(id)));
        }

        // POST api/values
        [HttpPost]
        public async Task Post([FromForm] object value)
        {
            await repository.Generate(new GeneralMessage { CorrellationId = new Random().Next(), Id = Guid.NewGuid() });
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
