using CovidInformationPortal.Models.FilterModels;
using CovidInformationPortal.Models.VisualizationModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CovidInformationPortal.Services
{
    public interface IInformationService
    {
        Task<LineChartDataModel> GatherDataPositiveVsTests(FilterModel filter);
    }
}
