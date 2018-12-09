using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Messaging.Tests.Integration.Utility
{
    public abstract class ATest
    {
        private IServiceCollection Registrations;
        protected ServiceProvider Container;

        [TestInitialize]
        public void EachTestInitialization()
        {

        }
    }
}
