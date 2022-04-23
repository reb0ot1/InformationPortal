using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace CovidInformationPortal.Client.Models.Response
{
    [DataContract]
    public class LineChartDataModel
    {
        [DataMember(Name = "categories")]
        public string[] Categories { get; set; } = new string[] { };

        [DataMember(Name = "series")]
        public List<SeriesItemLineAndColumnModel> Series { get; set; } = new List<SeriesItemLineAndColumnModel>();
    }
}
