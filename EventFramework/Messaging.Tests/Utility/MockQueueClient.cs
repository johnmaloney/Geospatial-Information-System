using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Messaging.Tests.Utility
{
    public class MockQueueClient : IQueueClient
    {
        #region Used

        private List<Func<Message, CancellationToken, Task>> handlers = new List<Func<Message, CancellationToken, Task>>();

        public async Task SendAsync(Message message)
        {
            await Task.Factory.StartNew(() => 
               {
                   
                   var property = typeof(Message.SystemPropertiesCollection).GetProperty("SequenceNumber");
                   property.SetValue(message.SystemProperties, 1);

                    foreach (var handler in handlers)
                        handler(message, new CancellationToken());
                });
        }

        #endregion

        #region Unused

        public string QueueName => throw new NotImplementedException();

        public int PrefetchCount { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ReceiveMode ReceiveMode => throw new NotImplementedException();

        public string ClientId => throw new NotImplementedException();

        public bool IsClosedOrClosing => throw new NotImplementedException();

        public string Path => throw new NotImplementedException();

        public TimeSpan OperationTimeout { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ServiceBusConnection ServiceBusConnection => throw new NotImplementedException();

        public bool OwnsConnection => throw new NotImplementedException();

        public IList<ServiceBusPlugin> RegisteredPlugins => throw new NotImplementedException();

        public Task AbandonAsync(string lockToken, IDictionary<string, object> propertiesToModify = null)
        {
            throw new NotImplementedException();
        }

        public Task CancelScheduledMessageAsync(long sequenceNumber)
        {
            throw new NotImplementedException();
        }

        public Task CloseAsync()
        {
            throw new NotImplementedException();
        }

        public Task CompleteAsync(string lockToken)
        {
            return Task.FromResult(true);
        }

        public Task DeadLetterAsync(string lockToken, IDictionary<string, object> propertiesToModify = null)
        {
            throw new NotImplementedException();
        }

        public Task DeadLetterAsync(string lockToken, string deadLetterReason, string deadLetterErrorDescription = null)
        {
            throw new NotImplementedException();
        }

        public void RegisterMessageHandler(Func<Message, CancellationToken, Task> handler, Func<ExceptionReceivedEventArgs, Task> exceptionReceivedHandler)
        {
            this.handlers.Add(handler);
        }

        public void RegisterMessageHandler(Func<Message, CancellationToken, Task> handler, MessageHandlerOptions messageHandlerOptions)
        {
            this.handlers.Add(handler);
        }

        public void RegisterPlugin(ServiceBusPlugin serviceBusPlugin)
        {
            throw new NotImplementedException();
        }

        public void RegisterSessionHandler(Func<IMessageSession, Message, CancellationToken, Task> handler, Func<ExceptionReceivedEventArgs, Task> exceptionReceivedHandler)
        {
            throw new NotImplementedException();
        }

        public void RegisterSessionHandler(Func<IMessageSession, Message, CancellationToken, Task> handler, SessionHandlerOptions sessionHandlerOptions)
        {
            throw new NotImplementedException();
        }

        public Task<long> ScheduleMessageAsync(Message message, DateTimeOffset scheduleEnqueueTimeUtc)
        {
            throw new NotImplementedException();
        }
        
        public Task SendAsync(IList<Message> messageList)
        {
            throw new NotImplementedException();
        }

        public void UnregisterPlugin(string serviceBusPluginName)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
