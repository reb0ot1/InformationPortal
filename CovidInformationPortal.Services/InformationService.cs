using CovidInformationPortal.Client.Models;
using CovidInformationPortal.Data;
using CovidInformationPortal.Models;
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
        private const string PositiveLabel = "Позитивни";
        private const string MadeTestsLabel = "Направени тестове";
        private const string PercentageLabel = "Съотношение";
        private const string LostBattle = "Загубили битката";
        private const string AverageAge = "Средна възраст";
        private const string DateFormat = "dd-MM-yy";

        private readonly ILogger<InformationService> logger;
        private readonly IRepository<DayInformation> dayInformationRepository;
        private CultureInfo culture = new CultureInfo("bg-BG");

        public InformationService(
            IRepository<DayInformation> dayInformationRepository,
            ILogger<InformationService> logger)
        {
            this.logger = logger;
            this.dayInformationRepository = dayInformationRepository;
        }

        public async Task<LineChartDataModel> GatherLostBattleData(FilterModel filter)
        {
            var allPeriods = this.GatherPeriods(filter);
            var dataContainer = new LineChartDataModel { Categories = allPeriods.ToArray(), Series = new List<SeriesItemModel>() };
            dataContainer.Series.Add(
                    new SeriesItemModel()
                    {
                        Name = LostBattle,
                        Type = "column",
                        Data = new double?[allPeriods.Count()],
                        Tooltip = new TootlTipModel { ValueSuffix = "" },
                        YAxis = 1
                    });

            dataContainer.Series.Add(
                   new SeriesItemModel()
                   {
                       Name = AverageAge,
                       Type = "spline",
                       Data = new double?[allPeriods.Count()],
                       Tooltip = new TootlTipModel { ValueSuffix = "" }
                   });

            var result = await this.FulFillLostBattleData(allPeriods.ToArray(), filter, dataContainer);
            var periods = allPeriods.ToArray();
            for (int i = 0; i < periods.Count(); i++)
            {
                var findDayPeriods = result.FirstOrDefault(e => e.Id == periods[i]);
                if (findDayPeriods != null)
                {
                    dataContainer.Series[0].Data[i] = findDayPeriods.LostBattleCount;
                    var avAge = findDayPeriods.AverageAge;
                    if (avAge != null)
                    {
                        avAge = Math.Round((double)findDayPeriods.AverageAge, 2);
                    }
                    dataContainer.Series[1].Data[i] = avAge;
                }
            }

            return dataContainer;
        }

        public async Task<LineChartDataModel> GatherDataPositiveVsTests(FilterModel filter)
        {
            var allPeriods = this.GatherPeriods(filter);
            var dataContainer = new LineChartDataModel { Categories = allPeriods.ToArray(), Series = new List<SeriesItemModel>() };
            dataContainer.Series.Add(
                new SeriesItemModel 
                { 
                    Name = PositiveLabel, 
                    Type="column", 
                    YAxis = 1, 
                    Data = new double?[allPeriods.Count()]
                });

            dataContainer.Series.Add(
                new SeriesItemModel 
                {
                    Name = MadeTestsLabel,
                    Type = "column", 
                    YAxis = 1, 
                    Data = new double?[allPeriods.Count()] 
                });

            dataContainer.Series.Add(
                    new SeriesItemModel()
                    {
                        Name = PercentageLabel,
                        Type = "spline",
                        Data = new double?[allPeriods.Count()],
                        Tooltip = new TootlTipModel { ValueSuffix = "%"}
                    });

            var result = await this.FulFillData(allPeriods.ToArray(), filter, dataContainer);
            var periods = allPeriods.ToArray();
            for (int i = 0; i < periods.Count(); i++)
            {
                var findDayPeriods = result.FirstOrDefault(e => e.Id == periods[i]);
                if (findDayPeriods != null)
                {
                    var totalCount = findDayPeriods.TotalCount;
                    var totalTest = findDayPeriods.TotalTestsMade;

                    dataContainer.Series[0].Data[i] = totalCount;
                    dataContainer.Series[1].Data[i] = totalTest;
                    if (totalCount != null && totalTest != null && totalTest > 0)
                    {
                        dataContainer.Series[2].Data[i] = Math.Round((double)totalCount / (double)totalTest * 100, 2);
                    }
                }
            }

            return dataContainer;
        }

        public async Task<LineChartDataModel> GatherPossitiveVaccinated(FilterModel filter)
        {
            var allPeriods = this.GatherPeriods(filter);
            var dataContainer = new LineChartDataModel { Categories = allPeriods.ToArray(), Series = new List<SeriesItemModel>() };
            
            dataContainer.Series.Add(
                    new SeriesItemModel()
                    {
                        Name = PercentageLabel,
                        Type = "spline",
                        Data = new double?[allPeriods.Count()]
                    });

            var result = await this.FulFillPositiveVaccinated(allPeriods.ToArray(), filter, dataContainer);
            var periods = allPeriods.ToArray();
            for (int i = 0; i < periods.Count(); i++)
            {
                var findDayPeriods = result.FirstOrDefault(e => e.Id == periods[i]);
                if (findDayPeriods != null)
                {
                    var totalCount = findDayPeriods.VaccinatedPercentage;
                    dataContainer.Series[0].Data[i] = totalCount != null ? (double)totalCount : default;
                }
            }

            return dataContainer;

        }

        private IEnumerable<string> GatherPeriods(FilterModel filter)
        {
            var tempDateTimeFrom = new DateTime(filter.From.Year, filter.From.Month, filter.From.Day);
            var tempDateTimeTo = new DateTime(filter.To.Year, filter.To.Month, filter.To.Day);
            var allPeriods = new List<string>();

            if (filter.PeriodType != Models.Enums.FilterDatePeriodType.Weeks)
            {
                while (tempDateTimeFrom <= tempDateTimeTo)
                {
                    if (filter.PeriodType == Models.Enums.FilterDatePeriodType.Days)
                    {
                        allPeriods.Add($"{tempDateTimeFrom.Date.ToString(DateFormat)}");
                        tempDateTimeFrom = tempDateTimeFrom.AddDays(1);
                    }
                    else if (filter.PeriodType == Models.Enums.FilterDatePeriodType.Months)
                    {
                        allPeriods.Add($"{tempDateTimeFrom.Month}/{tempDateTimeFrom.Year}");
                        tempDateTimeFrom = tempDateTimeFrom.AddMonths(1);
                    }
                }
            }
            else
            {
                allPeriods = this.GetWeeksPeriod(tempDateTimeFrom, tempDateTimeTo).ToList();
            }

            return allPeriods;
        }

        private async Task<IEnumerable<LostBattleModel>> FulFillLostBattleData(string[] periods, FilterModel filter, LineChartDataModel dataContainer)
        {
            IEnumerable<LostBattleModel> result = new List<LostBattleModel>();
            try
            {
                switch (filter.PeriodType)
                {
                    case Models.Enums.FilterDatePeriodType.Days:
                        result = await this.dayInformationRepository
                                                .GetAll()
                                                .Where(e => e.Date >= filter.From && e.Date <= filter.To)
                                                .Select(s => new LostBattleModel(
                                                    s.Date.ToString(DateFormat),
                                                    s.TotalPositive,
                                                    s.LostBattle.Count,
                                                    s.LostBattle.AverageAge)
                                                )
                                                .ToListAsync();
                        break;
                    case Models.Enums.FilterDatePeriodType.Weeks:
                        var queryResult = await this.dayInformationRepository
                        .GetAll()
                        .Where(e => e.Date >= filter.From && e.Date <= filter.To)
                        .Select(s => new { 
                            Date = s.Date, 
                            TotalPositive = s.TotalPositive, 
                            LostBattleCount = s.LostBattle.Count, 
                            LostBattleAgeAv = s.LostBattle.AverageAge
                        })
                        .ToListAsync();

                        result = queryResult
                            .GroupBy(gr => new
                            {
                                WeekNumber = this.culture.Calendar.GetWeekOfYear(
                                                gr.Date,
                                                this.culture.DateTimeFormat.CalendarWeekRule,
                                                this.culture.DateTimeFormat.FirstDayOfWeek
                                                )
                                                .ToString()
                            }
                            )
                            .Select(g => new LostBattleModel(
                                g.Key.WeekNumber, g.Sum(b => b.TotalPositive),
                                g.Sum(a => a.LostBattleCount),
                                g.Average(a => a.LostBattleAgeAv))
                            )
                            .ToList();
                        break;
                    case Models.Enums.FilterDatePeriodType.Months:
                        result = await this.dayInformationRepository
                                            .GetAll()
                                            .Where(e => e.Date >= filter.From && e.Date <= filter.To)
                                            .Select(s => new { Date1 = s.Date, PositiveCount = s.TotalPositive, LostBattleCount = s.LostBattle.Count, AvAge = s.LostBattle.AverageAge})
                                            .GroupBy(o => new {
                                                Month = o.Date1.Month,
                                                Year = o.Date1.Year
                                            })
                                            .Select(g => new LostBattleModel(
                                               string.Format("{0}/{1}", g.Key.Month, g.Key.Year),
                                                g.Sum(e => e.PositiveCount),
                                                g.Sum(e => e.LostBattleCount),
                                                g.Average(e => e.AvAge)
                                            ))
                                            .ToListAsync();
                        break;
                    case Models.Enums.FilterDatePeriodType.Years:
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                ;
            }

            return result;
        }

        private async Task<IEnumerable<PositiveVaccinatedModel>> FulFillPositiveVaccinated(string[] periods, FilterModel filter, LineChartDataModel dataContainer)
        {
            IEnumerable<PositiveVaccinatedModel> result = new List<PositiveVaccinatedModel>();
            try
            {
                switch (filter.PeriodType)
                {
                    case Models.Enums.FilterDatePeriodType.Days:
                        var queryResult = await this.dayInformationRepository
                                                .GetAll()
                                                .Where(e => e.Date >= filter.From && e.Date <= filter.To)
                                                .Select(s => new { 
                                                    Id = s.Date.ToString(DateFormat),
                                                    VaccinatedPercentage = s.VaccinatedPercentage
                                                }
                                                )
                                                .ToListAsync();

                        result = queryResult
                            .Select(w => {
                                var obj = new PositiveVaccinatedModel
                                {
                                    Id = w.Id
                                };
                                if (w.VaccinatedPercentage != null)
                                {
                                    obj.VaccinatedPercentage = Math.Round(100 - (decimal)w.VaccinatedPercentage, 2);
                                }
                                return obj;
                            }).ToList();
                        break;
                    case Models.Enums.FilterDatePeriodType.Months:
                        result = await this.dayInformationRepository
                                            .GetAll()
                                            .Where(e => e.Date >= filter.From && e.Date <= filter.To && e.VaccinatedPercentage != null)
                                            .Select(s => new { Date1 = s.Date, VaccinatedPercentage = s.VaccinatedPercentage})
                                            .GroupBy(o => new {
                                                Month = o.Date1.Month,
                                                Year = o.Date1.Year
                                            })
                                            .Select(g => new PositiveVaccinatedModel { 
                                                Id = string.Format("{0}/{1}", g.Key.Month, g.Key.Year),
                                                VaccinatedPercentage = Math.Round(100 - (decimal)(g.Sum(b => b.VaccinatedPercentage) / g.Where(w => w.VaccinatedPercentage != null).Count()), 2)
                                            }
                                            )
                                            .ToListAsync();
                        break;
                    case Models.Enums.FilterDatePeriodType.Years:
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                ;
            }

            return result;
        }

        private async Task<IEnumerable<PositiveVsTestsModel>> FulFillData(string[] periods, FilterModel filter, LineChartDataModel dataContainer)
        {
            List<PositiveVsTestsModel> result = new List<PositiveVsTestsModel>();
            switch (filter.PeriodType)
            {
                case Models.Enums.FilterDatePeriodType.Days:
                    result = await this.dayInformationRepository
                                            .GetAll()
                                            .Where(e => e.Date >= filter.From && e.Date <= filter.To)
                                            .Select(s => new PositiveVsTestsModel(
                                                s.Date.ToString(DateFormat),
                                                s.TotalPositive, 
                                                s.TotalTestsMade)
                                            )
                                            .ToListAsync();
                    break;
                case Models.Enums.FilterDatePeriodType.Weeks:
                    
                    break;
                case Models.Enums.FilterDatePeriodType.Months:
                    result = await this.dayInformationRepository
                                                .GetAll()
                                                .Where(e => e.Date >= filter.From && e.Date <= filter.To)
                                                .GroupBy(o => new {
                                                    Month = o.Date.Month,
                                                    Year = o.Date.Year
                                                })
                                                .Select(g => new PositiveVsTestsModel(
                                                    string.Format("{0}/{1}", g.Key.Month, g.Key.Year),
                                                    g.Sum(a => a.TotalPositive),
                                                    g.Sum(b => b.TotalTestsMade)
                                                ))
                                                .ToListAsync();
                    break;
                case Models.Enums.FilterDatePeriodType.Years:
                    break;
                default:
                    break;
            }

            return result;
            
        }

        private IEnumerable<string> GetWeeksPeriod(DateTime dateStart, DateTime dateEnd)
        {
            var allPeriods = new List<string>();
            while (dateStart <= dateEnd)
            {
                var weekNumber = this.culture
                                        .Calendar
                                        .GetWeekOfYear(
                                            dateStart, 
                                            this.culture.DateTimeFormat.CalendarWeekRule, 
                                            this.culture.DateTimeFormat.FirstDayOfWeek
                                            )
                                        .ToString();
                if (!allPeriods.Contains(weekNumber))
                {
                    allPeriods.Add(weekNumber);
                }

                dateStart = dateStart.AddDays(1);
            }

            return allPeriods;
        }
    }
}
