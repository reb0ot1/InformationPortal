using CovidInformationPortal.Data;
using CovidInformationPortal.Models;
using CovidInformationPortal.Models.Attributes;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Utf8Json;

namespace CovidInformationPortal.Services.Utilities
{
    public class GetDataJob2 : IJob
    {
        private readonly ILogger<GetDataJob2> logger;
        private readonly IDataGatheringService dataGatherService;
        public GetDataJob2(ILogger<GetDataJob2> logger, IDataGatheringService dataGatherService)
        {
            this.logger = logger;
            this.dataGatherService = dataGatherService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var date = DateTime.UtcNow;
                await this.dataGatherService.StartGatheringData(new DateTime(date.Year, date.Month, date.Day, 0, 0, 0));
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Something went wrong when processing data.");
            }
            
        }
    }
}
