using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Universal.Contracts;
using Universal.Contracts.Models;
using Universal.Contracts.Serial;

namespace Universal.Tests.Models
{
    [TestClass]
    public class ModelTests : ATest
    {
        [TestMethod]
        public void with_layer_model_expect_serialization()
        {
            var id = Guid.NewGuid();
            var model = new LayerInformationModel()
            {
                Identifier = id,
                Name = "Colorado Outline",
                Path = "filesystem",
                Properties = new Property[]
                {
                    new Property() { Name = "StorageInfo", Value = "FileSystem", ValueType = typeof(string) }
                }
            };

            var serial = model.SerializeToJson();
            var deserial = serial.DeserializeJson<LayerInformationModel>();
            Assert.AreEqual(id, deserial.Identifier);
            Assert.AreEqual("Colorado Outline", deserial.Name);
            Assert.AreEqual("filesystem", deserial.Path);
            Assert.AreEqual("StorageInfo", deserial.Properties[0].Name);
            Assert.AreEqual("FileSystem", deserial.Properties[0].Value);
            Assert.AreEqual(typeof(string), deserial.Properties[0].ValueType);
        }
    }
}
