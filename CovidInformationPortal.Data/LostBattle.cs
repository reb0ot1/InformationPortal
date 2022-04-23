namespace CovidInformationPortal.Data
{
    public class LostBattle
    {
        public int Id { get; set; }

        public int Count { get; set; } = 0;

        public double? AverageAge { get; set; }

        public int Males { get; set; }

        public int Females { get; set; }

        public int DayInformationId { get; set; }

        public DayInformation DayInformation { get; set; }
    }
}