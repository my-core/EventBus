using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventBus.RabbitMQ.RabbitMQPersistent
{
    /// <summary>
    /// RabbitMQ持久连接 接口
    /// </summary>
    public interface IRabbitMQConnection
    {
        /// <summary>
        /// 判断是否连接
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// 重试连接
        /// </summary>
        /// <returns></returns>
        bool TryConnect();

        /// <summary>
        /// 创建channel信道
        /// </summary>
        /// <returns></returns>
        IModel CreateModel();
    }
}
