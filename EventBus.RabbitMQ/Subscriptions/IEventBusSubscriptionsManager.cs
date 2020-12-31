using EventBus.RabbitMQ.Event;
using EventBus.RabbitMQ.Subscriptions;
using EventBus.RabbitMQ.Handler;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventBus.RabbitMQ.Subscriptions
{
    /// <summary>
    /// 事件总线订阅管理器 接口
    /// </summary>
    public interface IEventBusSubscriptionsManager
    {
        /// <summary>
        /// 判断是否为空
        /// </summary>
        bool IsEmpty { get; }
        /// <summary>
        /// 移除事件
        /// </summary>
        event EventHandler<string> OnEventRemoved;


        /// <summary>
        /// 添加订阅
        /// </summary>
        /// <typeparam name="T">约束：事件</typeparam>
        /// <typeparam name="TH">约束：事件处理器接口<事件></typeparam>
        void AddSubscription<T, TH>()
           where T : BaseEvent
           where TH : IBaseEventHandler<T>;

        /// <summary>
        /// 移除订阅
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TH"></typeparam>
        void RemoveSubscription<T, TH>()
             where T : BaseEvent
             where TH : IBaseEventHandler<T>;

        /// <summary>
        /// 添加动态订阅
        /// </summary>
        /// <typeparam name="TH">约束：动态事件处理器接口</typeparam>
        /// <param name="eventName">事件名</param>
        void AddDynamicSubscription<TH>(string eventName)
           where TH : IDynamicBaseEventHandler;

        /// <summary>
        /// 移除动态订阅
        /// </summary>
        /// <typeparam name="TH"></typeparam>
        /// <param name="eventName"></param>
        void RemoveDynamicSubscription<TH>(string eventName)
            where TH : IDynamicBaseEventHandler;

        /// <summary>
        /// 判断是否存在指定事件名(typeof(T)的订阅
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        bool HasSubscriptionsForEvent<T>() where T : BaseEvent;

        /// <summary>
        /// 判断是否存在指定事件名(eventName)的订阅
        /// </summary>
        /// <param name="eventName"></param>
        /// <returns></returns>
        bool HasSubscriptionsForEvent(string eventName);

        /// <summary>
        /// 获取指定事件名(eventName)的事件type
        /// </summary>
        /// <param name="eventName"></param>
        /// <returns></returns>
        Type GetEventTypeByName(string eventName);

        /// <summary>
        /// 清队订阅事件
        /// </summary>
        void Clear();

        /// <summary>
        /// 获取指定事件名(typeof(T)事件处理器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>() where T : BaseEvent;

        /// <summary>
        /// 获取指定事件名(eventName)事件处理器
        /// </summary>
        /// <param name="eventName"></param>
        /// <returns></returns>
        IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        string GetEventKey<T>();
    }
}
