using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Logging.Tests
{
    [TestClass]
    public abstract class ATest
    {
        protected static IConfigurationRoot Config;

        [AssemblyInitialize]
        public static void Initialize(TestContext context)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location))
                .AddJsonFile("appsettings.json");

            Config = builder.Build();
        }
    }
}
