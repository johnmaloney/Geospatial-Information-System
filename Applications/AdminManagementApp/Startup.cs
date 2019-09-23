using AdminManagementApp.Data;
using AdminManagementApp.Services;
using Files;
using Files.CloudFileStorage;
using Logging;
using Messaging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using Universal.Contracts.Files;
using Universal.Contracts.Logging;
using Universal.Contracts.Messaging;

namespace AdminManagementApp
{
    public class Startup
    {
        const string publisherConnectionString = "Endpoint=sb://aetosmessaging.servicebus.windows.net/;SharedAccessKeyName=Publisher;SharedAccessKey=knJ9TZyB9kf8kdv/cCcTW4b9/sPCTP5tcX2G9zU1QUE=";
        const string subscriberConnectionString = "Endpoint=sb://aetosmessaging.servicebus.windows.net/;SharedAccessKeyName=Subscriber;SharedAccessKey=AADc5dQr/zv+4s6lbDlaKrdDMq6h38VBKksFHOBPWZY=";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var queuePublisher = new QueueClient(publisherConnectionString, Queues.GeneralCommand);
            services.AddTransient<IQueueMessengerClient>(sp =>
                new MessengerClient(queuePublisher));

            var queueListener = new QueueClient(subscriberConnectionString, Queues.GeneralCommand);
            services.AddTransient<IQueueObserverClient>(sp =>
                new ObserverClient(queueListener));

            var topicBus = new TopicClient(publisherConnectionString, Topics.GeneralInfo);
            services.AddTransient<ITopicMessengerClient>(sp =>
                new MessengerClient(topicBus));

            var subscriberBus = new SubscriptionClient(subscriberConnectionString, Topics.GeneralInfo, "gis");
            services.AddSingleton<ITopicObserverClient>(sp =>
                new ObserverClient(subscriberBus));
            
            // START the Logger, connects to Kibana //
            var logger = new LogManager(Configuration["ElasticConfiguration:Uri"]);
            logger.Log(new LogEntry { Id = 1, Title = "Administration Management App initializing.", Type = LogType.Information.ToString() });
            services.AddSingleton<Universal.Contracts.Logging.ILogger>(logger);

            // ADD a default data storage mechanism //
            services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("Messages"));
            
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            var container = services.BuildServiceProvider();
            var repository = new MessageRepository(
                container.GetService<ApplicationDbContext>());

            services.AddSingleton<MessageRepository>(repository);

            services.AddSingleton<MessagingService>();

            services.AddSingleton<IFileRepository>(fr =>
            {
                return new AzureFileRepository(
                    new AzureFileReader(
                        Configuration["UploadStorageAcctName"],
                        Configuration["StorageAccountKey"],
                        "gis-uploads"));
            });

            services.AddSingleton<FileService>();
        }  

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // TO DEBUG REQUESTS //
            //app.Use(async (context, next) =>
            //{
            //    var r = context.Request;
            //    //var v = await r.ReadFormAsync();

            //    var input = new StreamReader(r.Body).ReadToEnd();
            //    var i = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.JobRequest>(input);
            //    // Call the next delegate/middleware in the pipeline
            //    await next();
            //});

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseMvc();

        }
    }
}
