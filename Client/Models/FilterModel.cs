using CovidInformationPortal.Client.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CovidInformationPortal.Client.Models
{
    public class FilterModel
    {
        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public DatePeriodType PeriodType { get; set; }
    }
}
