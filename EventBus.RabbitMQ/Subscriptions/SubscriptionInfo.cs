using System;
using System.Collections.Generic;
using System.Text;

namespace EventBus.RabbitMQ.Subscriptions
{
    /// <summary>
    /// 订阅信息
    /// </summary>
    public class SubscriptionInfo
    {
        /// <summary>
        /// 是否动态订阅
        /// </summary>
        public bool IsDynamic { get; }
        /// <summary>
        /// 事件处理器type
        /// </summary>
        public Type HandlerType { get; }

        private SubscriptionInfo(bool isDynamic, Type handlerType)
        {
            IsDynamic = isDynamic;
            HandlerType = handlerType;
        }

        public static SubscriptionInfo Dynamic(Type handlerType)
        {
            return new SubscriptionInfo(true, handlerType);
        }
        public static SubscriptionInfo Typed(Type handlerType)
        {
            return new SubscriptionInfo(false, handlerType);
        }
    }
}
