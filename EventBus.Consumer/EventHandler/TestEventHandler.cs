
using EventBus.RabbitMQ.Handler;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventBus.Consumer
{
    public class TestEventHandler : IBaseEventHandler<TestEvent>
    {
        private readonly ILogger<TestEventHandler> _logger;
        public TestEventHandler(ILogger<TestEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(TestEvent _event)
        {
            _logger.LogInformation(JsonConvert.SerializeObject(_event));
            return Task.CompletedTask;
        }
    }
}
