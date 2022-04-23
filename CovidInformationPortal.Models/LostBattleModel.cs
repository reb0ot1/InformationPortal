using System;
using System.Collections.Generic;
using System.Text;

namespace CovidInformationPortal.Models
{
    public class LostBattleModel
    {
        public LostBattleModel(
            string id, 
            int? positiveCount, 
            int? lostBattleCount, 
            double? averageAge)
        {
            this.Id = id;
            this.PositiveCount = positiveCount;
            this.LostBattleCount = lostBattleCount;
            this.AverageAge = averageAge;
        }

        public string Id { get; set; }

        public int? PositiveCount { get; set; }

        public int? LostBattleCount { get; set; }

        public double? AverageAge { get; set; }
    }
}
