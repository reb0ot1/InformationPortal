using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CovidInformationPortal.Client.Models
{
    public class SeriesItemModel
    {
        public string Name { get; set; }
        public double?[] Data { get; set; } = new double?[] { };
    }
}
