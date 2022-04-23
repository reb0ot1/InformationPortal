using System.Runtime.Serialization;

namespace CovidInformationPortal.Models.Response
{
    public class TootlTipApiModel
    {
        [DataMember(Name = "valuesufix")]
        public string ValueSuffix { get; set; }
    }
}