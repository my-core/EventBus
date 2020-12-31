using EventBus.RabbitMQ.Event;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.RabbitMQ.Handler
{

    /// <summary>
    /// 集成事件处理程序
    /// 泛型接口
    /// </summary>
    /// <typeparam name="TBaseEvent"></typeparam>
    public interface IBaseEventHandler<in TBaseEvent> : IBaseEventHandler
       where TBaseEvent : BaseEvent
    {
        /// <summary>
        /// 事件处理入口
        /// </summary>
        /// <param name="_event"></param>
        /// <returns></returns>
        Task Handle(TBaseEvent _event);
    }

    /// <summary>
    /// 集成事件处理程序 基接口
    /// </summary>
    public interface IBaseEventHandler
    {
    }
}
