using Messaging.Tests.Integration.Utility;
using Messaging.Tests.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Universal.Contracts.Messaging;
using Messaging.Models;

namespace Messaging.Tests.Integration
{
    [TestClass]
    public class QueueClientTests : ATest
    {
        [TestMethod]
        public async Task generate_new_client_send_message_expect_response()
        {
            var message = new GeneralMessage { CorrellationId = 1, Id = Guid.NewGuid() };
            var observer = Container.GetService<IQueueObserverClient>();
            observer.RegisterForNotificationOf<GeneralMessage>(m =>
            {
                if (m is GeneralMessage gm)
                    gm.CorrellationId += 1;
                return Task.FromResult(true);
            });

            var messenger = Container.GetService<IQueueMessengerClient>();
            await messenger.Send(message);

            Assert.AreEqual(1, message.CorrellationId);
        }

        [TestMethod]
        public async Task generate_multiple_new_clients_send_message_expect_proper_response()
        {
            var message = new GeneralCommand { CorrellationId = 1, Id = Guid.NewGuid() };
            var observer = Container.GetService<IQueueObserverClient>();
            observer.RegisterForNotificationOf<GeneralMessage>(m =>
            {
                if (m is GeneralMessage gm)
                    gm.CorrellationId += 1;
                return Task.FromResult(true);
            });

            observer.RegisterForNotificationOf<GeneralCommand>(m =>
            {
                if (m is GeneralCommand gm)
                    gm.CorrellationId += 1;
                return Task.FromResult(true);
            });

            var messenger = Container.GetService<IQueueMessengerClient>();
            await messenger.Send(message);

            // this asserts that only one of the handlers above were executed due //
            // to the type of the message //
            Assert.AreEqual(1, message.CorrellationId);
        }
    }
}
