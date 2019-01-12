using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Logging.Tests
{
    [TestClass]
    public class BasicLoggingTests : ATest
    {
        [TestMethod]
        public void given_appsettings_expect_value_for_uri()
        {
            var uri = ATest.Config["ElasticConfiguration:Uri"];
            Assert.AreEqual("http://40.118.239.120:9200/", uri);
        }

        
    }
}
