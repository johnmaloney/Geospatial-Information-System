
using Microsoft.Azure.ServiceBus;
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
    
    public class MessengerClient : IQueueMessengerClient
    {
        #region Fields

        private readonly IQueueClient client;

        #endregion

        #region Properties
               
        #endregion

        #region Methods

        public MessengerClient(IQueueClient client)
        {
            this.client = client;
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
                await client.SendAsync(qMessage);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
    }
}
