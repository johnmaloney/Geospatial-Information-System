using Messaging.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Universal.Contracts.Messaging;

namespace TileProcessingApp.Models
{
    public class MessageRepository
    {
        public ConcurrentDictionary<string, string> SentMessages { get; private set; }

        public MessageRepository()
        {
            SentMessages = new ConcurrentDictionary<string, string>();
        }

        public void AddMessage(IMessage message)
        {
            if (message is TopicMessage topic)
            {
                SentMessages.TryAdd(DateTime.Now.Millisecond.ToString(), topic.Message);
            }
            else
                SentMessages.TryAdd(DateTime.Now.Millisecond.ToString(), $"A Message (Id:{message.Id}) of type {message.Type} was transferred.");
        }
    }
}
