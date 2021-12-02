using CovidInformationPortal.Services;
using CovidInformationPortal.Models.FilterModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CovidInformationPortal.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : Controller
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IInformationService informationService;

        public ReportsController(
            ILogger<WeatherForecastController> logger, 
            IInformationService informationService
            )
        {
            _logger = logger;
            this.informationService = informationService;
        }

        [HttpPost("firstChart")]
        public async Task<IActionResult> GetFirstChartData([FromBody] FilterModel filter)
        {
            var result = await this.informationService.GatherDataPositiveVsTests(filter);

            return this.Ok(result);
        }
    }
}
