
using EventBus.RabbitMQ.Event;
using EventBus.RabbitMQ.EventBus;
using EventBus.RabbitMQ.Handler;
using EventBus.RabbitMQ.Options;
using EventBus.RabbitMQ.Subscriptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.RabbitMQ.RabbitMQPersistent
{
    /// <summary>
    /// 基于RabbitMQ的事件总线
    /// </summary>
    public class EventBusRabbitMQ : IEventBus, IDisposable
    {
        private readonly ILogger<EventBusRabbitMQ> _logger;
        private readonly IRabbitMQConnection _persistentConnection;
        private readonly IEventBusSubscriptionsManager _subsManager;
        private readonly IServiceProvider _serviceProvider;
        private readonly RabbitMqSetting _rabbitMqSetting;
        private IModel _consumerChannel;

        /// <summary>
        /// RabbitMQ事件总线
        /// </summary>
        /// <param name="logger">日志</param>
        /// <param name="persistentConnection">RabbitMQ持久连接</param>
        /// <param name="subsManager">事件总线订阅管理器</param>
        /// <param name="serviceProvider"></param>
        /// <param name="rabbitMqSetting"></param>
        public EventBusRabbitMQ(ILogger<EventBusRabbitMQ> logger, IRabbitMQConnection persistentConnection, 
            IEventBusSubscriptionsManager subsManager, IServiceProvider serviceProvider, IOptions<RabbitMqSetting> rabbitMqSetting)
        {
            _logger = logger;
            _persistentConnection = persistentConnection;
            _subsManager = subsManager;
            _serviceProvider = serviceProvider;
            _rabbitMqSetting = rabbitMqSetting.Value;
            //创建信道
            _consumerChannel = CreateConsumerChannel();
            //注册订阅移除事件
            _subsManager.OnEventRemoved += SubsManager_OnEventRemoved;
        }

        /// <summary>
        /// 订阅管理器事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventName"></param>
        private void SubsManager_OnEventRemoved(object sender, string eventName)
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }
            using (var channel = _persistentConnection.CreateModel())
            {
                channel.QueueUnbind(queue: _rabbitMqSetting.QueueName, exchange: _rabbitMqSetting.BrokerName, routingKey: eventName);
                if (_subsManager.IsEmpty)
                {
                    _consumerChannel.Close();
                }
            }
        }

        /// <summary>
        /// 发布
        /// </summary>
        /// <param name="_event">事件模型</param>
        public void Publish(BaseEvent _event)
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            var policy = Policy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(_rabbitMqSetting.RetryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {
                    _logger.LogWarning(ex, "Could not publish event: {EventId} after {Timeout}s ({ExceptionMessage})", _event.Id, $"{time.TotalSeconds:n1}", ex.Message);
                });
            var eventName = _event.GetType().Name;
            _logger.LogTrace("Creating RabbitMQ channel to publish event: {EventId} ({EventName})", _event.Id, eventName);

            using (var channel = _persistentConnection.CreateModel())
            {
                _logger.LogTrace("Declaring RabbitMQ exchange to publish event: {EventId}", _event.Id);

                channel.ExchangeDeclare(exchange: _rabbitMqSetting.BrokerName, type: "direct");
                var message = JsonConvert.SerializeObject(_event);
                var body = Encoding.UTF8.GetBytes(message);
                policy.Execute(() =>
                {
                    var properties = channel.CreateBasicProperties();
                    properties.DeliveryMode = 2; // persistent

                    _logger.LogTrace("Publishing event to RabbitMQ: {EventId}", _event.Id);

                    channel.BasicPublish(
                        exchange: _rabbitMqSetting.BrokerName,
                        routingKey: eventName,
                        mandatory: true,
                        basicProperties: properties,
                        body: body);
                });
            }
        }

        /// <summary>
        /// 订阅
        /// 动态
        /// </summary>
        /// <typeparam name="TH">事件处理器</typeparam>
        /// <param name="eventName">事件名</param>
        public void SubscribeDynamic<TH>(string eventName)
            where TH : IDynamicBaseEventHandler
        {
            _logger.LogInformation("Subscribing to dynamic event {EventName} with {EventHandler}", eventName, typeof(TH).FullName);

            DoInternalSubscription(eventName);
            _subsManager.AddDynamicSubscription<TH>(eventName);
            StartBasicConsume();
        }

        /// <summary>
        /// 订阅
        /// </summary>
        /// <typeparam name="T">约束：事件模型</typeparam>
        /// <typeparam name="TH">约束：事件处理器<事件模型></typeparam>
        public void Subscribe<T, TH>()
            where T : BaseEvent
            where TH : IBaseEventHandler<T>
        {
            var eventName = _subsManager.GetEventKey<T>();
            DoInternalSubscription(eventName);

            _logger.LogInformation("Subscribing to event {EventName} with {EventHandler}", eventName, typeof(TH).FullName);

            _subsManager.AddSubscription<T, TH>();
            StartBasicConsume();
        }

        private void DoInternalSubscription(string eventName)
        {
            var containsKey = _subsManager.HasSubscriptionsForEvent(eventName);
            if (!containsKey)
            {
                if (!_persistentConnection.IsConnected)
                {
                    _persistentConnection.TryConnect();
                }

                using (var channel = _persistentConnection.CreateModel())
                {
                    channel.QueueBind(queue: _rabbitMqSetting.QueueName, exchange: _rabbitMqSetting.BrokerName, routingKey: eventName);
                }
            }
        }

        /// <summary>
        /// 取消订阅
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TH"></typeparam>
        public void Unsubscribe<T, TH>()
            where T : BaseEvent
            where TH : IBaseEventHandler<T>
        {
            var eventName = _subsManager.GetEventKey<T>();
            _logger.LogInformation("Unsubscribing from event {EventName}", eventName);
            _subsManager.RemoveSubscription<T, TH>();
        }

        public void UnsubscribeDynamic<TH>(string eventName)
            where TH : IDynamicBaseEventHandler
        {
            _subsManager.RemoveDynamicSubscription<TH>(eventName);
        }

        public void Dispose()
        {
            if (_consumerChannel != null)
            {
                _consumerChannel.Dispose();
            }
            _subsManager.Clear();
        }

        /// <summary>
        /// 开始基本消费
        /// </summary>
        private void StartBasicConsume()
        {
            _logger.LogTrace("Starting RabbitMQ basic consume");

            if (_consumerChannel != null)
            {
                var consumer = new AsyncEventingBasicConsumer(_consumerChannel);
                consumer.Received += Consumer_Received;
                _consumerChannel.BasicConsume(queue: _rabbitMqSetting.QueueName, autoAck: false, consumer: consumer);
            }
            else
            {
                _logger.LogError("StartBasicConsume can't call on _consumerChannel == null");
            }
        }

        /// <summary>
        /// 消费者接受到
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        private async Task Consumer_Received(object sender, BasicDeliverEventArgs eventArgs)
        {
            var eventName = eventArgs.RoutingKey;
            var message = Encoding.UTF8.GetString(eventArgs.Body.Span);

            try
            {
                if (message.ToLowerInvariant().Contains("throw-fake-exception"))
                {
                    throw new InvalidOperationException($"Fake exception requested: \"{message}\"");
                }

                await ProcessEvent(eventName, message);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "----- ERROR Processing message \"{Message}\"", message);
            }

            // Even on exception we take the message off the queue.
            // in a REAL WORLD app this should be handled with a Dead Letter Exchange (DLX). 
            // For more information see: https://www.rabbitmq.com/dlx.html
            _consumerChannel.BasicAck(eventArgs.DeliveryTag, multiple: false);
        }

        /// <summary>
        /// 创造消费通道
        /// </summary>
        /// <returns></returns>
        private IModel CreateConsumerChannel()
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            _logger.LogTrace("Creating RabbitMQ consumer channel");

            var channel = _persistentConnection.CreateModel();

            channel.ExchangeDeclare(exchange: _rabbitMqSetting.BrokerName,
                                    type: "direct");

            channel.QueueDeclare(queue: _rabbitMqSetting.QueueName,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            channel.CallbackException += (sender, ea) =>
            {
                _logger.LogWarning(ea.Exception, "Recreating RabbitMQ consumer channel");

                _consumerChannel.Dispose();
                _consumerChannel = CreateConsumerChannel();
                StartBasicConsume();
            };

            return channel;
        }

        private async Task ProcessEvent(string eventName, string message)
        {
            _logger.LogTrace("Processing RabbitMQ event: {EventName}", eventName);

            if (_subsManager.HasSubscriptionsForEvent(eventName))
            {

                var subscriptions = _subsManager.GetHandlersForEvent(eventName);
                foreach (var subscription in subscriptions)
                {
                    if (subscription.IsDynamic)
                    {
                        var handler = _serviceProvider.GetService(subscription.HandlerType) as IDynamicBaseEventHandler;
                        if (handler == null) continue;
                        dynamic eventData = JObject.Parse(message);

                        await Task.Yield();
                        await handler.Handle(eventData);
                    }
                    else
                    {
                        var handler = _serviceProvider.GetService(subscription.HandlerType);
                        if (handler == null) continue;
                        var eventType = _subsManager.GetEventTypeByName(eventName);
                        var integrationEvent = JsonConvert.DeserializeObject(message, eventType);
                        var concreteType = typeof(IBaseEventHandler<>).MakeGenericType(eventType);

                        await Task.Yield();
                        await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { integrationEvent });
                    }
                }
            }
            else
            {
                _logger.LogWarning("No subscription for RabbitMQ event: {EventName}", eventName);
            }
        }
    }
}
