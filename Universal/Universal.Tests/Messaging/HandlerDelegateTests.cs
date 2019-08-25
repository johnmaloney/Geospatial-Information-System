using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Universal.Contracts.Messaging;

namespace Universal.Tests.Messaging
{
    [TestClass]
    public class HandlerDelegateTests
    {
        [TestMethod]
        public async Task register_single_delegate_for_notification_expect_message_processing()
        {
            var client = new MockQueueObserverClient();
            client.RegisterForNotificationOf<GeneralMessage>(async (m) =>
            {
                if (m is GeneralMessage gMessage)
                {
                    gMessage.CorrellationId += 1;
                }
                await Task.FromResult(true);
            });

            var message = new GeneralMessage { CorrellationId = 1 };
            await client.Process(message);

            Assert.AreEqual(2, message.CorrellationId, "The message was not properly processed through the IQueueObserverClient");
        }

        [TestMethod]
        public async Task register_multiple_delegates_for_notification_expect_message_processing()
        {
            var client = new MockQueueObserverClient();
            client.RegisterForNotificationOf<GeneralMessage>(async (m) =>
            {
                if (m is GeneralMessage gMessage)
                {
                    gMessage.CorrellationId += 1;
                }
                await Task.FromResult(true);
            });
            client.RegisterForNotificationOf<GeneralMessage>(async (m) =>
            {
                if (m is GeneralMessage gMessage)
                {
                    gMessage.CorrellationId += 1000;
                }
                await Task.FromResult(true);
            });

            var message = new GeneralMessage { CorrellationId = 1 };
            await client.Process(message);

            Assert.AreEqual(1002, message.CorrellationId, "The message was not properly processed through the IQueueObserverClient");
        }

        [TestMethod]
        public async Task register_differing_type_delegates_for_notification_expect_message_processing()
        {
            var client = new MockQueueObserverClient();
            client.RegisterForNotificationOf<GeneralMessage>(async (m) =>
            {
                if (m is GeneralMessage message)
                {
                    message.CorrellationId += 1;
                }
                await Task.FromResult(true);
            });
            client.RegisterForNotificationOf<SpecificMessage>(async (m) =>
            {
                if (m is SpecificMessage message)
                {
                    message.CorrellationId += 1000;
                }
                await Task.FromResult(true);
            });

            var gMessage = new GeneralMessage { CorrellationId = 1 };
            await client.Process(gMessage);

            Assert.AreEqual(2, gMessage.CorrellationId, "The message was not properly processed through the IQueueObserverClient");

            var sMessage = new SpecificMessage { CorrellationId = 1000 };
            await client.Process(sMessage);

            Assert.AreEqual(2000, sMessage.CorrellationId, "The message was not properly processed through the IQueueObserverClient");
        }
    }   
}
