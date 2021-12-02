using CovidInformationPortal.Data;
using CovidInformationPortal.Models.FilterModels;
using CovidInformationPortal.Models.VisualizationModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace CovidInformationPortal.Services
{
    public class InformationService : IInformationService
    {
        private const string PositiveLabel = "Positive";
        private const string MadeTestsLabel = "Made tests";
        private const string DateFormat = "dd-MM-yy";

        private readonly ILogger<InformationService> logger;
        private readonly IRepository<DayInformation> dayInformationRepository;

        public InformationService(
            IRepository<DayInformation> dayInformationRepository,
            ILogger<InformationService> logger)
        {
            this.logger = logger;
            this.dayInformationRepository = dayInformationRepository;
        }

        public async Task<LineChartDataModel> GatherDataPositiveVsTests(FilterModel filter)
        {
            LineChartDataModel result = new LineChartDataModel();
            switch (filter.PeriodType)
            {
                case Models.Enums.FilterDatePeriodType.Days:
                    result = await this.GetDataByDay(filter);
                    break;
                case Models.Enums.FilterDatePeriodType.Weeks:
                    result = await this.GetDataByWeek(filter);
                    break;
                case Models.Enums.FilterDatePeriodType.Months:
                    result = await this.GetDataByMonth(filter);
                    break;
                case Models.Enums.FilterDatePeriodType.Years:
                    break;
                default:
                    break;
            }

            return result;
        }

        private async Task<LineChartDataModel> GetDataByDay(FilterModel filter)
        {
            var tempDateTimeFrom = new DateTime(filter.From.Year, filter.From.Month, filter.From.Day);
            var tempDateTimeTo = new DateTime(filter.To.Year, filter.To.Month, filter.To.Day);
            var allPeriods = new List<string>();
            while (tempDateTimeFrom <= tempDateTimeTo)
            {
                allPeriods.Add($"{tempDateTimeFrom.Date.ToString(DateFormat)}");
                tempDateTimeFrom = tempDateTimeFrom.AddDays(1);
            }

            var dataContainer = new LineChartDataModel { Categories = allPeriods.ToArray(), Series = new List<SeriesItemModel>() };
            dataContainer.Series.Add(new SeriesItemModel { Name = PositiveLabel, Data = new double?[allPeriods.Count] });
            dataContainer.Series.Add(new SeriesItemModel { Name = MadeTestsLabel, Data = new double?[allPeriods.Count] });

            try
            {
                var ll = await this.dayInformationRepository
                    .GetAll()
                    .Where(e => e.Date >= filter.From && e.Date <= filter.To)
                    .Select(s => new { Id = s.Date.ToString(DateFormat),
                        TotalCount = s.TotalPositive,
                        TotalTestMade = s.TotalTestsMade
                    })
                    .ToListAsync();

                for (int i = 0; i < allPeriods.Count; i++)
                {
                    var findDayPeriods = ll.Where(e => e.Id == allPeriods[i]);
                    
                    if (findDayPeriods.Any())
                    {
                        if (findDayPeriods.Count() > 1)
                        {
                            this.logger.LogWarning($"More than one record for a date. [{findDayPeriods.First().ToString()}]");
                        }
                        dataContainer.Series.First().Data[i] = findDayPeriods.Sum(e => e.TotalCount);
                        dataContainer.Series.Last().Data[i] = findDayPeriods.Sum(e => e.TotalTestMade);
                    }
                }
            }
            catch (Exception ex)
            {
                //this.logger.LogError(ex, $"Something went wrong when trying to get expenses by period. UserId [{this.userPackage.UserId}]");
            }

            return dataContainer;
        }

        private async Task<LineChartDataModel> GetDataByMonth(FilterModel filter)
        {
            LineChartDataModel dataContainer = new LineChartDataModel();
            try
            {
                var tempDateTimeFrom = new DateTime(filter.From.Year, filter.From.Month, filter.From.Day);
                var tempDateTimeTo = new DateTime(filter.To.Year, filter.To.Month, filter.To.Day);
                var allPeriods = new List<string>();
                while (tempDateTimeFrom <= tempDateTimeTo)
                {
                    allPeriods.Add($"{tempDateTimeFrom.Month}/{tempDateTimeFrom.Year}");
                    tempDateTimeFrom = tempDateTimeFrom.AddMonths(1);
                }

                dataContainer.Categories = allPeriods.ToArray();
                dataContainer.Series.Add(new SeriesItemModel { Name = "Positive", Data = new double?[allPeriods.Count] });
                dataContainer.Series.Add(new SeriesItemModel { Name = "Test count", Data = new double?[allPeriods.Count] });

            
                var queryResult = await this.dayInformationRepository
                    .GetAll()
                    .Where(e => e.Date >= filter.From && e.Date <= filter.To)
                    .GroupBy(o => new {
                        Month = o.Date.Month,
                        Year = o.Date.Year
                    })
                    .Select(g => new {
                        Id = string.Format("{0}/{1}", g.Key.Month, g.Key.Year),
                        TotalCount = g.Sum(a => a.TotalPositive),
                        TotalTestMade = g.Sum(b => b.TotalTestsMade)
                    })
                    .ToListAsync();

                for (int i = 0; i < allPeriods.Count; i++)
                {
                    var findPeriod = queryResult.FirstOrDefault(e => e.Id == allPeriods[i]);
                    if (findPeriod != null)
                    {
                        dataContainer.Series.First().Data[i] = findPeriod.TotalCount;
                        dataContainer.Series.Last().Data[i] = findPeriod.TotalTestMade;
                    }
                }
            }
            catch (Exception ex)
            {
                //this.logger.LogError(ex, $"Something went wrong when trying to get expenses by period. UserId [{this.userPackage.UserId}]");
            }

            return dataContainer;
        }

        private async Task<LineChartDataModel> GetDataByWeek(FilterModel filter)
        {
            LineChartDataModel dataContainer = new LineChartDataModel();
            try
            {
                CultureInfo myCI = new CultureInfo("en-US");
                Calendar myCal = myCI.Calendar;
                CalendarWeekRule myCWR = myCI.DateTimeFormat.CalendarWeekRule;
                DayOfWeek myFirstDOW = myCI.DateTimeFormat.FirstDayOfWeek;

                var tempDateTimeFrom = new DateTime(filter.From.Year, filter.From.Month, filter.From.Day);
                var tempDateTimeTo = new DateTime(filter.To.Year, filter.To.Month, filter.To.Day);
                var allPeriods = new List<string>();
                while (tempDateTimeFrom <= tempDateTimeTo)
                {
                    var weekNumber = myCal.GetWeekOfYear(tempDateTimeFrom, myCWR, myFirstDOW).ToString();
                    if (!allPeriods.Contains(weekNumber))
                    {
                        allPeriods.Add(weekNumber);
                    }

                    tempDateTimeFrom = tempDateTimeFrom.AddDays(1);
                }

                dataContainer.Categories = allPeriods.ToArray();
                dataContainer.Series.Add(new SeriesItemModel { Name = "Positive", Data = new double?[allPeriods.Count] });
                dataContainer.Series.Add(new SeriesItemModel { Name = "Test count", Data = new double?[allPeriods.Count] });
            
                var queryResult = await this.dayInformationRepository
                    .GetAll()
                    .Where(e => e.Date >= filter.From && e.Date <= filter.To)
                    .ToListAsync();

                var groupResult = queryResult
                    .GroupBy(gr => new { WeekNumber = myCal.GetWeekOfYear(gr.Date, myCWR, myFirstDOW).ToString() })
                    .Select(g => new
                    {
                        Id = g.Key.WeekNumber,
                        TotalCount = g.Sum(a => a.TotalPositive),
                        TotalTestMade = g.Sum(b => b.TotalTestsMade),
                        AllDaysDate = g.Select(s => s.Date).OrderBy(e => e).ToList()
                    });


                for (int i = 0; i < allPeriods.Count; i++)
                {
                    var findPeriod = groupResult.FirstOrDefault(e => e.Id.ToString() == allPeriods[i]);
                    if (findPeriod != null)
                    {
                        dataContainer.Series.First().Data[i] = findPeriod.TotalCount;
                        dataContainer.Series.Last().Data[i] = findPeriod.TotalTestMade;
                    }
                }
            }
            catch (Exception ex)
            {
                ;
            }

            return dataContainer;
        }
    }
}
