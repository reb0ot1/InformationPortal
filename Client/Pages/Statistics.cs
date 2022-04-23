using BlazorDateRangePicker;
using CovidInformationPortal.Client.Components;
using CovidInformationPortal.Client.Models;
using CovidInformationPortal.Client.Models.Enums;
using CovidInformationPortal.Client.Models.Response;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Web;
using System.Linq;
using System.Globalization;

namespace CovidInformationPortal.Client.Pages
{
    public partial class Statistics
    {
        private const string Positive = "Позитивни";
        private const string LostBattle = "Изгубили битката с вируса";
        private const string Negative = "Негативни";
        private const string TestsMaded = "Направени тестове";
        private const string PositiveNegativeRatio = "Съотношение";
        private const string AverageAge = "Средна възраст";
        private const string ValueLabel = "{value}";
        private const string ValueLabelWithPercent = ValueLabel + "%";

        public const string TestsPositiveRatioChartLabel = "Направени тестове / Позитивни / Съотношение";
        public const string LostBattleChartLabel = "Изгубили битката с вируса";
        public const string VaccinatedPossitiveChartLabel = "Процент ваксинирани позитивни";


        protected CultureInfo CultureInfo = CultureInfo.GetCultureInfo("bg-BG");

        [Inject]
        public HttpClient httpClient { get; set; }
        public FilterModel Filter { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        protected LineAndColumnChart LineChartPercents { get; set; }
        protected LineAndColumnChart LineChartComponent { get; set; }
        protected LineChart LineChartPositiveVaccinatedComponent { get; set; }

        public LineAndColumnChartModel LineAndColumnChartPositiveNegative { get; set; }
        public LineAndColumnChartModel LineAndColumnChartLostBattle { get; set; }
        public LineChartModel LineAndColumnChartPositiveVaccinated { get; set; }

        public string DatePeriodTypeSelected;

        protected override void OnInitialized()
        {
            this.DateStart = DateTime.UtcNow;
            this.DateEnd = DateTime.UtcNow.AddMonths(-1);
            DatePeriodTypeSelected = DatePeriodType.Days.ToString();
            Filter = new FilterModel { From = DateStart, To = DateEnd, PeriodType = Models.Enums.DatePeriodType.Months };

            LineAndColumnChartPositiveNegative = new LineAndColumnChartModel {
                YAxisFirstText = $"{TestsMaded} / {Positive}",
                YAxisSecondText = PositiveNegativeRatio,
                YAxisSecondValueFormat = ValueLabelWithPercent
            };
            LineAndColumnChartLostBattle = new LineAndColumnChartModel {
                YAxisFirstText = AverageAge,
                YAxisSecondText = LostBattle,
            };

            LineAndColumnChartPositiveVaccinated = new LineChartModel();
        }

        protected async Task OnDateTypeChange(ChangeEventArgs val)
        {
            this.DatePeriodTypeSelected = val.Value.ToString();
            this.Filter.PeriodType = (DatePeriodType)Enum.Parse(typeof(DatePeriodType), DatePeriodTypeSelected);
            await this.GetResultPositiveVsTestMade();
            await this.GetResultLostBattle();
            await this.GetResultPositiveVaccinated();
        }

        protected async Task OnRangeSelect(DateRange dateRange)
        {
            this.Filter.From = dateRange.Start.DateTime;
            this.Filter.To = dateRange.End.DateTime;
            this.Filter.PeriodType = (DatePeriodType)Enum.Parse(typeof(DatePeriodType), DatePeriodTypeSelected);

            await this.GetResultPositiveVsTestMade();
            await this.GetResultLostBattle();
            await this.GetResultPositiveVaccinated();

        }

        private async Task GetResultPositiveVsTestMade()
        {
            var resultLineChart = await httpClient.PostAsJsonAsync<FilterModel>("api/Reports/positiveVsTestsMade", this.Filter);
            var newInstance = new LineAndColumnChartModel
            {
                YAxisFirstText = $"{TestsMaded} / {Positive}",
                YAxisSecondText = PositiveNegativeRatio,
                YAxisSecondValueFormat = ValueLabelWithPercent,
            };
            if (resultLineChart.IsSuccessStatusCode)
            {
                var responseResult = await resultLineChart.Content.ReadFromJsonAsync<LineChartDataModel>();
                newInstance.Categories = responseResult.Categories;
                newInstance.Series = responseResult.Series;
                this.LineAndColumnChartPositiveNegative = newInstance;
                LineChartPercents.Update = true;
            }
            else
            {
                this.LineAndColumnChartPositiveNegative = newInstance;
            }
        }

        private async Task GetResultLostBattle()
        {
            var resultLineChart = await httpClient.PostAsJsonAsync<FilterModel>("api/Reports/lostBattleChart", this.Filter);
            var newInstance = new LineAndColumnChartModel
            {
                YAxisFirstText = AverageAge,
                YAxisSecondText = LostBattle,
            };

            if (resultLineChart.IsSuccessStatusCode)
            {
                var responseResult = await resultLineChart.Content.ReadFromJsonAsync<LineChartDataModel>();
                newInstance.Categories = responseResult.Categories;
                newInstance.Series = responseResult.Series;

                this.LineAndColumnChartLostBattle = newInstance;

                LineChartComponent.Update = true;
            }
            else
            {
                this.LineAndColumnChartLostBattle = newInstance;
            }
        }

        private async Task GetResultPositiveVaccinated()
        {
            var resultLineChart = await httpClient.PostAsJsonAsync<FilterModel>("api/Reports/positiveVaccinated", this.Filter);
            var newInstance = new LineChartModel();

            if (resultLineChart.IsSuccessStatusCode)
            {
                var responseResult = await resultLineChart.Content.ReadFromJsonAsync<LineChartDataModel>();
                newInstance.Categories = responseResult.Categories;
                newInstance.Series = responseResult.Series;

                this.LineAndColumnChartPositiveVaccinated = newInstance;
            }
            else
            {
                this.LineAndColumnChartPositiveVaccinated = newInstance;
            }

            LineChartPositiveVaccinatedComponent.Update = true;
        }

        void ResetClick(MouseEventArgs e, DateRangePicker picker)
        {
            // Close the picker
            picker.Close();
            // Fire OnRangeSelectEvent
            picker.OnRangeSelect.InvokeAsync(new DateRange());
        }
    }
}
