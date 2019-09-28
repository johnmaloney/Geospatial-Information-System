using Files;
using Files.CloudFileStorage;
using Logging;
using Messaging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
using TileFactory.Layers;
using TileFactory.Models;
using TileProcessingApp.Models;
using TileProcessingApp.Services;
using Universal.Contracts.Files;
using Universal.Contracts.Layers;
using Universal.Contracts.Logging;
using Universal.Contracts.Messaging;
using Universal.Contracts.Tiles;

namespace TileProcessingApp
{
    public class Startup
    {
        const string ServiceBusConnectionString = "Endpoint=sb://aetosmessaging.servicebus.windows.net/;SharedAccessKeyName=Publisher;SharedAccessKey=knJ9TZyB9kf8kdv/cCcTW4b9/sPCTP5tcX2G9zU1QUE=";
        const string subscriberConnectionString = "Endpoint=sb://aetosmessaging.servicebus.windows.net/;SharedAccessKeyName=Subscriber;SharedAccessKey=AADc5dQr/zv+4s6lbDlaKrdDMq6h38VBKksFHOBPWZY=";
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

            var qObserver = new QueueClient(subscriberConnectionString, Queues.GeneralCommand);
            services.AddTransient<IQueueObserverClient>(sp =>
            {
                var observer = new ObserverClient(qObserver);
                observer.RegisterForLogNotifications(LogProcessor);
                return observer;
            });

            var topicBus = new TopicClient(ServiceBusConnectionString, Topics.GeneralInfo);
            services.AddTransient<ITopicMessengerClient>(sp =>
                new MessengerClient(topicBus));

            var subscriberBus = new SubscriptionClient(ServiceBusConnectionString, Topics.GeneralInfo, "gis");
            services.AddSingleton<ITopicObserverClient>(sp =>
            {
                var observer = new ObserverClient(subscriberBus, false);
                observer.RegisterForLogNotifications(LogProcessor);
                return observer;
            });

            services.AddSingleton<LayerTileCacheAccessor>(new LayerTileCacheAccessor(
               () => new SimpleTileCacheStorage<ITransformedTile>(),
               () => new SimpleTileCacheStorage<ITile>()));

            services.AddSingleton<ProcessingService>();
            services.AddSingleton<MessageRepository>(new MessageRepository());

             var fileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/layers"));
            var serverIp = Configuration["ServerAddress:Https"];

            services.AddSingleton<ILayerInitializationService>(new LayerInitializationFileService(fileProvider, serverIp));

            services.AddTransient<Generator>();

            services.AddTransient<TileRetrieverService>();

            services.AddSingleton<IFileProvider>(fileProvider);

            services.AddSingleton<IFileRepository>(fr =>
            {
                return new AzureFileRepository(
                    new AzureFileReader(
                        Configuration["AzureFileStorage:UploadStorageAcctName"],
                        Configuration["AzureFileStorage:StorageAccountKey"],
                        Configuration["AzureFileStorage:FileStoreName"]));
            });

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
                       builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                   });
            });

            // Build the container to allow for the registration of message receivers //
            var container = services.BuildServiceProvider();

            var processing = container.GetService<ProcessingService>();
            processing.RegisterNotificationHandlers(container.GetService<MessageRepository>(), container.GetService<IFileRepository>());
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
