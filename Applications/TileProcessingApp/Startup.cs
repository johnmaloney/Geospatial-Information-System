using Logging;
using Messaging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TileFactory;
using TileFactory.Interfaces;
using TileFactory.Models;
using TileProcessingApp.Models;
using TileProcessingApp.Services;
using Universal.Contracts.Logging;
using Universal.Contracts.Messaging;

namespace TileProcessingApp
{
    public class Startup
    {
        const string ServiceBusConnectionString = "Endpoint=sb://aetosmessaging.servicebus.windows.net/;SharedAccessKeyName=Publisher;SharedAccessKey=knJ9TZyB9kf8kdv/cCcTW4b9/sPCTP5tcX2G9zU1QUE=";
        private Universal.Contracts.Logging.ILogger logger;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            logger = new LogManager(Configuration["ElasticConfiguration:Uri"]);
            logger.Log(new LogEntry { Id = 1, Title = "Administration Management App initializing.", Type = LogType.Information.ToString() });
            services.AddSingleton<Universal.Contracts.Logging.ILogger>(logger);

            var serviceBus = new QueueClient(ServiceBusConnectionString,Queues.GeneralCommand);
            services.AddTransient<IQueueMessengerClient>(sp =>
                new MessengerClient(serviceBus));

            services.AddTransient<IQueueObserverClient>(sp =>
            {
                var observer = new ObserverClient(serviceBus);
                observer.RegisterForLogNotifications(LogProcessor);
                return observer;
            });

            var topicBus = new TopicClient(ServiceBusConnectionString, Topics.GeneralInfo);
            services.AddTransient<ITopicMessengerClient>(sp =>
                new MessengerClient(topicBus));

            var subscriberBus = new SubscriptionClient(ServiceBusConnectionString, Topics.GeneralInfo, "gis");
            services.AddSingleton<ITopicObserverClient>(sp =>
            {
                var observer = new ObserverClient(subscriberBus);
                observer.RegisterForLogNotifications(LogProcessor);
                return observer;
            });

            services.AddSingleton<ProcessingService>();

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

            // Build the container //
            var container = services.BuildServiceProvider();
            var processing = container.GetService<ProcessingService>();
            processing.RegisterNotificationHandlers();
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

        internal async Task LogProcessor(IEnumerable<ILogEntry> entries)
        {
            foreach (var entry in entries)
            {
                await logger.Log(entry);
            }
        }
    }
}
