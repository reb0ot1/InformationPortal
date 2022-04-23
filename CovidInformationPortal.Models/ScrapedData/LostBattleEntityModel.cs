using CovidInformationPortal.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace CovidInformationPortal.Models.ScrapedData
{
    public class LostBattleEntityModel
    {
        public int Age { get; set; }

        public Gender Gender { get; set; }

        public bool HasAdditionalHealthProblems { get; set; }
    }
}
