using EventBus.RabbitMQ.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventBus.Publisher.EventHandler
{
    /// <summary>
    /// 
    /// </summary>
    public class TestEvent : BaseEvent
    {
        public string Message { get; set; }
    }
}
