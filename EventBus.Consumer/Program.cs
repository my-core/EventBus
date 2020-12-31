
using EventBus.RabbitMQ.EventBus;
using EventBus.RabbitMQ.Options;
using EventBus.RabbitMQ.RabbitMQPersistent;
using EventBus.RabbitMQ.Subscriptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventBus.Consumer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddOptions().Configure<RabbitMqSetting>(hostContext.Configuration.GetSection("RabbitMqSetting"));
                    //MQ持久化连接
                    services.AddSingleton<IRabbitMQConnection, RabbitMQConnection>();                   
                    //订阅管理器
                    services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
                    //事件处理器
                    services.AddTransient<TestEventHandler>();
                    //MQ事件总线实例
                    services.AddSingleton<IEventBus, EventBusRabbitMQ>();
                    //订阅
                    services.BuildServiceProvider().GetService<IEventBus>().Subscribe<TestEvent, TestEventHandler>();
                });

    }
}
