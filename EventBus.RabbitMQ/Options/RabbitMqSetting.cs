using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.RabbitMQ.Options
{
    /// <summary>
    /// mq配置
    /// </summary>
    public class RabbitMqSetting
    {
        /// <summary>
        /// mq主机
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// mq端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 重连试数
        /// </summary>
        public int RetryCount { get; set; }
        /// <summary>
        /// 消息队列服务器实体名称(用作消息交换机名)
        /// </summary>
        public string BrokerName { get; set; }
        /// <summary>
        /// 队列名称
        /// </summary>
        public string QueueName { get; set; }
    }
    //Broker：简单来说就是消息队列服务器实体。
    //Exchange：消息交换机，它指定消息按什么规则，路由到哪个队列。
    //Queue：消息队列载体，每个消息都会被投入到一个或多个队列。
    //Binding：绑定，它的作用就是把exchange和queue按照路由规则绑定起来。
    //Routing Key：路由关键字，exchange根据这个关键字进行消息投递。
    //vhost：虚拟主机，一个broker里可以开设多个vhost，用作不同用户的权限分离。
    //Producer：消息生产者，就是投递消息的程序。
    //Consumer：消息消费者，就是接受消息的程序。
    //Channel：消息通道，在客户端的每个连接里，可建立多个channel，每个channel代表一个会话任务。
}