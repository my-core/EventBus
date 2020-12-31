using System;
using System.Collections.Generic;
using System.Text;

namespace EventBus.RabbitMQ.Event
{
    /// <summary>
    /// 事件模型 基类
    /// </summary>
    public class BaseEvent
    {
        public BaseEvent()
        {
            Id = Guid.NewGuid();
            EventTime = DateTime.UtcNow;
        }

        public BaseEvent(Guid id, DateTime eventTime)
        {
            Id = id;
            EventTime = eventTime;
        }

        /// <summary>
        /// 事件id
        /// </summary>
        public Guid Id { get; private set; }
        /// <summary>
        /// 事件发生时间
        /// </summary>
        public DateTime EventTime { get; private set; }
    }
}
