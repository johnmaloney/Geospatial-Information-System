using Messaging;
using Messaging.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Universal.Contracts.Messaging;

namespace AdminManagementApp.Data
{
    public class MessageRepository
    {
        private readonly ApplicationDbContext database;

        public MessageRepository(ApplicationDbContext context)
        {
            this.database = context;
        }

        public IEnumerable<IMessage> GetAll()
        {
            var messages = new List<IMessage>();
            messages.AddRange(database.GeneralMessages);
            messages.AddRange(database.TopicMessages);
            messages.AddRange(database.GeneralCommands);
            return messages;
        }

        public IMessage Get(Guid id)
        {
            IMessage message = this.database.TopicMessages.Find(id);
            if (message == null)
                message = this.database.GeneralMessages.Find(id);
            if (message == null)
                message= this.database.GeneralCommands.Find(id);
            return message ?? null;
        }

        public void Add(IMessage message)
        {
            if (message is GeneralCommand gCommand)
            {
                this.database.GeneralCommands.Add(gCommand);
                this.database.SaveChanges();
            }
            else if (message is GeneralMessage gMessage)
            {
                this.database.GeneralMessages.Add(gMessage);
                this.database.SaveChanges();
            }
            else if (message is TopicMessage gTopic)
            {
                this.database.TopicMessages.Add(gTopic);
                this.database.SaveChanges();
            }
            else
                throw new NotSupportedException($"The message type of : {message.GetType()} does not have a database store.");
        }
    }
}
