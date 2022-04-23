using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CovidInformationPortal.Client.Models
{
    public class LineChartModel
    {
        public string[] Categories { get; set; } = new string[] { };
        public List<SeriesItemLineAndColumnModel> Series { get; set; } = new List<SeriesItemLineAndColumnModel>();
    }
}
