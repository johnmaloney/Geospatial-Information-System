using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Universal.Tests
{ 
    [TestClass]
    [DeploymentItem(@"Data\", @"Data\")]
    public abstract class ATest
    {
        private IServiceCollection Registrations;
        protected ServiceProvider IsolatedContainer;

        [TestInitialize]
        public void EachTestInitialization()
        {
            Registrations = new ServiceCollection();
            Registrations.AddSingleton<IConfigurationStrategy>(new ConfigurationStrategy());
            IsolatedContainer = Registrations.BuildServiceProvider();
        }
    }
}
