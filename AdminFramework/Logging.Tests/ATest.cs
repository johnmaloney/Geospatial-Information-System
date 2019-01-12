using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Universal.Contracts.Logging;

namespace Logging.Tests
{
    [TestClass]
    public abstract class ATest
    {
        protected static IConfigurationRoot Config;
        protected static IServiceCollection Registrations;
        protected static ServiceProvider Container;

        [AssemblyInitialize]
        public static void Initialize(TestContext context)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location))
                .AddJsonFile("appsettings.json");

            Config = builder.Build();

            Registrations =new ServiceCollection();
            Registrations.AddSingleton<ILogger>(new LogManager(Config["ElasticConfiguration:Uri"]));
            Container = Registrations.BuildServiceProvider();
        }    
    }
}
