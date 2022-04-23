using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CovidInformationPortal.Models.Response
{
    public class LineChartApiModel
    {
        public LineChartApiModel()
        {
            this.Categories = new string[] { };
            this.Series = new List<SeriesItemApiModel>();
        }

        [DataMember(Name = "categories")]
        public string[] Categories { get; set; }

        [DataMember(Name = "series")]
        public List<SeriesItemApiModel> Series { get; set; }
    }
}
