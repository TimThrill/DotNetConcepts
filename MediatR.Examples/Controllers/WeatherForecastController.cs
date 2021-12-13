using MediatR.Examples.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediatR.Examples.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IMediator _mediator;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            var requestMessageA = new RequestMessage<ConcreteClassA>
            {
                AbstractClass = new ConcreteClassA()
            };
            var requestMessageB = new RequestMessage<ConcreteClassB>
            {
                AbstractClass = new ConcreteClassB()
            };

            var typeA = await _mediator.Send(requestMessageA);
            var typeB = await _mediator.Send(requestMessageB);

            return new List<WeatherForecast> { new WeatherForecast { Summary = typeA }, new WeatherForecast { Summary = typeB } };
        }
    }
}
