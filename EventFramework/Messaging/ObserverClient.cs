using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Universal.Contracts.Messaging;
using Universal.Contracts.Serial;

namespace Messaging
{
    /// <summary>
    /// Provides implementation for the Queue and Suscription modes.
    /// Queue mode (ctor with IReceiverClient) allows for registering a receiver of Queues.
    /// Subscription mode (ctor with ISubscriptionClient) allows for registration as a receiver of Topics.
    /// To send Topics use the MessengerClient.
    /// </summary>
    public class ObserverClient : IQueueObserverClient, ITopicObserverClient
    {
        #region Fields

        private readonly IReceiverClient receiver;
        private readonly bool shouldOnlyReceiveOnce;

        private Dictionary<Type, IList<Func<IMessage, Task>>> registrations = 
            new Dictionary<Type, IList<Func<IMessage, Task>>>();

        #endregion

        #region Properties



        #endregion

        #region Methods
        public ObserverClient(ISubscriptionClient client, bool shouldRecieveOnce = true)
        {
            this.receiver = client;

            shouldOnlyReceiveOnce = shouldRecieveOnce;

            // Register QueueClient's MessageHandler and receive messages in a loop //
            RegisterOnMessageHandlerAndReceiveMessages();
        }

        public ObserverClient(IReceiverClient client)
        {
            // Set the receive once flag for Queues //
            shouldOnlyReceiveOnce = true;

            this.receiver = client;

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
            else
            {
                registrations.Add(typeof(TMessage), new List<Func<IMessage, Task>> { messageHandler });
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
            receiver.RegisterMessageHandler(Process, messageHandlerOptions);
        }

        private async Task Process(Message message, CancellationToken token)
        {
            // Process the message
            Console.WriteLine($"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{Encoding.UTF8.GetString(message.Body)}");

            var contentType = Type.GetType(message.ContentType);

            var gMessage = Encoding.UTF8.GetString(message.Body).DeserializeJson<IMessage>(contentType);

            if (shouldOnlyReceiveOnce)
            {
                // Complete the message so that it is not received again.
                // This can be done only if the queueClient is created in ReceiveMode.PeekLock mode (which is default).
                await receiver.CompleteAsync(message.SystemProperties.LockToken);
            }

            // Note: Use the cancellationToken passed as necessary to determine if the queueClient has already been closed.
            // If queueClient has already been Closed, you may chose to not call CompleteAsync() or AbandonAsync() etc. calls 
            // to avoid unnecessary exceptions.

            foreach (var handler in registrations[contentType])
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
