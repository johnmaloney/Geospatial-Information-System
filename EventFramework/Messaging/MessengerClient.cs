
using Messaging.Models;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Universal.Contracts.Messaging;

namespace Messaging
{
    /// <summary>
    /// Provides implementation for the Queue and Topic sender modes.
    /// Queue and Topic modes allows for registering a receiver of messages.
    /// Subscription mode (ctor with ISubscriptionClient) allows for registration as a receiver of Topics.
    /// To send Topics use the MessengerClient.
    /// </summary>
    public class MessengerClient : IQueueMessengerClient, ITopicMessengerClient
    {
        #region Fields

        private readonly ISenderClient sender;

        #endregion

        #region Properties

        #endregion

        #region Methods

        public MessengerClient(ISenderClient client)
        {
            this.sender = client;
        }

        public async Task Send(IMessage message)
        {
            string messageBody = string.Empty;
            Type messageType = Type.GetType(message.Type);
            if (messageType == typeof(GeneralMessage))
            {
                var gMessage = message as GeneralMessage;
                messageBody = JsonConvert.SerializeObject(gMessage);
            }
            else if (messageType == typeof(GeneralCommand))
            {
                var gCommand = message as GeneralCommand;
                messageBody = JsonConvert.SerializeObject(gCommand);
            }
            else if (messageType == typeof(TopicMessage))
            {
                var gCommand = message as TopicMessage;
                messageBody = JsonConvert.SerializeObject(gCommand);
            }
            else
                throw new NotSupportedException($"The message type of: {message.GetType()} is not supported.");

            try
            {
                var qMessage = new Message(Encoding.UTF8.GetBytes(messageBody));
                qMessage.ContentType = messageType.AssemblyQualifiedName;
                qMessage.CorrelationId = message.Id.ToString();
                await sender.SendAsync(qMessage);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
    }
}
