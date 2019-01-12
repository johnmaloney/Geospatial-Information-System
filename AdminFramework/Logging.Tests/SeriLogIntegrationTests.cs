using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Universal.Contracts.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Logging.Tests
{
    [TestClass]
    public class SeriLogIntegrationTests : ATest
    {
        [TestMethod]
        public async Task given_log_entry_send_to_kibana_expect_entry()
        {
            var manager = ATest.Container.GetService<ILogger>();
            var results = await manager.Log(new MockLogEntry() { Type = "Test", Title = "Logging Integratrion Test", Id = 1.0d });

        }
    }

    public class MockLogEntry : ILogEntry
    {
        public string Type  { get; set;}
        public string Title { get; set;}
        public double Id { get; set; }
    }
}
