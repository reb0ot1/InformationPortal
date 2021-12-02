using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CovidInformationPortal.Client.Models.Response
{
    public class LineChartDataModel
    {
        public string[] Categories { get; set; } = new string[] { };

        public List<SeriesItemModel> Series { get; set; } = new List<SeriesItemModel>();
    }
}
