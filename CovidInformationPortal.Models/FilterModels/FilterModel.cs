using CovidInformationPortal.Models.Enums;
using System;

namespace CovidInformationPortal.Models.FilterModels
{
    public class FilterModel
    {
        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public FilterDatePeriodType PeriodType { get; set; } 
    }
}
