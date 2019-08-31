using Messaging.Tests.Utility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Universal.Contracts.Messaging;

namespace Messaging.Tests.Integration.Utility
{
    [TestClass]
    public abstract class ATest
    {
        const string ServiceBusConnectionString = "Endpoint=sb://aetosmessaging.servicebus.windows.net/;SharedAccessKeyName=Publisher;SharedAccessKey=knJ9TZyB9kf8kdv/cCcTW4b9/sPCTP5tcX2G9zU1QUE=";
        protected ServiceProvider Container;

        public ATest()
        {
            var registrations = new ServiceCollection();

            var serviceBus = new MockQueueClient();
            registrations.AddTransient<IQueueMessengerClient>(sp => 
                new MessengerClient(serviceBus));

            registrations.AddTransient<IQueueObserverClient>(sp => 
                new ObserverClient(serviceBus));

            Container = registrations.BuildServiceProvider();           
        }
    }
}
