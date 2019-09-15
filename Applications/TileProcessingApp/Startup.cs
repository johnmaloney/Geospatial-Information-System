using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Messaging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TileFactory;
using TileFactory.Interfaces;
using TileFactory.Models;
using Universal.Contracts.Messaging;

namespace TileProcessingApp
{
    public class Startup
    {
        const string ServiceBusConnectionString = "Endpoint=sb://aetosmessaging.servicebus.windows.net/;SharedAccessKeyName=Publisher;SharedAccessKey=knJ9TZyB9kf8kdv/cCcTW4b9/sPCTP5tcX2G9zU1QUE=";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var serviceBus = new QueueClient(ServiceBusConnectionString,Queues.GeneralCommand);
            services.AddTransient<IQueueMessengerClient>(sp =>
                new MessengerClient(serviceBus));

            services.AddTransient<IQueueObserverClient>(sp =>
                new ObserverClient(serviceBus));

            var fileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/layers"));

            services.AddSingleton<ILayerInitializationService>(new LayerInitializationFileService(fileProvider));

            services.AddSingleton<IFileProvider>(fileProvider);

            services.AddSingleton<ITileCacheStorage<ITile>>(new SimpleTileCacheStorage<ITile>());
            services.AddSingleton<ITileCacheStorage<ITransformedTile>>(new SimpleTileCacheStorage<ITransformedTile>());
            services.AddTransient<ITileContext>((sp) =>
            {
                return new SimpleTileContext()
                {
                    MaxZoom = 14,
                    Buffer = 64,
                    Extent = 4096,
                    Tolerance = 3
                };
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Adding Cross Origin Request requires the AddCors() call in the ConfigureServices, from this: //
            // https://docs.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-2.2
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                   builder =>
                   {
                       builder.AllowAnyOrigin();
                   });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            // Adding Cross Origin Request requires the AddCors() call in the ConfigureServices, from this: //
            // https://docs.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-2.2
            app.UseCors();
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
