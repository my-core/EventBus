
using EventBus.RabbitMQ.RabbitMQPersistent;
using EventBus.Publisher.EventHandler;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventBus.RabbitMQ.EventBus;
using EventBus.RabbitMQ.Subscriptions;
using EventBus.RabbitMQ.Options;

namespace EventBus.Publisher
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions().Configure<RabbitMqSetting>(Configuration.GetSection("RabbitMqSetting"));
            //MQ�־û�����
            services.AddSingleton<IRabbitMQConnection, RabbitMQConnection>();
            //���Ĺ�����
            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
            //MQ�¼�����ʵ��
            services.AddSingleton<IEventBus, EventBusRabbitMQ>();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
