using CovidInformationPortal.Models.Attributes;
using System;

namespace CovidInformationPortal.Models
{
    public class DailyInformationModel
    {
        [PropertyCustomName("Дата")]
        public DateTime Date { get; set; }

        [PropertyCustomName("Направени тестове")]
        public int TestsUntilNow { get; set; }

        [PropertyCustomName("Тестове за денонощие")]
        public int Tests { get; set; }

        [PropertyCustomName("Потвърдени случаи")]
        public int ConfirmedCases { get; set; }

        [PropertyCustomName("Активни случаи")]
        public int ActiveCases { get; set; }

        [PropertyCustomName("Нови случаи за денонощие")]
        public int Cases { get; set; }

        [PropertyCustomName("Хоспитализирани")]
        public int InHostpitalSoFar { get; set; }

        [PropertyCustomName("Новохоспитализирани")]
        public int InHostpital { get; set; }

        [PropertyCustomName("В интензивно отделение")]
        public int InIntensiveCareUnit { get; set; }

        [PropertyCustomName("Излекувани")]
        public int CuredSoFar { get; set; }

        [PropertyCustomName("Излекувани за денонощие")]
        public int Cured { get; set; }

        [PropertyCustomName("Починали")]
        public int LostBattleSoFar { get; set; }

        [PropertyCustomName("Починали за денонощие")]
        public int LostBattle { get; set; }
    }
}
