using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Universal.Contracts.Messaging;
using Universal.Contracts.Serial;

namespace Universal.Tests.Messaging
{
    [TestClass]
    public class CommandDataTests
    {
        [TestMethod]
        public void given_command_data_expect_data_type_conversion()
        {
            var mockData = new MockCommandData()
            {
                Data = 178.90,
                DataType = typeof(double).AssemblyQualifiedName
            };

            Assert.AreEqual(178.90, mockData.DataAs<double>());
        }
        
        [TestMethod]
        public void given_command_data_expect_data_type_presence_after_serialization()
        {          
            var mockData = new MockCommandData()
            {
                Data = 178.90,
                DataType = typeof(double).AssemblyQualifiedName
            };

            var serial = mockData.SerializeToJson();
            var mockDataDeserial = serial.DeserializeJson<InternalMockCommandData>();

            Assert.AreEqual(178.90, mockDataDeserial.DataAs<double>());
        }

        [TestMethod]
        public void given_command_data_expect_data_type_of_noncore_type()
        {
            var mockData = new MockCommandData()
            {
                Data = new { propertyOne = 10, propertyTwo = "test" },
                DataType = "geojson"
            };

            var serial = mockData.SerializeToJson();
            var mockDataDeserial = serial.DeserializeJson<InternalMockCommandData>();

            Assert.AreEqual("geojson", mockDataDeserial.DataType);
            Assert.AreEqual(10, ((dynamic)mockDataDeserial.Data).propertyOne);
            Assert.AreEqual("test", ((dynamic)mockDataDeserial.Data).propertyTwo);
        }

        internal class InternalMockCommandData : ICommandData
        {
            public string DataType { get; set; }

            public object Data { get; set; }

            public double Version { get { return 1.1; } }

            public T DataAs<T>()
            {
                if (Data == null)
                    return default(T);

                if (typeof(T) == Type.GetType(DataType))
                    return (T)Data;

                throw new InvalidCastException($"The type T:{typeof(T)} is not compatible with the DataType:{DataType}");
            }
        }
    }
}
