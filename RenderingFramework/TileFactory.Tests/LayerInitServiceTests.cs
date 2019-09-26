using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using TileFactory.Models;
using TileFactory.Tests.Utility;
using Microsoft.Extensions.DependencyInjection;
using TileFactory.Utility;
using TileFactory.Transforms;
using TileFactory.Tests.Mocks;
using Microsoft.Extensions.FileProviders;
using System.Linq;
using System.Threading.Tasks;
using Universal.Contracts.Models;
using System.IO;

namespace TileFactory.Tests
{
    [TestClass]
    public class LayerInitServiceTests : ATest
    {
        [TestMethod]
        public async Task given_a_new_init_service_against_files_expect_models()
        {
            var service = new LayerInitializationFileService(Container.GetService<IFileProvider>());
            Assert.IsTrue(service.Models.Any(m => m.Name == "colorado_outline_projected"));

            var coloradoModel = service.Models.First(m => m.Name == "colorado_outline_projected");
            var features = await service.InitializeLayer(coloradoModel.Identifier);
            Assert.AreEqual(1, features.Count());
            Assert.IsNotNull(features.FirstOrDefault(), "The deserialized feature must be populated.");
            Assert.AreEqual(386, features.First().Geometry.First().Count());
        }

        [TestMethod]
        public void add_new_model_to_the_layer_collection_expect_entry()
        {
            var service = new LayerInitializationFileService(Container.GetService<IFileProvider>());

            int beginningCount = service.Models.Count();

            var newLayer = new LayerInformationModel
            {
                Identifier = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString().Substring(0, 6)
            };
            service.AddLayer(newLayer);

            Assert.AreEqual(beginningCount + 1, service.Models.Count());
        }

        [TestMethod]
        public void add_new_model_with_properties_to_the_layer_collection_expect_entry()
        {
            var service = new LayerInitializationFileService(Container.GetService<IFileProvider>());

            int beginningCount = service.Models.Count();

            var newLayer = new LayerInformationModel
            {
                Identifier = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString().Substring(0, 6), 
                Properties = new Property[]
                {
                    new Property { Name = "FileExtension", Value = "json", ValueType = typeof(string) },
                    new Property { Name = "TileAccessTemplate", Value = "http://Server/v1/tiles/tileName/{z}/{x}/{y}.vector.pbf?access_token={token}"}
                }
            };
            service.AddLayer(newLayer);

            Assert.AreEqual(beginningCount + 1, service.Models.Count());
            Assert.AreEqual("json", newLayer.Properties.First(p => p.Name == "FileExtension").Value);
            Assert.AreEqual(
                "http://Server/v1/tiles/tileName/{z}/{x}/{y}.vector.pbf?access_token={token}", 
                newLayer.Properties.First(p => p.Name == "TileAccessTemplate").Value);
        }
    }
}
