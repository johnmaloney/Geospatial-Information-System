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
    public class ObserverClient : IQueueObserverClient
    {
        #region Fields

        private readonly IQueueClient client;

        private Dictionary<Type, IList<Func<IMessage, Task>>> registrations = new Dictionary<Type, IList<Func<IMessage, Task>>>()
        {
            { typeof(GeneralMessage), new List<Func<IMessage, Task>>() }
        };

        #endregion

        #region Properties



        #endregion

        #region Methods

        public ObserverClient(IQueueClient client)
        {
            this.client = client;
            // Register QueueClient's MessageHandler and receive messages in a loop //
            RegisterOnMessageHandlerAndReceiveMessages();
        }
        
        public void RegisterForNotificationOf<TMessage>(Func<IMessage, Task> messageHandler)
        {
            if (registrations.ContainsKey(typeof(TMessage)))
            {
                // If the delegate list contains the type of TMessage then replace the delegate pointer //
                registrations[typeof(TMessage)].Add(messageHandler);
            }
        }

        private void RegisterOnMessageHandlerAndReceiveMessages()
        {
            // Configure the MessageHandler Options in terms of exception handling, number of concurrent messages to deliver etc.
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                // Maximum number of Concurrent calls to the callback `ProcessMessagesAsync`, set to 1 for simplicity.
                // Set it according to how many messages the application wants to process in parallel.
                MaxConcurrentCalls = 1,

                // Indicates whether MessagePump should automatically complete the messages after returning from User Callback.
                // False below indicates the Complete will be handled by the User Callback as in `ProcessMessagesAsync` below.
                AutoComplete = false
            };

            // Register the function that will process messages
            client.RegisterMessageHandler(Process, messageHandlerOptions);
        }

        private async Task Process(Message message, CancellationToken token)
        {
            // Process the message
            Console.WriteLine($"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{Encoding.UTF8.GetString(message.Body)}");

            var gMessage = JsonConvert.DeserializeObject<GeneralMessage>(Encoding.UTF8.GetString(message.Body));

            // Complete the message so that it is not received again.
            // This can be done only if the queueClient is created in ReceiveMode.PeekLock mode (which is default).
            await client.CompleteAsync(message.SystemProperties.LockToken);

            // Note: Use the cancellationToken passed as necessary to determine if the queueClient has already been closed.
            // If queueClient has already been Closed, you may chose to not call CompleteAsync() or AbandonAsync() etc. calls 
            // to avoid unnecessary exceptions.

            foreach (var handler in registrations[typeof(GeneralMessage)])
            {
                await handler(gMessage);
            }
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            Console.WriteLine("Exception context for troubleshooting:");
            Console.WriteLine($"- Endpoint: {context.Endpoint}");
            Console.WriteLine($"- Entity Path: {context.EntityPath}");
            Console.WriteLine($"- Executing Action: {context.Action}");
            return Task.CompletedTask;
        }

        #endregion
    }
}
