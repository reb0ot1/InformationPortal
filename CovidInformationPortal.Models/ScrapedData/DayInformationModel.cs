using CovidInformationPortal.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace CovidInformationPortal.Models.ScrapedData
{
    public class DayInformationModel
    {
        public DateTime DayDate { get; set; }

        public string FileName { get; set; }

        public int? TotalPositive { get; set; }

        public int? TotalTestsMade { get; set; }

        public int? TotalTestsPCR { get; set; }

        public int? TotalTestsQuick { get; set; }

        public int? PositiveWithPCR { get; set; }

        public int? PositiveWithQuickTest { get; set; }

        public int? ActiveSoFar { get; set; }

        public int? CuredSoFar { get; set; }

        public int? Hospital { get; set; }

        public decimal? VaccinatedPercentage { get; set; }

        public int? LoseTheBattle { get; set; }

        public int? PaternId { get; set; }

        public DayInformation MapFrom()
        {
            return new DayInformation
            {
                ActiveSoFar = this.ActiveSoFar,
                CuredSoFar = this.CuredSoFar,
                Date = this.DayDate,
                FileName = this.FileName,
                Hospital = this.Hospital,
                LoseTheBattle = this.LoseTheBattle,
                PositiveWithPCR = this.PositiveWithPCR,
                PositiveWithQuickTest = this.PositiveWithQuickTest,
                TotalTestsPCR = this.TotalTestsPCR,
                TotalTestsQuick = this.TotalTestsQuick,
                ReadTemplateId = this.PaternId ?? 1,
                TotalPositive = this.TotalPositive,
                TotalTestsMade = this.TotalTestsMade,
                VaccinatedPercentage = this.VaccinatedPercentage
            };
        }
    }
}
