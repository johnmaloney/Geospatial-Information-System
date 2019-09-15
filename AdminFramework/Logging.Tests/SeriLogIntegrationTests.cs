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
            var results = await manager.Log(new MockLogEntry()
            {
                Type = LogType.Information.ToString(),
                Title = "Logging Integration Test",
                Id = 1.0d,
                Document = new ComplexType()
                {
                    Children = new List<ComplexType>
                    {
                        new ComplexType(),
                        new ComplexType()
                    }
                }
            });       
        }
    }

    public class MockLogEntry : ILogEntry
    {
        public string Type  { get; set;}
        public string Title { get; set;}
        public double Id { get; set; }
        public ComplexType Document { get; set; }
    }

    public class ComplexType
    {
        public int Id => 1000;
        public string Message => "This is a test mock object";
        public string Title => "Regal Mock Title";
        public List<ComplexType> Children { get; set; }
    }
}
