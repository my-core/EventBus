using EventBus.RabbitMQ;
using EventBus.Publisher.EventHandler;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventBus.RabbitMQ.EventBus;

namespace EventBus.Publisher.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;

        private IEventBus _eventBus;
        public TestController(ILogger<TestController> logger, IEventBus eventBus)
        {
            _logger = logger;
            _eventBus = eventBus;
        }

        [HttpGet]
        public string Get()
        {
            _eventBus.Publish(new TestEvent { Message = "test" });
            return "hello";
        }
    }
}
