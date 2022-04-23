using System;

namespace CovidInformationPortal.Data
{
    public class DailyInformation
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public int Tests { get; set; }

        public int Cases { get; set; }

        public int InHospital { get; set; }

        public int Cured { get; set; }

        public int LostBattle { get; set; }
    }
}
