using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CovidInformationPortal.Models.VisualizationModels
{
    public class LineChartDataModel
    {
        public LineChartDataModel()
        {
            this.Categories = new string[] { };
            this.Series = new List<SeriesItemModel>();
        }

        public string[] Categories { get; set; }

        public List<SeriesItemModel> Series { get; set; }
    }
}
