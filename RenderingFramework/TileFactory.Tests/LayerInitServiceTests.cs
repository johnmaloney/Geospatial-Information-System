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
    }
}
