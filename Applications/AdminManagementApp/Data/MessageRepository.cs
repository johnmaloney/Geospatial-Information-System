using Messaging;
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
        private readonly ITopicMessengerClient messenger;
        private readonly ITopicObserverClient topicObserver;

        public MessageRepository(ApplicationDbContext context, 
            ITopicMessengerClient messenger, 
            ITopicObserverClient topicObserver)
        {
            this.database = context;
            this.messenger = messenger;
            this.topicObserver = topicObserver;
            this.topicObserver.RegisterForNotificationOf<GeneralMessage>(MessageReceiver);
        }

        public IEnumerable<IMessage> GetAll()
        {
            return database.Messages;
        }

        public IMessage Get(Guid id)
        {
            var message = this.database.Messages.Find(id);
            return message ?? null;
        }

        public async Task<bool> Generate(IMessage message)
        {
            await messenger.Send(message);
            return true;
        }

        public Task MessageReceiver(IMessage message)
        {
            if (message is GeneralMessage gMessage)
            {
                var existingMessage = this.database.Messages.Find(gMessage.Id);
                if (existingMessage == null)
                {
                    this.database.Messages.Add(gMessage);
                    this.database.SaveChanges();
                }
            }


            return Task.FromResult(true);
        }
    }
}
