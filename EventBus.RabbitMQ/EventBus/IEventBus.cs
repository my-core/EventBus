using EventBus.RabbitMQ.Event;
using EventBus.RabbitMQ.EventBus;
using EventBus.RabbitMQ.Handler;
using System;

namespace EventBus.RabbitMQ.EventBus
{

    /// <summary>
    /// 事件总线 接口
    /// </summary>
    public interface IEventBus
    {
        /// <summary>
        /// 发布
        /// </summary>
        /// <param name="event">事件模型</param>
        void Publish(BaseEvent _event);

        /// <summary>
        /// 订阅
        /// </summary>
        /// <typeparam name="T">约束：事件模型</typeparam>
        /// <typeparam name="TH">约束：事件处理器<事件模型></typeparam>
        void Subscribe<T, TH>()
            where T : BaseEvent
            where TH : IBaseEventHandler<T>;

        /// <summary>
        /// 取消订阅
        /// </summary>
        /// <typeparam name="T">约束：事件模型</typeparam>
        /// <typeparam name="TH">约束：事件处理器<事件模型></typeparam>
        void Unsubscribe<T, TH>()
            where T : BaseEvent
            where TH : IBaseEventHandler<T>;

        /// <summary>
        /// 动态订阅
        /// </summary>
        /// <typeparam name="TH">约束：事件处理器</typeparam>
        /// <param name="eventName"></param>
        void SubscribeDynamic<TH>(string eventName)
            where TH : IDynamicBaseEventHandler;

        /// <summary>
        /// 动态取消订阅
        /// </summary>
        /// <typeparam name="TH"></typeparam>
        /// <param name="eventName"></param>
        void UnsubscribeDynamic<TH>(string eventName)
            where TH : IDynamicBaseEventHandler;
    }
}
