using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Universal.Contracts;

namespace Universal.Tests.Versioning
{
    [TestClass]
    public class VersionTests : ATest
    {
        [TestMethod]
        public void with_version_one_fail_to_convert_expect_error()
        {
            var data = Container.GetService<IConfigurationStrategy>().Into<Data>("data");
        }
    }

    internal class Data : IVersion
    {
        public double Id { get; set; }
    }
}
