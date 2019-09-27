using Messaging.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Universal.Contracts.Messaging;

namespace AdminManagementApp.Hubs
{
    public class MessageHub : Hub
    {
        private readonly ITopicObserverClient topicObserver;

        public MessageHub(ITopicObserverClient topicObserver)
        {
            this.topicObserver = topicObserver;
            this.topicObserver.RegisterForNotificationOf<TopicMessage>(MessageReceiver);
        }

        public void Ping(string message)
        {
            Clients.All.SendAsync("broadcastMessage", message);
        }

        public async Task MessageReceiver(IMessage message)
        {
            if (message is TopicMessage gMessage)
            {
                await Clients.All.SendAsync("broadcastMessage", gMessage.Message);
            }
        }
    }
}
