using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CovidInformationPortal.Models.VisualizationModels
{
    public class SeriesItemModel
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public int? YAxis { get; set; }

        public double?[] Data { get; set; }

        public TootlTipModel Tooltip { get; set; }
    }
}
