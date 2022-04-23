using System.Runtime.Serialization;

namespace CovidInformationPortal.Client.Models
{
    public class TootlTipModel
    {
        [DataMember(Name = "valuesuffix")]
        public string ValueSuffix { get; set; }
    }
}