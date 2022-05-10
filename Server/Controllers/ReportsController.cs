using CovidInformationPortal.Services;
using CovidInformationPortal.Models.FilterModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using AutoMapper;
using CovidInformationPortal.Models.Response;
using Quartz.Impl;
using System;
using Quartz;
using Quartz.Impl.Matchers;
using System.Collections.Generic;

namespace CovidInformationPortal.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : Controller
    {
        private readonly ILogger<ReportsController> _logger;
        private readonly IInformationService informationService;
        private readonly IMapper mapper;

        public ReportsController(
            ILogger<ReportsController> logger,
            IInformationService informationService,
            IMapper mapper
            )
        {
            _logger = logger;
            this.informationService = informationService;
            this.mapper = mapper;
        }

        [HttpPost("positiveVsTestsMade")]
        public async Task<IActionResult> PositiveVsTestsMade([FromBody] FilterModel filter)
        {
            var result = await this.informationService.GatherDataPositiveVsTests(filter);

            return this.Ok(this.mapper.Map<LineChartApiModel>(result));
        }

        [HttpPost("lostBattleChart")]
        public async Task<IActionResult> LostBattleData([FromBody] FilterModel filter)
        {
            var result = await this.informationService.GatherLostBattleData(filter);

            return this.Ok(this.mapper.Map<LineChartApiModel>(result));
        }

        [HttpPost("positiveVaccinated")]
        public async Task<IActionResult> PositiveVaccinatedData([FromBody] FilterModel filter)
        {
            var result = await this.informationService.GatherPossitiveVaccinated(filter);

            return this.Ok(this.mapper.Map<LineChartApiModel>(result));
        }

        public async Task<IActionResult> GetJobs()
        {
            StdSchedulerFactory schedulerFactory = new StdSchedulerFactory();
            IScheduler scheduler = schedulerFactory.GetScheduler().Result;

            var all = scheduler.GetCurrentlyExecutingJobs();

            return this.Ok();
        }
    
    }
}
