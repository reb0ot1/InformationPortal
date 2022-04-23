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
    public class GetDataJob : IJob
    {
        private readonly ILogger<GetDataJob> logger;
        private readonly IRepository<DayInformation> dayInfoRepository;
        public GetDataJob(ILogger<GetDataJob> logger, IRepository<DayInformation> dayInfoRepository)
        {
            this.logger = logger;
            this.dayInfoRepository = dayInfoRepository;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var result = await this.GetData();

                Dictionary<string, string> propNameAttribute = new Dictionary<string, string>();
                PropertyInfo[] propertiesInfo = typeof(DailyInformationModel).GetProperties();
                foreach (var propertyInfo in propertiesInfo)
                {
                    object[] attrs = propertyInfo.GetCustomAttributes(true);
                    var attr = attrs.FirstOrDefault();
                    if (attr != null)
                    {
                        PropertyCustomName cna = attr as PropertyCustomName;
                        propNameAttribute.Add(cna.Name, propertyInfo.Name);
                    }
                }

                string[] names = result.First();
                string[] data = result.Last();
                var propsHelper = PropertyHelper.GetProperties(typeof(DailyInformationModel));
                var instance = new DailyInformationModel();

                for (int i = 0; i < names.Count(); i++)
                {
                    var value = data[i];
                    var prop = propsHelper.FirstOrDefault(pr => pr.Name == propNameAttribute[names[i]]);
                    var converter = TypeDescriptor.GetConverter(prop.PropertyType);
                    var convertedObject = converter.ConvertFromString(value);
                    prop.SetValue(instance, convertedObject);
                }

                this.logger.LogInformation("Ready");

            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Something went wrong when processing data.");
            }
            
        }

        private async Task<List<string[]>> GetData()
        {
            var httpClient = new HttpClient();
            var getResult = await httpClient
                .GetStreamAsync("https://data.egov.bg/resource/download/e59f95dd-afde-43af-83c8-ea2916badd19/json");
            var result = new List<string[]>();
            using (getResult)
            {
                result = JsonSerializer.Deserialize<List<string[]>>(getResult);
            }

            return result;
        }
    }
}
