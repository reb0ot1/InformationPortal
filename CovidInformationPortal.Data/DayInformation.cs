using System;
using System.Collections.Generic;
using System.Text;

namespace CovidInformationPortal.Data
{
    public class DayInformation
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public string FileName { get; set; }

        public int? TotalPositive { get; set; }

        public int? TotalTestsMade { get; set; }

        public int? PositiveWithPCR { get; set; }

        public int? PositiveWithQuickTest { get; set; }

        public int? TotalTestsPCR { get; set; }

        public int? TotalTestsQuick { get; set; }

        public int? ActiveSoFar { get; set; }

        public int? CuredSoFar { get; set; }

        public int? Hospital { get; set; }

        public decimal? VaccinatedPercentage { get; set; }

        public int? LoseTheBattle { get; set; }

        public int ReadTemplateId { get; set; }

        public ReadTemplate ReadTemplate { get; set; }

        public LostBattle LostBattle { get; set; }
    }
}
