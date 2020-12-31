
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
                    //MQ�־û�����
                    services.AddSingleton<IRabbitMQConnection, RabbitMQConnection>();                   
                    //���Ĺ�����
                    services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
                    //�¼�������
                    services.AddTransient<TestEventHandler>();
                    //MQ�¼�����ʵ��
                    services.AddSingleton<IEventBus, EventBusRabbitMQ>();
                    //����
                    services.BuildServiceProvider().GetService<IEventBus>().Subscribe<TestEvent, TestEventHandler>();
                });

    }
}
