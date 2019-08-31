
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
    public class GeneralMessage : IMessage
    {
        public Guid Id { get; set; }
        public int CorrellationId { get; set; }
        public string Type { get { return this.GetType().ToString(); } }
        public int Version { get; set; }
    }

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
            var gMessage = message as GeneralMessage;
            if (gMessage == null)
                throw new NotSupportedException("The message type of: {message.GetType()} is not supported.");

            try
            {
                var body = JsonConvert.SerializeObject(gMessage);
                var qMessage = new Message(Encoding.UTF8.GetBytes(body));
                qMessage.CorrelationId = gMessage.Id.ToString();
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
