using AdminManagementApp.Data;
using AdminManagementApp.Services;
using Logging;
using Messaging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Universal.Contracts.Messaging;

namespace AdminManagementApp
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
            var serviceBus = new QueueClient(ServiceBusConnectionString, Queues.GeneralCommand);
            services.AddTransient<IQueueMessengerClient>(sp =>
                new MessengerClient(serviceBus));

            services.AddTransient<IQueueObserverClient>(sp =>
                new ObserverClient(serviceBus));

            var topicBus = new TopicClient(ServiceBusConnectionString, Topics.GeneralInfo);
            services.AddTransient<ITopicMessengerClient>(sp =>
                new MessengerClient(topicBus));

            var subscriberBus = new SubscriptionClient(ServiceBusConnectionString, Topics.GeneralInfo, "gis");
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

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
