using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace CovidInformationPortal.Models.Response
{
    public class SeriesItemApiModel
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "xAxis")]
        public int? YAxis { get; set; }

        [DataMember(Name = "data")]
        public double?[] Data { get; set; }

        [DataMember(Name = "tooltip")]
        public TootlTipApiModel Tooltip { get; set; }
    }
}
