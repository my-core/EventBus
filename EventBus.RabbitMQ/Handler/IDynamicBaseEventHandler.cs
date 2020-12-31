using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.RabbitMQ.Handler
{
    /// <summary>
    /// 动态集成事件处理程序 接口
    /// </summary>
    public interface IDynamicBaseEventHandler
    {
        /// <summary>
        /// 事件处理入口
        /// </summary>
        /// <param name="_event"></param>
        /// <returns></returns>
        Task Handle(dynamic _event);
    }
}
