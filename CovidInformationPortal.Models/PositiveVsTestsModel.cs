using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CovidInformationPortal.Client.Models
{
    public class PositiveVsTestsModel
    {
        public PositiveVsTestsModel(string id, int? totalCount, int? totalTestMade)
        {
            this.Id = id;
            this.TotalCount = totalCount;
            this.TotalTestsMade = totalTestMade;
        }

        public string Id { get; set; }

        public int? TotalCount { get; set; }

        public int? TotalTestsMade { get; set; }
    }
}
