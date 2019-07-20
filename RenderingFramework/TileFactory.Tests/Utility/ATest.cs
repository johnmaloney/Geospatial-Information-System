using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TileFactory.Interfaces;
using TileFactory.Tests.Utility;
using TileFactory.Utility;

namespace TileFactory.Tests
{
    [TestClass]
    [DeploymentItem(@"Data\", @"Data\")]
    public abstract class ATest
    {
        protected IServiceCollection Registrations;
        protected ServiceProvider Container;

        [TestInitialize]
        public void EachTestInitialization()
        {
            Registrations = new ServiceCollection();
            Registrations.AddSingleton<IConfigurationStrategy>(new ConfigurationStrategy());
            Registrations.AddSingleton<MockContextRepository>(new MockContextRepository());

            // This processing factory needs a delegate that will dictate //
            // a process to instantiate a processing item foreach individual //
            // goemetry item (e.g. Point, Linestring, etc) that is being processed //
            Registrations.AddSingleton<ProjectionProcessingFactory>(
                new ProjectionProcessingFactory(
                    (geoItem)=> new WebMercatorProcessor(geoItem)));

            var currentDirectory = System.Reflection.Assembly.GetExecutingAssembly().Location;
            Registrations.AddSingleton<IFileProvider>(new PhysicalFileProvider(
                Path.Combine(Directory.GetCurrentDirectory(), "data/tiles")));
            Container = Registrations.BuildServiceProvider();
        }
    }
}
