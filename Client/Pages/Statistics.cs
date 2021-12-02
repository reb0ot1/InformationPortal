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

namespace CovidInformationPortal.Client.Pages
{
    public partial class Statistics
    {
        [Inject]
        public HttpClient httpClient { get; set; }
        public LineChartDataModel LineChartData { get; set; }
        public FilterModel Filter { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }

        protected LineChart LineChart { get; set; }
        public string DatePeriodTypeSelected;

        protected override void OnInitialized()
        {
            this.DateStart = DateTime.UtcNow;
            this.DateEnd = DateTime.UtcNow.AddMonths(-1);
            DatePeriodTypeSelected = DatePeriodType.Days.ToString();
            Filter = new FilterModel { From = DateStart, To = DateEnd, PeriodType = Models.Enums.DatePeriodType.Months };
            LineChartData = new LineChartDataModel();
        }

        protected async Task OnDateTypeChange(ChangeEventArgs val)
        {
            Console.WriteLine(val);
            this.DatePeriodTypeSelected = val.Value.ToString();
            this.Filter.PeriodType = (DatePeriodType)Enum.Parse(typeof(DatePeriodType), DatePeriodTypeSelected);
            await this.GetResult();
        }

        protected async Task OnRangeSelect(DateRange dateRange)
        {
            this.Filter.From = dateRange.Start.DateTime;
            this.Filter.To = dateRange.End.DateTime;
            this.Filter.PeriodType = (DatePeriodType)Enum.Parse(typeof(DatePeriodType), DatePeriodTypeSelected);

            await this.GetResult();

        }

        private async Task GetResult()
        {
            var resultLineChart = await httpClient.PostAsJsonAsync<FilterModel>("api/Reports/firstChart", this.Filter);
            if (resultLineChart.IsSuccessStatusCode)
            {
                var responseResult = await resultLineChart.Content.ReadFromJsonAsync<LineChartDataModel>();
                this.LineChartData = responseResult;
                LineChart.Update = true;
            }
            else
            {
                this.LineChartData = new LineChartDataModel();
            }
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
