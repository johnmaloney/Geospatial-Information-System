using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Universal.Contracts.Messaging;

namespace Universal.Tests.Messaging
{
    public class MockQueueObserverClient : IQueueObserverClient
    {
        private Dictionary<Type, IList<Func<IMessage, Task>>> registrations = new Dictionary<Type, IList<Func<IMessage, Task>>>()
        {
            { typeof(GeneralMessage), new List<Func<IMessage, Task>>() },
            { typeof(SpecificMessage), new List<Func<IMessage, Task>>() }
        };

        public void RegisterForNotificationOf<TMessage>(Func<IMessage, Task> messageHandler)
        {
            if (registrations.ContainsKey(typeof(TMessage)))
            {
                // If the delegate list contains the type of TMessage then replace the delegate pointer //
                registrations[typeof(TMessage)].Add(messageHandler);
            }
        }

        public async Task Process(IMessage message)
        {
            if (registrations.ContainsKey(message.GetType()))
            {
                foreach (var handler in registrations[message.GetType()])
                {
                    await handler(message);
                }
            }
        }

        public Task Close()
        {
            throw new NotImplementedException();
        }
    }
}
