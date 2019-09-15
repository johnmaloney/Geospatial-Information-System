using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Universal.Contracts;
using Universal.Contracts.Models;
using Universal.Contracts.Serial;
using Universal.Tests.Messaging;
using Universal.Contracts.Messaging;
using System.Linq;

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

        [TestMethod]
        public void take_serialized_contract_deserialize_using_type_expect_object()
        {
            var gMessage = new GeneralMessage { Id = Guid.NewGuid() };
            var serial = gMessage.SerializeToJson();

            var deserial = serial.DeserializeJson<GeneralMessage>(Type.GetType(gMessage.Type));
            Assert.AreEqual(gMessage.Id, deserial.Id);

            var deserial2 = serial.DeserializeJson<GeneralMessage>(gMessage.Type);
            Assert.AreEqual(gMessage.Id, deserial2.Id);

            IMessage deserial3 = serial.DeserializeJson<IMessage>(gMessage.Type);
            Assert.AreEqual(gMessage.Id, deserial3.Id);
        }

        [TestMethod]
        public void with_general_command_serialize_expect_proper_deserial()
        {
            var command = new GeneralCommand
            {
                Command = "",
                CommandDataCollection = new List<MockCommandData>
                {
                    new MockCommandData { Data = true, DataType = typeof(bool).AssemblyQualifiedName }
                },
                Id = Guid.NewGuid()
            };

            var serial = command.SerializeToJson();
            var deserial = serial.DeserializeJson<IMessage>(command.Type);
            var deserial1 = serial.DeserializeJson<GeneralCommand>();
            Assert.AreEqual(command.Id, deserial.Id);
            Assert.AreEqual(command.CommandDataCollection.Count(), deserial1.CommandDataCollection.Count());
        }
    }
}
