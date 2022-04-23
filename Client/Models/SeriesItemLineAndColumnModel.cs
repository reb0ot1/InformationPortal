using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CovidInformationPortal.Client.Models
{
    public class SeriesItemLineAndColumnModel
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [JsonPropertyName("yAxis")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [DataMember(Name = "yAxis")]
        public int? Yaxis { get; set; }

        [DataMember(Name = "series")]
        public double?[] Data { get; set; } = new double?[] { };

        [DataMember(Name = "tooltip")]
        public TootlTipModel Tooltip { get; set; }
    }
}
