using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CovidInformationPortal.Client.Models
{
    public class LineAndColumnChartModel
    {
        public string YAxisFirstText { get; set; } = string.Empty;
        public string YAxisSecondText { get; set; } = string.Empty;
        public string YAxisFirstValueFormat { get; set; } = "{value}";
        public string YAxisSecondValueFormat { get; set; } = "{value}";
        public string[] Categories { get; set; } = new string[] { };
        public List<SeriesItemLineAndColumnModel> Series { get; set; } = new List<SeriesItemLineAndColumnModel>();

    }
}
